using ClinicApi.Repositories;
using ClinicApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar conexão com PostgreSQL
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
    "Host=localhost;Database=clinic_db;Username=postgres;Password=postgres";

builder.Services.AddSingleton(connectionString);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here-make-it-long-enough")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Register repositories and services - TODAS AS TELAS
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IFinancialRepository, FinancialRepository>();

// Agenda Module - Complete 3-layer architecture with calendar and scheduling features
builder.Services.AddScoped<IAgendaRepository, AgendaRepository>();
builder.Services.AddScoped<IAgendaService, AgendaService>();

// Patient Module - Complete 3-layer architecture with analytics and segmentation
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

// Authentication Module - Complete user management and security
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Inventory Module - Complete 3-layer architecture with smart features and optimization
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();