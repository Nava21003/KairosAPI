using KairosAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization; // Necesario para ReferenceHandler

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---

builder.Services.AddCors();

// CORRECCIÓN PRINCIPAL AQUÍ:
// Usamos IgnoreCycles para que el JSON sea estándar y React lo entienda.
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Opcional: limpia nulos
});

// Configuración de la conexión a la Base de Datos
var connectionString = builder.Configuration.GetConnectionString("Kairos");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Configuración de JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
// Verificación de seguridad para evitar errores si falta la clave
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("SecretKey no configurada en appsettings.json.");
}
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


// 3. Configuración de API y SWAGGER

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KairosAPI", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    });
});

// Configuración Kestrel (Puertos)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5219); // HTTP
    options.ListenAnyIP(7219, listenOptions => { /* opcional TLS */ });
});


// --- CONSTRUCCIÓN DE LA APP ---
var app = builder.Build();


// --- PIPELINE DE MIDDLEWARES ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KairosAPI v1"));
}

app.UseHttpsRedirection();

// El CORS debe ir antes de Auth y Authorization para evitar bloqueos en pre-flight requests
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();