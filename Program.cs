using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ProjectManager.Data;

using Service;
using Tables.Models;
using Microsoft.OpenApi;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Google.GenAI;
using AgentTools;


var builder = WebApplication.CreateBuilder(args);

// OpenAPI para swagger
builder.Services.AddOpenApi();

// Controllers
builder.Services.AddControllers();

var agent = new Client(
    apiKey: builder.Configuration["Gemini:ApiKey"]!
);


builder.Services.AddSingleton(agent);

builder.Services.AddScoped<ProjectTools>();
builder.Services.AddScoped<AgentToolDispatcher>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<ProjectsService>();
builder.Services.AddScoped<TokenServices>();
builder.Services.AddScoped<ProjectMembersService>();
builder.Services.AddScoped<ProjectSprintService>();
builder.Services.AddScoped<ProjectTaskService>();
builder.Services.AddScoped<ProjectInvitationsService>();


// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    const string schemeId = "Bearer";

    options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite apenas o token JWT"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemeId, document)] = []
        });
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

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["AppSettings:Token"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenKey!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
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