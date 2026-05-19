using System.Text;
using DayOneAPI.Interfaces;
using DayOneAPI.Services;
using DayOneAPI.Transformers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auth
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<BearerSecurityTransformer>());

var app = builder.Build();

app.MapOpenApi();


app.MapScalarApiReference("/api/v1",options =>
{
    options.WithTitle("DayOne API")
           .WithTheme(ScalarTheme.Purple)
           .AddPreferredSecuritySchemes("Bearer")
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
