using BlogApi.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Controllers + Force camelCase JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// ✅ Add CORS for React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Register Services
builder.Services.AddSingleton<BlogService>();
builder.Services.AddSingleton<SavedBlogService>(); // ✅ New saved blogs service
builder.Services.AddSingleton<UserService>(); // ✅ register user service

var app = builder.Build();

// ✅ Order matters
app.UseHttpsRedirection();
app.UseCors("AllowReact");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();