using Microsoft.EntityFrameworkCore;
using EFCore6WebApiTraining.Repository.Db;
using EFCore6WebApiTraining.Repository.Interfaces;
using EFCore6WebApiTraining.Repository.Implements;
using EFCore6WebApiTraining.Repository.Entities;
using EFCore6WebApiTraining.Repository.UnitOfWork.Interfaces;
using EFCore6WebApiTraining.Repository.UnitOfWork.Implements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// EF 6
builder.Services.AddDbContext<BloggingContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));



// Repositoies
builder.Services.AddTransient<IBlogRepository, BlogRepository>();

// GenericRepository
builder.Services.AddTransient<IGenericRepository<Blog>, GenericRepository<Blog>>();
builder.Services.AddTransient<IGenericRepository<Post>, GenericRepository<Post>>();


// GenericUnitOfWork
builder.Services.AddTransient<IGenericUnitOfWork, GenericUnitOfWork>();

// UnitOfWork
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
