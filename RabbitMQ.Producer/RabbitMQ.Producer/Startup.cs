using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace RabbitMQ.Producer
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
            var rabbitConfig = Configuration.GetSection("rabbit");
            services.Configure<RabbitOptions>(rabbitConfig);

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();

            services.AddSingleton<IRabbitManager, RabbitManager>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("1.0.0", new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "RabbitMQ demo",
                    Description = "RabbitMQ demo API (ASP.NET Core 3.0)",
                    Contact = new OpenApiContact()
                    {
                        //Name = "Swagger Codegen Contributors",
                        //Url = new Uri("https://github.com/swagger-api/swagger-codegen"),
                        Email = ""
                    },
                    //TermsOfService = new Uri("")
                });
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //TODO: Either use the SwaggerGen generated Swagger contract (generated from C# classes)
                c.SwaggerEndpoint("/swagger/1.0.0/swagger.json", "Connected Provisioning APIs");
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
