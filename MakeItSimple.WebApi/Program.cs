using FluentValidation;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Common.Behavior;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Features.SignalRNotification;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.


var connectionString = builder.Configuration.GetConnectionString("DevConnection");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
builder.Services.AddDbContext<MisDbContext>(x =>
{
    if (connectionString != null) x.UseMySql(connectionString, serverVersion)
        .UseSnakeCaseNamingConvention()
        .EnableSensitiveDataLogging();
});

builder.Services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly);

builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddControllers( options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();

    options.Filters.Add(new AuthorizeFilter(policy));

}).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddControllers();
builder.Services.AddScoped<ValidatorHandler>();
builder.Services.AddScoped<TokenGenerator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RDF MakeItSimple(TicketSystem) API", Version = "v1" });

    // Define the BearerAuth scheme in Swagger document
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // Assign the BearerAuth scheme to globally apply to all operations
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwtOptions =>
    {
        var key = builder.Configuration.GetValue<string>("JwtConfig:Key");
        var keyBytes = Encoding.ASCII.GetBytes(key);
        jwtOptions.SaveToken = true;
        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"]
        };
    });

builder.Services.Configure<CloudinaryOption>(config.GetSection("Cloudinary"));

const string clientPermission = "_clientPermission";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: clientPermission, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



builder.Services.AddSignalR();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(clientPermission);


app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();

app.UseWebSockets();


app.MapHub<NotificationHub>("/notification-hub"); // Use top-level route registration
app.MapControllers(); 

                                         app.Run();
