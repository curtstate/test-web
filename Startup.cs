using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace WebApiTestProject
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<MvcOptions>(options =>
      {
        options.Filters.Add(new RequireHttpsAttribute());
      });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      var fileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "clientapp", "dist"));
      var options = new RewriteOptions() { StaticFileProvider = fileProvider }
                    .AddRedirectToHttps();
                    //.AddRewrite(@"^/index.html", "/index.html", skipRemainingRules: true);


      app.UseRewriter(options);

      app.Use(async (context, next) =>
      {
        await next();
        if (context.Response.StatusCode == 404)
        {
          context.Request.Path = "/index.html";
          await next();
        }
      });

      app.UseDefaultFiles();
      app.UseStaticFiles(new StaticFileOptions()
      {
        FileProvider = fileProvider
      });
    }
  }
}
