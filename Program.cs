using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseInMemoryDatabase("MinimalApiDemo");
});

builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<PasswordHasher>();

builder.Services.AddJwt(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddCors();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.TagActionsBy(d =>
    {
        var rootSegment = d.RelativePath?
            .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? "Home";
        return new List<string> { rootSegment! };
    });
});
builder.Services.AddEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization(); 

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapEndpoints();

app.Run();
