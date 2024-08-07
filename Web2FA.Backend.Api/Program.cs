using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Web2FA.Backend.Model.Models;
using Web2FA.Backend.Repository.Interfaces.Base;
using Web2FA.Backend.Repository.Interfaces.Derived;
using Web2FA.Backend.Repository.Repositories.Base;
using Web2FA.Backend.Repository.Repositories.Derived;
using Web2FA.Backend.Service.Interfaces.Derived;
using Web2FA.Backend.Service.Services.Derived;
using Web2FA.Backend.Shared.Constants;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.MapperConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization(config =>
{
    var def = config.DefaultPolicy;
    config.DefaultPolicy = new AuthorizationPolicy(def.Requirements,
        new string[] { "Bearer" });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "Centech Bilişim",
            ValidAudience = "Web2FA",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ApplicationConstants.ApplicationKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<Web2FAContext>(x =>
{
    string? sqlConnectionString = builder.Configuration.GetConnectionString("SqlConnection")?.AsDecrypted();

    if (sqlConnectionString != null && sqlConnectionString.IsNotNullOrEmpty())
    {
        x.UseSqlServer(sqlConnectionString, option =>
        {
            option.MigrationsAssembly(Assembly.GetAssembly(typeof(Web2FAContext))?.GetName().Name);
        });
    }
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
