using Happinest.Services.AuthAPI.CustomModels;
using Happinest.Services.AuthAPI.DataContext;
using Happinest.Services.AuthAPI.Interfaces;
using Happinest.Services.AuthAPI.Services;
using HappinestApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===================== DB CONTEXT =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // or .WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// ===================== DEPENDENCY INJECTION =====================
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Auth Service API",
        Description = "JWT Authentication with PostgreSQL"
    });

    // Add Bearer Token Authorization in Swagger
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nEnter 'Bearer' [space] and then your token.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\"",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// ===================== JWT CONFIGURATION =====================
var key = builder.Configuration["JwtToken:Key"];
string Issuer = builder.Configuration["JwtToken:Issuer"];
string Audiance = builder.Configuration["JwtToken:Audiance"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Issuer,
        ValidAudience = Audiance,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        RoleClaimType = ClaimTypes.Role
    };
    options.Events = new JwtBearerEvents
    {
        OnForbidden = async context =>
        {
            var response = new BaseResponse
            {
                ResponseStatus = false,
                ValidationMessage = "You are not authorized to use this endpoint.",
                StatusCode = Happinest.Services.AuthAPI.Helpers.Constant.StatusCode.Forbidden
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        },
        OnChallenge = async context =>
        {
            context.HandleResponse(); // stops the default logic

            var response = new BaseResponse
            {
                ResponseStatus = false,
                ValidationMessage = "You are not authenticated.",
                StatusCode = Happinest.Services.AuthAPI.Helpers.Constant.StatusCode.Unauthorized
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    };
});

builder.Services.AddAuthorization();

// ===================== APP CONFIGURATION =====================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // ?? IMPORTANT: Must come before UseAuthorization
app.UseAuthorization();
// Use CORS before MapControllers
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
