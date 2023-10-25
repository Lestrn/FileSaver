namespace FileSaver.API
{
    using FileSaver.Application.Interfaces;
    using FileSaver.Application.Services;
    using FileSaver.Domain.Interfaces;
    using FileSaver.Domain.Models;
    using FileSaver.Domain.Models.Mapping;
    using FileSaver.Infrastructure.Authentication;
    using FileSaver.Infrastructure.Authentication.Interfaces;
    using FileSaver.Infrastructure.Authentication.Services;
    using FileSaver.Infrastructure.Persistence;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<FileSaverContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IEntityRepository<User>, EntityRepository<User>>();
            builder.Services.AddScoped<IEntityRepository<SavedFile>, EntityRepository<SavedFile>>();
            builder.Services.AddScoped<IEntityRepository<PendingUser>, EntityRepository<PendingUser>>();
            builder.Services.AddScoped<IEntityRepository<SharedFile>, EntityRepository<SharedFile>>();
            builder.Services.AddScoped<IEntityRepository<Friendship>, EntityRepository<Friendship>>();
            builder.Services.AddScoped<IEntityRepository<Message>, EntityRepository<Message>>();
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            AuthOptions authOptions = new AuthOptions(config);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                };
            });
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please insert token",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        },
                        Array.Empty<string>()
                    },
                });
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(MappingProfile).Assembly);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(x => x
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .SetIsOriginAllowed(origin => true) // allow any origin
                                                       //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
                   .AllowCredentials()); // allow credentials
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}