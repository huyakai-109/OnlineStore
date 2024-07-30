using Training.Api.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Training.Common.Constants;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCoreDependencies(config);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config[ConfigKeys.Security.Jwt.Issuer],
            ValidAudience = config[ConfigKeys.Security.Jwt.Audience],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config[ConfigKeys.Security.Jwt.Secret]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authorization = context.Request.Headers[RestfulConstants.RequestHeaders.Authorization];
                if (authorization.Any() && authorization[0].StartsWith(RestfulConstants.RequestHeaders.AuthScheme))
                {
                    context.Token = authorization[0].Substring(RestfulConstants.RequestHeaders.AuthScheme.Length).Trim();
                }
                return Task.CompletedTask;
            }
        };
        options.MapInboundClaims = false;
    });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.RunMigration();

app.Run();
