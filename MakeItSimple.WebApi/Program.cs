using FluentValidation;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures;
using MakeItSimple.WebApi.Common.Behavior;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.ValidatorHandler;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var connectionString = builder.Configuration.GetConnectionString("DevConnection");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
builder.Services.AddDbContext<MisDbContext>(x =>
{
    if (connectionString != null) x.UseMySql(connectionString, serverVersion).UseSnakeCaseNamingConvention();
});

builder.Services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly);

builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddControllers();
builder.Services.AddScoped<ValidatorHandler>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
