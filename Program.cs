using Inventory;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:Audience"];

    // Discovery normally requires https; Keycloak is plain http locally.
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

    // Keep Keycloak's claim names ("sub", "preferred_username", "email")
    // instead of renaming them to long Microsoft claim-type URIs.
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,          // default
        ValidateAudience = true,        // default
        ValidateLifetime = true,        // default
        ValidateIssuerSigningKey = true,
        NameClaimType = "preferred_username", // makes User.Identity.Name work
        ClockSkew = TimeSpan.FromSeconds(30)  // tighter than the 5-minute default
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            ctx.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>()
                .LogWarning(ctx.Exception, "JWT rejected");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddDbContext<InventoryDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("InventoryDb"))
    .UseSnakeCaseNamingConvention()
);

builder.Services.AddHealthChecks()
    .AddNpgSql(sp => sp.GetRequiredService<IConfiguration>().GetConnectionString("InventoryDb")!
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
