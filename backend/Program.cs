using BlogApi.Services;
using BlogApi.Helpers;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

// 🔥 Load .env variables
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ================= CONTROLLERS ================= //
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// ================= CORS ================= //
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ================= JWT CONFIG ================= //
var jwtSettings = builder.Configuration.GetSection("Jwt");

var keyString = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrWhiteSpace(keyString) ||
    string.IsNullOrWhiteSpace(issuer) ||
    string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException("JWT configuration missing");
}

var key = Encoding.ASCII.GetBytes(keyString);

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
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero
    };
});

// ================= SWAGGER ================= //
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================= SMTP CONFIG (.env) ================= //

var smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL");
var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

if (string.IsNullOrWhiteSpace(smtpEmail) ||
    string.IsNullOrWhiteSpace(smtpPassword))
{
    throw new InvalidOperationException("SMTP credentials missing in .env file");
}

// ================= SERVICES ================= //

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddSingleton<BlogService>();
builder.Services.AddSingleton<SavedBlogService>();

builder.Services.AddScoped<JwtHelper>();

// ================= BUILD APP ================= //
var app = builder.Build();

// ================= MIDDLEWARE ================= //

app.UseCors("AllowReact");

// 🔥🔥 ADD THIS LINE (VERY IMPORTANT)
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// 🔥🔥 CHANGE HERE (Swagger DISABLED)
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.MapControllers();

app.Run();