using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DentalSpa.Infrastructure.Data;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Repositories;
using DentalSpa.Application.Services;
using DentalSpa.Application.Mappings;

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

// Configure multiple database connections
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                       builder.Configuration.GetConnectionString("DefaultConnection");
var sqlServerConnectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING") ?? 
                               builder.Configuration.GetConnectionString("SqlServerConnection");

// Add Entity Framework with PostgreSQL (Primary)
builder.Services.AddDbContext<DentalSpaDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Entity Framework with SQL Server (Secondary)
builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(sqlServerConnectionString ?? "Server=localhost;Database=DentalSpa_SqlServer;Trusted_Connection=true;TrustServerCertificate=true;"));

// ========== CAMADA DOMAIN - INTERFACES DE REPOSITÓRIO ==========
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IAuthRepository, DentalSpa.Infrastructure.Repositories.AuthRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IClientRepository, DentalSpa.Infrastructure.Repositories.ClientRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IAppointmentRepository, DentalSpa.Infrastructure.Repositories.AppointmentRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IServiceRepository, DentalSpa.Infrastructure.Repositories.ServiceRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IStaffRepository, DentalSpa.Infrastructure.Repositories.StaffRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IInventoryRepository, DentalSpa.Infrastructure.Repositories.InventoryRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IFinancialRepository, DentalSpa.Infrastructure.Repositories.FinancialRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IPackageRepository, DentalSpa.Infrastructure.Repositories.PackageRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IBeforeAfterRepository, DentalSpa.Infrastructure.Repositories.BeforeAfterRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IAgendaRepository, DentalSpa.Infrastructure.Repositories.AgendaRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IPatientRepository, DentalSpa.Infrastructure.Repositories.PatientRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.ILearningAreaRepository, DentalSpa.Infrastructure.Repositories.LearningAreaRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IClinicInfoRepository, DentalSpa.Infrastructure.Repositories.ClinicInfoRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.ISubscriptionRepository, DentalSpa.Infrastructure.Repositories.SubscriptionRepository>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IUserRepository, DentalSpa.Infrastructure.Repositories.UserRepository>();

// ========== CAMADA APPLICATION - SERVIÇOS DE APLICAÇÃO ==========
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IAuthService, DentalSpa.Application.Services.AuthService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IClientService, DentalSpa.Application.Services.ClientService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IAppointmentService, DentalSpa.Application.Services.AppointmentService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IAgendaService, DentalSpa.Application.Services.AgendaService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IServiceService, DentalSpa.Application.Services.ServiceService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IStaffService, DentalSpa.Application.Services.StaffService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IInventoryService, DentalSpa.Application.Services.InventoryService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IFinancialService, DentalSpa.Application.Services.FinancialService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IPackageService, DentalSpa.Application.Services.PackageService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IBeforeAfterService, DentalSpa.Application.Services.BeforeAfterService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IPatientService, DentalSpa.Application.Services.PatientService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.ILearningService, DentalSpa.Application.Services.LearningService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IClinicInfoService, DentalSpa.Application.Services.ClinicInfoService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.ISubscriptionService, DentalSpa.Application.Services.SubscriptionService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IUserService, DentalSpa.Application.Services.UserService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IDatabaseSelectorService, DentalSpa.Application.Services.DatabaseSelectorService>();
builder.Services.AddScoped<DentalSpa.Application.Interfaces.IOrcamentoService, DentalSpa.Application.Services.OrcamentoService>();
builder.Services.AddScoped<DentalSpa.Domain.Interfaces.IOrcamentoRepository, DentalSpa.Infrastructure.Repositories.OrcamentoRepository>();

// ========== AUTOMAPPER CONFIGURATION ==========
builder.Services.AddAutoMapper(typeof(DentalSpaMappingProfile));

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

builder.Services.AddSingleton<DentalSpa.Application.Services.EmailService>();

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
app.UseMiddleware<DentalSpa.Application.Services.ErrorHandlingMiddleware>();
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