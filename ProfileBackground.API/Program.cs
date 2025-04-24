using ProfileBackground.API.Swagger;
using ProfileBackground.Domain.Interfaces;
using ProfileBackground.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProfileService, ProfileService>();
builder.Services.AddHostedService<ProfileUpdateService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<DicionarioCustomSchemaFilter>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Profile API V1");
    c.RoutePrefix = string.Empty;
});
app.UseAuthorization();
app.MapControllers();
app.Run();