using AutoMapper;
using Core.Interfaces;
using Core.Services;
using Dal.Interfaces;
using Dal.Services;
using Dal.UnitOfWork;
using Entities;
using Microsoft.EntityFrameworkCore;
using Models.Configs;
using Orders.API.MappingProfiles;
using System.Net.Mail;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySQLConnectionString = configuration.GetConnectionString("OrderDB");
services.AddDbContext<OrderDBContext>(options => options
    .UseMySql(mySQLConnectionString, ServerVersion.AutoDetect(mySQLConnectionString)));

#region DBContext
services.AddScoped<IOrderDBContext, OrderDBContext>();
#endregion

services.Configure<EmailConfig>(configuration.GetSection("Email"));

#region Inject UoW
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion

#region Inject Core
services.AddScoped<IOrderCore, OrderCore>();
services.AddScoped<IEmailService, EmailCore>();
#endregion

#region Inject Dal
services.AddScoped<IOrderDal, OrderDal>();
services.AddScoped<IIngredientDal, IngredientDal>();
services.AddScoped<IProductDal, ProductDal>();
#endregion


#region AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new OrdersMappingProfile());
});
services.AddSingleton(mappingConfig.CreateMapper());
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
