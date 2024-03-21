using API.InOutClock.API.Configurations;
using API.InOutClock.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<APIInOutClockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("APIInOutClockContext")));

//Inyectar configuracion de JWT, JwtConfig es una clase que contiene las propiedades del token
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

//Configuracion de parametros de validacion del token
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = true
};

//Configuracion de autenticacion para JWT, esquema predeterminado
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{        
    jwt.SaveToken = true;
    jwt.RequireHttpsMetadata = false;
    jwt.TokenValidationParameters = tokenValidationParameters;    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Habilitar autenticacion y autorizacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
