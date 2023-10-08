using FileSaver.Domain.DTOs;
using FileSaver.Domain.Enums;
using FileSaver.Domain.Interfaces;
using FileSaver.Domain.Models;
using FileSaver.Domain.Resources;
using FileSaver.Infrastructure.Authentication.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using FileSaver.Infrastructure.Authentication.Validation;

namespace FileSaver.Infrastructure.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;
        private IEntityRepository<UserDbModel> _userRepository;
        private IEntityRepository<UnconfirmedUserDbModel> _unconfirmedUserRepository;
        public AuthService(IConfiguration config, IEntityRepository<UserDbModel> entityRepository, IEntityRepository<UnconfirmedUserDbModel> unconfirmedUserRepository)
        {
            _config = config;
            _userRepository = entityRepository;
            _unconfirmedUserRepository = unconfirmedUserRepository;
        }
        public async Task<JObject> LogIn(UserLoginDTO user)
        {  
            UserDbModel? dbUser = (await _userRepository.WhereEnumerable(databaseUser => databaseUser.Email == databaseUser.Email && BCrypt.Net.BCrypt.EnhancedVerify(user.Password, databaseUser.Password))).FirstOrDefault();
            if (dbUser is null) return JObject.FromObject(new {status = "Bad request", code = 404, message = "User was not found" });
            return await GenerateToken(dbUser);
        }
        public async Task<JObject> RecoveryLogIn(string email, string userCode)
        {
            if (!await ConfirmCode(email, userCode, false))
            {
                return JObject.FromObject(new {status = "Bad request" , message = "Confirmation failed" });
            }
            UserDbModel? dbUser = (await _userRepository.WhereQueryable(dbUser => dbUser.Email == email)).FirstOrDefault();
            if (dbUser == null)
            {
                return JObject.FromObject(new {status = "Bad request", code = 404, message = $"User with {email} email was not found" });
            }
            return await GenerateToken(dbUser);
        }
        public async Task<JObject> Register(UserDTO user)
        {
            EmailValidator validator = new EmailValidator();
            var validationResult = validator.Validate(user.Email);
            if (!validationResult.IsValid)
            {
                return JObject.FromObject(new {status = "Bad request", code = 404, message = "Email validation failed" }); ;
            }
            bool EmailAlreadyExists = await _userRepository.Any(dbUser => dbUser.Email == user.Email);
            if (EmailAlreadyExists)
            {
                return JObject.FromObject(new {status = "Bad request", code = 404, message = "Email already exists" });
            }
            bool passwordIsValid = Regex.IsMatch(user.Password, "(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}");
            if (!passwordIsValid)
            {
                return JObject.FromObject(new {status = "Bad request 404", message = ResourceMsgs.InvalidPasswordMsg });
            }
            string subject = "Confirmation Code FileSaver";
            string code = await GenerateCode();
            string htmlBody = string.Format(ResourceMsgs.HtmlBodyEmailRegister, user.Username, code);
            bool isSent = await SendEmailCode(user.Email, subject, htmlBody);
            if (!isSent)
            {
                return JObject.FromObject(new {status = "Bad request", code = 404, message = "Invalid Email" });
            }
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 13);
            string codeHash = BCrypt.Net.BCrypt.EnhancedHashPassword(code, 13);
            UnconfirmedUserDbModel? unconfirmedUserDb = (await _unconfirmedUserRepository.WhereQueryable(uncUser => uncUser.Email == user.Email)).FirstOrDefault();
            if (unconfirmedUserDb == null)
            {
                unconfirmedUserDb = new UnconfirmedUserDbModel()
                {
                    Email = user.Email,
                    Password = passwordHash,
                    Role = UserRoles.Basic.ToString(),
                    CorrectCode = codeHash,
                    Username = user.Username
                };
                await _unconfirmedUserRepository.AddAsync(unconfirmedUserDb);
                await _unconfirmedUserRepository.SaveChangesAsync();
                var responseNewUser = new
                {
                    status = "Ok", code = 200,
                    Id = unconfirmedUserDb.Id,
                    Email = unconfirmedUserDb.Email,
                    Role = unconfirmedUserDb.Role
                };
                return JObject.FromObject(responseNewUser);
            }
            unconfirmedUserDb.CorrectCode = codeHash;
            await _unconfirmedUserRepository.UpdateAsync(unconfirmedUserDb);
            await _unconfirmedUserRepository.SaveChangesAsync();
            var responseUserExists = new
            {
                status = "Ok", code = 200,
                Id = unconfirmedUserDb.Id,
                Email = unconfirmedUserDb.Email,
                Role = unconfirmedUserDb.Role
            };
            return JObject.FromObject(responseUserExists);
        }
        public async Task<bool> DeleteAccount(UserDTODelete user)
        {
            UserDbModel userDb = await _userRepository.FindByIdWithIncludesAsync(user.Id, "Files");
            if (userDb == null)
            {
                return false;
            }
            await _userRepository.DeleteAsync(userDb);
            await _userRepository.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ConfirmCode(string email, string userCode, bool addToDatabase = true)
        {
            UnconfirmedUserDbModel? unconfirmedUserDbModel = (await _unconfirmedUserRepository.WhereQueryable(uncUser => uncUser.Email == email)).FirstOrDefault();
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
                UserDbModel userDbModel = new UserDbModel()
                {
                    Email = unconfirmedUserDbModel.Email,
                    Password = unconfirmedUserDbModel.Password,
                    Role = unconfirmedUserDbModel.Role,
                    Image = unconfirmedUserDbModel.Image,
                    Username = unconfirmedUserDbModel.Username
                };
                await _userRepository.AddAsync(userDbModel);
            }
            await _unconfirmedUserRepository.DeleteAsync(unconfirmedUserDbModel);
            await _userRepository.SaveChangesAsync();
            return true;
        }
        public async Task<JObject> RecoverAccount(string email)
        {
            UserDbModel? userDbModel = (await _userRepository.WhereQueryable(user => user.Email == email)).FirstOrDefault();
            if (userDbModel == null)
            {
                return JObject.FromObject(new {status = "Bad request", code = 404, message = "User with this email wasnt found" });
            }
            string code = await GenerateCode();
            string subject = "Recovery code FileServer";
            string htmlBody = string.Format(ResourceMsgs.HtmlBodyEmailRecovery, userDbModel.Username, code);
            var sendingResult = await SendEmailCode(email, subject, htmlBody);
            if (!sendingResult)
            {
                return JObject.FromObject(new {status = "Bad request", code = 404, message = "Invalid Email" });
            }
            string codeHash = BCrypt.Net.BCrypt.EnhancedHashPassword(code, 13);
            UnconfirmedUserDbModel? unconfirmedUserDbModel = (await _unconfirmedUserRepository.WhereQueryable(uncUser => uncUser.Email == email)).FirstOrDefault();
            if (unconfirmedUserDbModel == null)
            {
                unconfirmedUserDbModel = new UnconfirmedUserDbModel()
                {
                    Email = userDbModel.Email,
                    CorrectCode = codeHash,
                    Image = userDbModel.Image,
                    Password = userDbModel.Password,
                    Role = userDbModel.Role,
                    Username = userDbModel.Username
                };
                await _unconfirmedUserRepository.AddAsync(unconfirmedUserDbModel);
                await _unconfirmedUserRepository.SaveChangesAsync();
                var responseAdded = new
                {
                    status = "Ok",
                    code = 200,
                    userEmail = unconfirmedUserDbModel.Email
                };
                return JObject.FromObject(responseAdded);
            }
            unconfirmedUserDbModel.CorrectCode = codeHash;
            await _unconfirmedUserRepository.UpdateAsync(unconfirmedUserDbModel);
            await _unconfirmedUserRepository.SaveChangesAsync();
            var responseUpdated = new
            {
                status = "Ok",
                code = 200,
                userEmail = unconfirmedUserDbModel.Email
            };
            return JObject.FromObject(responseUpdated);
        }
        private Task<bool> SendEmailCode(string email, string subject, string htmlBody)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["EmailSettings:Email"], _config["EmailSettings:Password"]),
                EnableSsl = true,
            };
            Random rand = new Random();
            int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                stringBuilder.Append(numbers[rand.Next(0, numbers.Length)]);
            }

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:Email"]),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
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
        private Task<JObject> GenerateToken(UserDbModel dbUser)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, dbUser.Email), new Claim(ClaimTypes.Role, dbUser.Role.ToString()) };
            AuthOptions authOptions = new AuthOptions(_config);
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
                role = dbUser.Role.ToString()
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
