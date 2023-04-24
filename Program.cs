using System.Text;
using LearnJwtAuth.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LearnJwtAuth.Util;
using LearnJwtAuth.Services;
using LearnJwtAuth.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // use Npgsql
    options.UseNpgsql(builder.Configuration.GetConnectionString("BlogsDB"));
});

// NOTE: menambahkan autentikasi jwt pada swagger page
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // INI HARUS TRUEEEEEE BIAR DI VALIDASI EXPIRATION TIME NYA NAELLLL
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMIN", policy => policy.RequireRole("ROLE_ADMIN"));
    options.AddPolicy("USER", policy => policy.RequireRole("ROLE_USER"));
});

builder.Services.AddCors(options => options.AddPolicy(name: "AddAllOrigins",
    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
));

// mendaftarkan service ke injection container
builder.Services.AddScoped<IImageUtil, ImageUtil>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IUserService, UserService>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // untuk mencegah npgsql legacy error

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AddAllOrigins");

app.UseHttpsRedirection();

app.UseAuthentication(); // untuk mengesahkan authentikasi
app.UseAuthorization();

app.MapControllers();

app.Run();
