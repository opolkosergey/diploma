using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Core.Repositories;
using Diploma.Core.Repositories.Abstracts.Base;
using Diploma.Core.Services;
using Diploma.EmailSender;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;
using Diploma.Filters;
using Diploma.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Diploma.Services;
using Diploma.Options;

namespace Diploma
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("Configurations/EmailNotificatorConfiguration.json", optional: false, reloadOnChange: true)
                .AddJsonFile("Configurations/UsersConfiguration.json", optional: false, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionInterseptor));
                options.Filters.Add(new UserFoldersResultFilter()); 
                options.Filters.Add(new ResourceNotFoundFilter());
            });

            services.AddOptions();
            services.Configure<EmailSenderOptions>(Configuration.GetSection("NotificatorConfiguration"));
            services.Configure<UsersOptions>(Configuration.GetSection("UsersConfiguration"));

            services.AddTransient<IEmailNotificator, EmailNotificator>();
            services.AddTransient<AuditLogger>();

            services.AddTransient<DocumentService>();
            services.AddTransient<DocumentSignService>();
            services.AddTransient<OrganizationService>();
            services.AddTransient<SearchService>();
            services.AddTransient<SignatureRequestService>();
            services.AddTransient<UserTaskService>();
            services.AddTransient<UserService>();

            services.AddTransient<BaseRepository<Document>, DocumentRepository>();            
            services.AddTransient<BaseRepository<IncomingSignatureRequest>, SignatureRequestRepository>();
            services.AddTransient<BaseRepository<Organization>, OrganizationRepository>();
            services.AddTransient<BaseRepository<SignatureWarrant>, SignatureWarrantRepository>();
            services.AddTransient<BaseRepository<UserFolder>, UserFolderRepository>();
            services.AddTransient<BaseRepository<UserTask>, TaskRepository>();            

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();   
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();
            RolesInitializer.InitializeRoles(app.ApplicationServices).Wait();
            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
