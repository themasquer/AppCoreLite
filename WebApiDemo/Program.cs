using AppCoreLite.Models;
using AppCoreLite.Services;
using AppCoreLite.Utilities;
using DataAccessDemo.Contexts;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Culture
var cultureUtil = new CultureUtil();
builder.Services.Configure(cultureUtil.AddCulture());
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
#endregion

#region JWT
var jwtUtil = new JwtUtil(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtOptions.Issuer,
        ValidAudience = JwtOptions.Audience,
        IssuerSigningKey = jwtUtil.CreateSecurityKey(JwtOptions.SecurityKey)
    };
});
#endregion

// Add services to the container.
#region IoC Container
var connectionString = builder.Configuration.GetConnectionString("Db");
builder.Services.AddDbContext<Db>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<UnitDb>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<AccountService, UserAccountService>();
builder.Services.AddScoped<ProductServiceBase, ProductService>();
builder.Services.AddScoped<CategoryServiceBase, CategoryService>();
builder.Services.AddScoped<StoreServiceBase, StoreService>();
builder.Services.AddScoped<TreeNodeService, UnitTreeService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
#endregion

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
}).AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "vDemo",
        Title = "AppCoreLite WebApi",
        Description = "A Web API for E-Trade AppCoreLite using models",
        TermsOfService = null,
        Contact = new OpenApiContact
        {
            Name = "Cagil Alsac",
            Email = null,
            Url = null
        },
        License = new OpenApiLicense
        {
            Name = "Free License",
            Url = null
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1234abcde\""
    });
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
                new string[] {}
        }
    });
});
#endregion

var app = builder.Build();

#region Culture
app.UseRequestLocalization(cultureUtil.UseCulture());
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    #region Swagger
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppCoreLite WebApi vDemo"));
    #endregion
}

#region Web Service Testing
app.UseStaticFiles();
#endregion

app.UseHttpsRedirection();

#region CORS
app.UseCors();
#endregion

#region JWT
app.UseAuthentication();
#endregion

app.UseAuthorization();

app.MapControllers();

app.Run();
