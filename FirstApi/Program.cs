using FirstApi.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<HttpClient>();        
builder.Services.AddScoped<BooksService>();
builder.Services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext =>
{
    return new BadRequestObjectResult(new
    {
        Code = 400,
        Messages = actionContext.ModelState.Values.SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/error");
    //app.UseMiddleware<ExceptionHandlingMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();