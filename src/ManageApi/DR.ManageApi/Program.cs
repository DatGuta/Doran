using System.Text;
using DR.Application;
using DR.Database;
using DR.Resource;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DrContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(DrContext))));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecret"]!))
    });

builder.Services.AddAuthorization(options => options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser().Build());

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DR Management Api v1", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = @"API KEY",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors();
builder.Services.AddMiddlewares();

builder.Services.AddHttpContextAccessor();

builder.Services.AddResources();
//builder.Services.AddServices();
//builder.Services.AddTelegramBot(builder.Configuration);
builder.Services.AddMediatR(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DR Management Api v1"));

app.UseHttpsRedirection();

app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*"));

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddlewares();

app.UseRouting();
//AutoMigrate(app);

app.Run();

//static void AutoMigrate(IApplicationBuilder app) {
//    using var scope = app.ApplicationServices.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<FmsContext>();
//    dbContext.Database.EnsureCreated();
//    dbContext.Database.Migrate();
//}
