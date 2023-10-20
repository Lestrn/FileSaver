namespace FileSaver.Infrastructure.Authentication.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using FileSaver.Domain.DTOs;
    using FileSaver.Domain.Interfaces;
    using FileSaver.Domain.Models;
    using FileSaver.Domain.Resources;
    using FileSaver.Infrastructure.Authentication.Interfaces;
    using FileSaver.Infrastructure.Authentication.Validation;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json.Linq;

    public class AuthService : IAuthService
    {
        private readonly IConfiguration config;
        private readonly IEntityRepository<User> userRepository;
        private readonly IEntityRepository<PendingUser> pendingUserRepository;

        public AuthService(IConfiguration config, IEntityRepository<User> entityRepository, IEntityRepository<PendingUser> unconfirmedUserRepository)
        {
            this.config = config;
            this.userRepository = entityRepository;
            this.pendingUserRepository = unconfirmedUserRepository;
        }

        public async Task<JObject> LogIn(UserLoginDTO user)
        {
            User? dbUser = (await this.userRepository.Where((Func<User, bool>)(databaseUser => databaseUser.Email == user.Email && BCrypt.Net.BCrypt.EnhancedVerify(user.Password, databaseUser.Password)))).FirstOrDefault();
            if (dbUser is null)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = "User was not found" });
            }

            return await this.GenerateToken(dbUser);
        }

        public async Task<JObject> RecoveryLogIn(string email, string userCode)
        {
            if (!await this.ConfirmCode(email, userCode, false))
            {
                return JObject.FromObject(new { status = "Bad request", message = "Confirmation failed" });
            }

            User? dbUser = (await this.userRepository.Where(dbUser => dbUser.Email == email)).FirstOrDefault();
            if (dbUser == null)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = $"User with {email} email was not found" });
            }

            return await this.GenerateToken(dbUser);
        }

        public async Task<JObject> Register(UserDTO user)
        {
            EmailValidator validator = new EmailValidator();
            var validationResult = validator.Validate(user.Email);
            if (!validationResult.IsValid)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = "Email validation failed" });
            }

            bool emailAlreadyExists = await this.userRepository.Any(dbUser => dbUser.Email == user.Email);
            if (emailAlreadyExists)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = "Email already exists" });
            }

            bool userNameAlreadyExists = await this.userRepository.Any(dbUser => dbUser.Username == user.Username);
            if (userNameAlreadyExists)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = "Username already exists" });
            }

            bool passwordIsValid = Regex.IsMatch(user.Password, "(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}");
            if (!passwordIsValid)
            {
                return JObject.FromObject(new { status = "Bad request 404", message = ResourceMsgs.InvalidPasswordMsg });
            }

            string subject = "Confirmation Code FileSaver";
            string code = await this.GenerateCode();
            string htmlBody = string.Format(ResourceMsgs.HtmlBodyEmailRegister, user.Username, code);
            bool isSent = await this.SendEmailCode(user.Email, subject, htmlBody);
            if (!isSent)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = "Invalid Email" });
            }

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 13);
            string codeHash = BCrypt.Net.BCrypt.EnhancedHashPassword(code, 13);
            PendingUser? unconfirmedUserDb = (await this.pendingUserRepository.Where(uncUser => uncUser.Email == user.Email)).FirstOrDefault();
            if (unconfirmedUserDb == null)
            {
                unconfirmedUserDb = new PendingUser()
                {
                    Email = user.Email,
                    Password = passwordHash,
                    CorrectCode = codeHash,
                    Username = user.Username,
                };
                this.pendingUserRepository.Add(unconfirmedUserDb);
                await this.pendingUserRepository.SaveChangesAsync();
                var responseNewUser = new
                {
                    status = "Ok", code = 200,
                    Id = unconfirmedUserDb.Id,
                    Email = unconfirmedUserDb.Email,
                };
                return JObject.FromObject(responseNewUser);
            }

            unconfirmedUserDb.CorrectCode = codeHash;
            await this.pendingUserRepository.UpdateAsync(unconfirmedUserDb);
            await this.pendingUserRepository.SaveChangesAsync();
            var responseUserExists = new
            {
                status = "Ok", code = 200,
                Id = unconfirmedUserDb.Id,
                Email = unconfirmedUserDb.Email,
            };
            return JObject.FromObject(responseUserExists);
        }

        public async Task<bool> ConfirmCode(string email, string userCode, bool addToDatabase = true)
        {
            PendingUser? unconfirmedUserDbModel = (await this.pendingUserRepository.Where(uncUser => uncUser.Email == email)).FirstOrDefault();
            if (unconfirmedUserDbModel == null)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.EnhancedVerify(userCode, unconfirmedUserDbModel.CorrectCode))
            {
                return false;
            }

            if (addToDatabase)
            {
                User userDbModel = new User()
                {
                    Email = unconfirmedUserDbModel.Email,
                    Password = unconfirmedUserDbModel.Password,
                    Username = unconfirmedUserDbModel.Username,
                };
                this.userRepository.Add(userDbModel);
            }

            this.pendingUserRepository.Delete(unconfirmedUserDbModel);
            await this.userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<JObject> RecoverAccount(string email)
        {
            User? userDbModel = (await this.userRepository.Where(user => user.Email == email)).FirstOrDefault();
            if (userDbModel == null)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = "User with this email wasnt found" });
            }

            string code = await this.GenerateCode();
            string subject = "Recovery code FileServer";
            string htmlBody = string.Format(ResourceMsgs.HtmlBodyEmailRecovery, userDbModel.Username, code);
            var sendingResult = await this.SendEmailCode(email, subject, htmlBody);
            if (!sendingResult)
            {
                return JObject.FromObject(new { status = "Bad request", code = 404, message = "Invalid Email" });
            }

            string codeHash = BCrypt.Net.BCrypt.EnhancedHashPassword(code, 13);
            PendingUser? unconfirmedUserDbModel = (await this.pendingUserRepository.Where(uncUser => uncUser.Email == email)).FirstOrDefault();
            if (unconfirmedUserDbModel == null)
            {
                unconfirmedUserDbModel = new PendingUser()
                {
                    Email = userDbModel.Email,
                    CorrectCode = codeHash,
                    Password = userDbModel.Password,
                    Username = userDbModel.Username,
                };
                this.pendingUserRepository.Add(unconfirmedUserDbModel);
                await this.pendingUserRepository.SaveChangesAsync();
                var responseAdded = new
                {
                    status = "Ok",
                    code = 200,
                    userEmail = unconfirmedUserDbModel.Email,
                };
                return JObject.FromObject(responseAdded);
            }

            unconfirmedUserDbModel.CorrectCode = codeHash;
            await this.pendingUserRepository.UpdateAsync(unconfirmedUserDbModel);
            await this.pendingUserRepository.SaveChangesAsync();
            var responseUpdated = new
            {
                status = "Ok",
                code = 200,
                userEmail = unconfirmedUserDbModel.Email,
            };
            return JObject.FromObject(responseUpdated);
        }

        private Task<bool> SendEmailCode(string email, string subject, string htmlBody)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(this.config["EmailSettings:Email"], this.config["EmailSettings:Password"]),
                EnableSsl = true,
            };
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(this.config["EmailSettings:Email"]),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);
            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent successfully.");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        private Task<JObject> GenerateToken(User dbUser)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),  new Claim(ClaimTypes.Email, dbUser.Email), new Claim(ClaimTypes.Role, dbUser.Role.ToString()) };
            AuthOptions authOptions = new AuthOptions(this.config);
            var jwt = new JwtSecurityToken(
                    issuer: authOptions.Issuer,
                    audience: authOptions.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(authOptions.Lifetime)),
                    signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                status = "Ok",
                code = 200,
                access_token = encodedJwt,
                validTo = jwt.ValidTo,
                userId = dbUser.Id,
                username = dbUser.Username,
                image = dbUser.Image,
                email = dbUser.Email,
                role = dbUser.Role.ToString(),
            };
            return Task.FromResult(JObject.FromObject(response));
        }

        private Task<string> GenerateCode()
        {
            Random rand = new Random();
            int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                stringBuilder.Append(numbers[rand.Next(0, numbers.Length)]);
            }

            return Task.FromResult(stringBuilder.ToString());
        }
    }
}
