// ---- START OF CORRECTED FILE ----

using HRWorkForceSystemBackend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HRWorkForceSystemBackend.Services;
using Microsoft.OpenApi.Models;
using HRWorkForceSystemBackend.Models.AuthModels;

var builder = WebApplication.CreateBuilder(args);

// 1. Register the database context ONCE with the CORRECT name.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddLogging();

// 2. Register your services.
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AnnouncementService>(); // Registered ONCE here.

// --- THIS IS THE LINE THAT WAS REMOVED ---
// builder.Services.AddDbContext<ApplicationDbContext>(options => ... ); // REMOVED - This was the error.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- THIS IS THE LINE THAT WAS REMOVED ---
// builder.Services.AddScoped<AnnouncementService>(); // REMOVED - This was a duplicate.

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGci...\""
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataSeeder.SeedAdminAsync(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkForce API V1");
    });
}

app.UseCors("AllowFrontend");

// You may or may not need this. If your frontend and backend are on different
// protocols (HTTP vs HTTPS) during development, it's okay to comment it out.
// app.UseHttpsRedirection(); 

app.MapGet("/ping", () => "Pong!");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// ---- END OF CORRECTED FILE ----