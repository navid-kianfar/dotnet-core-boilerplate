using MyApplication.Abstraction;
using MyApplication.Abstraction.Helpers;
using MyApplication.Business;

namespace MyApplication.Endpoint.Setup;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        EnvironmentHelper.Configure();
        services.RegisterCore();
        services.RegisterBusiness();
        services.AddCors();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (EnvironmentHelper.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseCors(c => c
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        app.UseRouting();
        // app.MapControllers();
    }
}