using PaymentService.Options;
using PaymentService.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("EpayOptions"));

var stripeOptions = builder.Configuration.GetSection("StripeOptions").Get<StripeOptions>()
    ?? throw new Exception("StripeOptions not found in configuration");

StripeConfiguration.ApiKey = stripeOptions.SecretKey;

builder.Services.AddSingleton<IStripeService, StripeService>();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
