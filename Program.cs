using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ProjectManager.Data;

using Service;
using Tables.Models;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Controllers
builder.Services.AddControllers();


builder.Services.AddScoped<ProjectsService>();
builder.Services.AddScoped<TokenServices>();
builder.Services.AddScoped<ProjectMembersService>();
builder.Services.AddScoped<ProjectInvitationsService>();


// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        
        Description = "Standar Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var tokenKey = builder.Configuration["AppSettings:Token"];
    if (string.IsNullOrEmpty(tokenKey))
    {
        throw new InvalidOperationException("JWT token key is not configured in AppSettings:Token");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});  // Verifica quem é o user
builder.Services.AddAuthorization(); // Verifica se tem a permissão para acessar a funcionalidade

var app = builder.Build();

app.UseCors("Frontend");

// Middleware de erros
app.UseMiddleware<ErrorHandlingMiddleware>();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();