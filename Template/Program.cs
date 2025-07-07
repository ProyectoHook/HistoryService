using System;
using Application.Interfaces.Repositories;
using Infrastructrure.Persistence;
using Infrastructrure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces.Service;
using Application.UseCase;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var jwtSecretKey = builder.Configuration["Jwt:secretKey"];
var key = Encoding.UTF8.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(options =>
 {
     options.RequireHttpsMetadata = true;
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(key),
         ValidateIssuer = false,
         ValidateAudience = false,
         ValidateLifetime = true,
         ClockSkew = TimeSpan.Zero
     };
 });


// Configuración de CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5500",
                                             "http://127.0.0.1:5500",
                                             "http://127.0.0.1:5501",
                                             "https://127.0.0.1:5500",
                                             "http://127.0.0.1:3000",
                                             "https://slidex-front-end.vercel.app")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials(); // Si usás credenciales
                      });

});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ServiceContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<ISessionHistoryRepository, SessionHistoryRepository>();
builder.Services.AddScoped<ISlideRepository, SlideRepository>();
builder.Services.AddMediatR(typeof(Application.AssemblyReference).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IJwtService, JwtService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();             // OBLIGATORIO para CORS y endpoints

app.UseCors(MyAllowSpecificOrigins);  // CORS debe ir antes de autenticación

app.UseAuthentication();      // Autenticación después de CORS

app.UseAuthorization();

app.MapControllers();

app.Run();
