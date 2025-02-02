using Minio.Helper;
using TasAndJet.Api;
using TasAndJet.Api.Clients;
using TasAndJet.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
//регистрация зависимостей 
builder.Services.AddProgramDependencies(builder.Configuration);
builder.Services.AddClients(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await app.AddMigrationAsync();

app.UseAuthentication();
app.UseAuthorization();


app.UseHttpsRedirection();

var port = "5001";
var urls = $"http://0.0.0.0:{port}";

app.Urls.Add(urls);
app.MapControllers();

app.Run();
