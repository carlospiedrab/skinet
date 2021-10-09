
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API", Version = "v1" });
});

builder.Services.AddDbContext<StoreContext>(options => 
                                            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository,ProductRepository>();

var app = builder.Build();

// Applying the migrations and creating the Database at app 
using (var scope = app.Services.CreateScope())
{
     var services = scope.ServiceProvider;
     var loggerFactory = services.GetRequiredService<ILoggerFactory>();
     try
     {
          var context = services.GetRequiredService<StoreContext>();
          await context.Database.MigrateAsync();
          await StoreContextSeed.SeedAsync(context, loggerFactory);
     }
     catch (System.Exception ex)
     {
         var logger = loggerFactory.CreateLogger<Program>();
         logger.LogError(ex, "An Error occured during migration");
     }
}

// End

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
