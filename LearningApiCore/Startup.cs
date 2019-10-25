namespace LearningApiCore
{
    using LearningApiCore.Controllers;
    using LearningApiCore.DataAccess;
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Infrastructure;
    using LearningApiCore.Infrastructure.Errors;
    using LearningApiCore.Infrastructure.Security;
    using LearningApiCore.Interfaces;
    using LearningApiCore.Repositories;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

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
            //register db context service
            services.AddDbContext<AppDataContext>();

            //add Identity service 
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDataContext>().AddDefaultTokenProviders();


            //configure identity options
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 6; //password shouled be minimum 8 character length
                options.Password.RequireDigit = false; //password require digits
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = null;
            });

            //add cors service
            services.AddCors();

            //add Jwt service
            services.AddJwt();

            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(ValidatorActionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Dependecy Injection
            services.AddScoped<IJwtTokenCreator, JwtTokenCreator>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<IHomeRepository, HomeRepository>();

            //services.AddScoped<IPathProvider, PathProvider>();
            
            //add cache
            services.AddResponseCaching();

            //Add im memory cache
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AppDataContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //use cache
            app.UseResponseCaching();

            app.UseAuthentication();

            //error handling
            app.UseMiddleware<ErrorHandling>();

            //configure cors to http request pipeline
            app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            //configure Mvc to request pipeline
            app.UseMvc();

            app.UseHttpsRedirection();

            //ensure database created
            dbContext.Database.EnsureCreated();
        }
    }
}
