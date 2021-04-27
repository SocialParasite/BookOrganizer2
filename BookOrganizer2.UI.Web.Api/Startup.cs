using BookOrganizer2.DA.Repositories;
using BookOrganizer2.DA.Repositories.Lookups;
using BookOrganizer2.DA.SqlServer;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.DA;
using BookOrganizer2.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BookOrganizer2.UI.Web.Api
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
            var connString = Configuration.GetConnectionString("DEV");

            services.AddDbContext<BookOrganizer2DbContext>(options => options.UseSqlServer(connString));

            services.AddTransient<ISimpleDomainService<Language, LanguageId>, LanguageService>();
            services.AddTransient<IRepository<Language, LanguageId>, LanguageRepository>();
            services.AddTransient<ILanguageLookupDataService, LanguageLookupDataService>(ctx =>
            {
                return new LanguageLookupDataService(() => ctx.GetService<BookOrganizer2DbContext>());
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookOrganizer2.UI.Web.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookOrganizer2.UI.Web.Api v1"));
            }

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
