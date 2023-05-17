using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using tsapi.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JwtBearerSettings from appsettings.json
builder.Services.Configure<JwtBearerSettings>(builder.Configuration.GetSection("JwtBearer"));
//var cc = builder.Configuration["JwtBearer:Issuer"];


// Configure the JwtBearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtBearer").Get<JwtBearerSettings>();
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

// Configure Kafka consumer
var kafkaSettings = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>();

builder.Services.AddSingleton(new KafkaConsumer<string, string>(kafkaSettings.BootstrapServers, kafkaSettings.Topic, kafkaSettings.GroupId));
builder.Services.AddSingleton(new TransactionService(kafkaSettings.BootstrapServers, kafkaSettings.Topic, kafkaSettings.GroupId));



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

app.Run();
