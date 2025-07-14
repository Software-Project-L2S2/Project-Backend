using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Services;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Text.Json.Serialization;
using EmployeeProfileAPI.Models.AuthModels;


var builder = WebApplication.CreateBuilder(args);

// --- 1. Service Configuration ---

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Logging, Email, Services
builder.Services.AddLogging();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Your React app's URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .WithMethods("GET", "POST", "PUT", "DELETE");// Keep if your frontend sends credentials
        });
});

// JWT Authentication
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

// Controllers + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This prevents circular reference issues when serializing related entities
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkForce API", Version = "v1" });

    // JWT Auth config for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\nExample: \"Bearer abc123...\""
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


// --- 2. Build the Application ---
var app = builder.Build();

// Seed admin data (ensure DataSeeder.cs exists and is correctly implemented)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataSeeder.SeedAdminAsync(services);
}


// --- 3. Configure the HTTP Request Pipeline (Middleware) ---
// The order of middleware registration is critical.

// ✅ Configure exception handling based on the environment
if (app.Environment.IsDevelopment())
{
    // In development, show detailed exception pages for easier debugging.
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkForce API V1");
    });
}
else
{
    // In production, use a generic exception handler and enable HSTS.
    app.UseExceptionHandler("/Error"); // You would need to create an Error handling page/endpoint
    app.UseHsts();
}

// ✅ Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// ✅ Serve static files from wwwroot (e.g., for images, CSS, JS).
app.UseStaticFiles();

// ✅ Configure and serve static files for the 'uploads' folder.
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath); 
app.UseStaticFiles(new StaticFileOptions
{
    // The physical path to the 'uploads' directory
    FileProvider = new PhysicalFileProvider(uploadsPath),
    // The URL path to access the files (e.g., /uploads/image.png)
    RequestPath = "/uploads" 
});

// ✅ Enable routing to determine which endpoint to execute.
app.UseRouting();

// ✅ Apply the CORS policy. Must be after UseRouting and before UseAuthorization.
app.UseCors("AllowFrontend");

// ✅ Enable Authentication. This middleware identifies the user based on the incoming token.
// It MUST come before UseAuthorization.
app.UseAuthentication();

// ✅ Enable Authorization. This middleware checks if the identified user has permission to access the endpoint.
app.UseAuthorization();

// ✅ Execute the endpoints (map controller routes). This is where your API controllers are matched.
app.MapControllers();

// ✅ Run the application.
app.Run();