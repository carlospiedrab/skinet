
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using API.Helpers;
using API.Middleware;
using Microsoft.AspNetCore.Mvc;
using API.Errors;

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
builder.Services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.Configure<ApiBehaviorOptions>(options => 
{
    options.InvalidModelStateResponseFactory = actionContext => 
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();
        var errorResponse = new ApiValidationErroResponse  
        {
            Errors = errors
        }; 
        return new BadRequestObjectResult(errorResponse);
    };
});

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

app.UseMiddleware<ExceptionMiddleware>();

 app.UseSwagger();
 app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
*/

app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/errors/{0}");
//app.UseStatusCodePagesWithReExecute("/Erro", "?statusCode={0}");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
