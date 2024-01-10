using FirstApi.DTO;
using FirstApi.Services;
using FirstApi.ValidationAttributes;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson(option=>
{
    option.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
});
builder.Services.AddScoped<HttpClient>();        
builder.Services.AddScoped<BooksService>();
builder.Services.AddControllers(config =>
{
    config.Filters.Add<ApiValidateModelAttribute>();
});
builder.Services.AddValidatorsFromAssemblyContaining<BookDtpValidator>();

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler("/error");
//app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();