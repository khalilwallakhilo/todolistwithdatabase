using todolistwithdatabase.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using FluentValidation.AspNetCore;
using todolistwithdatabase;
using System.Security.Cryptography;
using todolistwithdatabase.Middleware;

var builder = WebApplication.CreateBuilder(args);

byte[] secretBytes = new byte[64];
using(var random = RandomNumberGenerator.Create())
{
    random.GetBytes(secretBytes);
}
string secretKey = Convert.ToBase64String(secretBytes);

builder.Services.AddControllers(option => { /*option.ReturnHttpNotAcceptable = true;*/ })
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();

builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Bearer Tokens used for authentication 
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Free Trained",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();
//builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
//    {
//        options.Password.RequiredLength = 6;
//        options.Password.RequireNonAlphanumeric = false;
//        options.Password.RequireDigit = false;
//        options.Password.RequireLowercase = false;
//        options.Password.RequireUppercase = false;
//    })
//    .AddEntityFrameworkStores<ApplicationDbContext>();
    


builder.Services.AddControllers(options =>
{
    // options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters()
.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ToDoListValidator>());


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
