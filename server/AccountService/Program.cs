using AccountService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using AspNetCore.Identity.CosmosDb.Extensions;
using System.Text;
using MassTransit;
using Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
});
;


builder.Services.AddDbContext<AppDbContext>(options =>
 options.UseCosmos(builder.Configuration.GetConnectionString("DefaultConnection"), databaseName: builder.Configuration["DatabaseName"]));

builder.Services.AddCosmosIdentity<AppDbContext, User, IdentityRole, string>(
      options =>
      {
          //options.SignIn.RequireConfirmedAccount = true; // Always a good idea :)
          options.SignIn.RequireConfirmedAccount = false;

          options.Password.RequiredLength = 6;
          options.Password.RequireNonAlphanumeric = false;
          options.Password.RequireDigit = false;
          options.Password.RequireUppercase = false;
          options.User.RequireUniqueEmail = true;
          //options.SignIn.RequireConfirmedEmail = true;
      }
).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

/*builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"] ?? string.Empty;
    options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? string.Empty;
});*/



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? ""))
    };

});

builder.Services.AddMassTransit(x =>
{
    //x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    //{
    //    o.QueryDelay = TimeSpan.FromSeconds(10);
    //    o.use();
    //    o.UseBusOutbox();
    //});
    x.AddConsumersFromNamespaceContaining<AccountCreated>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("account", false));

    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration["AzureServiceBusConnectionString"]);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")

                .AllowAnyHeader()

                .WithMethods("GET", "POST", "PATCH", "PUT", "DELETE")

                .SetIsOriginAllowed((host) => true)

                .AllowCredentials();
        });
});


var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}catch(Exception e)
{
    Console.WriteLine("Error setting up db");
}

app.Run();
