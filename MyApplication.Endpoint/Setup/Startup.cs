using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using MyApplication.Abstraction;
using MyApplication.Abstraction.Fixtures;
using MyApplication.Abstraction.Helpers;
using MyApplication.Business;
using MyApplication.Endpoint.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyApplication.Endpoint;

public static class Startup
{
    public static IServiceCollection RegisterApp(this IServiceCollection services)
    {
        services.RegisterCore();
        services.RegisterBusiness();
        services.AddAppServices();
        return services;
    }

    private static void AddAppServices(this IServiceCollection services)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddCors();
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        services.AddAntiforgery(options =>
        {
            options.Cookie.Name = "xsrf";
            options.Cookie.HttpOnly = false;
        });
        services.ConfigureApplicationCookie(options => { });
        services.Configure<IdentityOptions>(IdentityConfiguration.ConfigureOptions);
        var authChain = services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer(options => { options.TokenValidationParameters = TokenService.GetParameters(); });

        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
            c.TagActionsBy(api => new List<string> { api.GroupName });
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{ApplicationConstants.ApplicationName} API",
                Version = "v1",
                Description = $"{ApplicationConstants.ApplicationName} api explorer"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
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
                    new string[] { }
                }
            });
        });
    }

    public static void UseAppServices(this WebApplication app)
    {
        if (EnvironmentHelper.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseCors(c => c
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Lax,
            Secure = CookieSecurePolicy.SameAsRequest,
            CheckConsentNeeded = context => true
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApplication");
            c.RoutePrefix = ""; // Set Swagger UI at apps root
        });
    }
}