using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DentalSpa.Infrastructure.Data;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Repositories;
using DentalSpa.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure connection string from environment variable
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                       builder.Configuration.GetConnectionString("DefaultConnection");

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "DentalSpa API - Arquitetura DDD", 
        Version = "v1",
        Description = "API completa para gestão de clínica odontológica e estética com arquitetura Domain-Driven Design (DDD). Inclui: Autenticação, Clientes, Agendamentos, Serviços, Funcionários, Estoque, Financeiro, Pacotes, Antes/Depois, Área de Aprendizado e Informações da Clínica."
    });
    
    // Configure JWT authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<DentalSpaDbContext>(options =>
    options.UseNpgsql(connectionString));

// ========== CAMADA DOMAIN - INTERFACES DE REPOSITÓRIO ==========
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IFinancialTransactionRepository, FinancialTransactionRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IClientPackageRepository, ClientPackageRepository>();
builder.Services.AddScoped<IBeforeAfterRepository, BeforeAfterRepository>();
builder.Services.AddScoped<ILearningAreaRepository, LearningAreaRepository>();
builder.Services.AddScoped<IClinicInfoRepository, ClinicInfoRepository>();

// ========== CAMADA APPLICATION - SERVIÇOS DE APLICAÇÃO ==========
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "DentalSpa-Default-Secret-Key-For-JWT-Token-Generation-2024";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DentalSpa API v1");
        c.RoutePrefix = "api-docs"; // Para manter compatibilidade
        c.DocumentTitle = "DentalSpa API - Documentação";
    });
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configurar para rodar na porta 5000
app.Urls.Add("http://0.0.0.0:5000");

// Ensure database is created and initialize default data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DentalSpaDbContext>();
    
    // Create database if it doesn't exist
    await context.Database.EnsureCreatedAsync();
    
    // Seed default admin user if no users exist
    if (!await context.Users.AnyAsync())
    {
        var adminUser = new DentalSpa.Domain.Entities.User
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FullName = "Administrador",
            Email = "admin@dentalspa.com",
            Role = "admin",
            CreatedAt = DateTime.UtcNow
        };
        
        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
        
        Console.WriteLine("👤 Usuário administrador padrão criado:");
        Console.WriteLine("   Username: admin");
        Console.WriteLine("   Password: admin123");
    }
}

Console.WriteLine("🦷 DentalSpa API iniciada com arquitetura DDD!");
Console.WriteLine("🏗️  Camadas implementadas: Domain, Application, Infrastructure, Service");
Console.WriteLine("📚 Swagger UI disponível em: http://localhost:5000/api-docs");
Console.WriteLine("🔐 Autenticação JWT configurada");
Console.WriteLine("🗄️  PostgreSQL conectado");

app.Run();