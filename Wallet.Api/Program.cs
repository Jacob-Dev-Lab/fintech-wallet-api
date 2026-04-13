using System.Text;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Wallet.Api.Middleware;
using Wallet.Api.Validators;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Interfaces;
using Wallet.Application.UseCases;
using Wallet.Application.Utilities;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.Repository;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        // Controller services
        builder.Services.AddControllers();

        // Validator services
        builder.Services.AddFluentValidationAutoValidation(options =>
        {
            options.OverrideDefaultResultFactoryWith<GlobalValidationFactory>();
        });

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        builder.Services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidator>();
        builder.Services.AddScoped<IValidator<DepositRequest>, DepositRequestValidator>();
        builder.Services.AddScoped<IValidator<WithdrawRequest>, WithdrawRequestValidator>();
        builder.Services.AddScoped<IValidator<TransferRequest>, TransferRequestValidator>();

        // Db connection service
        var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<WalletApiDbContext>(options
            => options.UseSqlServer(dbConnection));

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        // JWT authentication service
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HereIsMyJwtToken_A_Special_12345"))
                };
            });

        // Repository services
        builder.Services.AddScoped<IWalletRepository, WalletRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Usecase services
        builder.Services.AddScoped<IWalletService, WalletService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();

        // Utilities
        builder.Services.AddSingleton<IEmailValidator, EmailValidator>();
        builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
                });
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Middleware pipeline
        app.UseSerilogRequestLogging();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}