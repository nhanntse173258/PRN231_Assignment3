using GrpcServer;
using GrpcServer.Repository;
using GrpcServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<JwtSettings>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var jwtSettings = new JwtSettings();
    configuration.GetSection("JwtSettings").Bind(jwtSettings);
    return jwtSettings;
});


// Add JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };
    });

// Define Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<ItemRepository>();

builder.Services.AddScoped<AuthServiceImpl>();

builder.Services.AddScoped<ItemServiceImpl>();

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuthServiceImpl>();
app.MapGrpcService<ItemServiceImpl>().RequireAuthorization("AdminOnly");
//app.MapGrpcService<ItemServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run("https://localhost:5001");
