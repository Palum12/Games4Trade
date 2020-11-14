using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Games4Trade.Data;
using Games4Trade.Dtos;
using Games4Trade.Hubs;
using Games4Trade.Interfaces.Repositories;
using Games4Trade.Interfaces.Services;
using Games4Trade.Repositories;
using Games4Trade.Services;
using Games4Trade.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Games4Trade
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
            services.AddCors();
            services.AddMvc().AddFluentValidation();
            services.AddSingleton(Configuration);

            var connectionString = Configuration.GetConnectionString("ApplicationContext");
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<IAnnouncementService, AnnouncementService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IStateService, StateService>();
            services.AddTransient<IValidator<UserRegisterDto>, UserRegisterDtoValidator> ();
            services.AddTransient<IValidator<UserRecoverDto>, UserRecoverDtoValidator> ();
            services.AddTransient<IValidator<AnnouncementSaveDto>, AnnoucementSaveValidator>();
            services.AddTransient<IValidator<ObservedUsersRelationshipDto>, ObservedUsersRelationshipValidator>();
            services.AddTransient<IValidator<AdvertisementSaveDto>, AdvertisementSaveValidator>();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7uUzzYky7Lxb4pkGLRzU77dxpazhWEr4")),
                        ValidateLifetime = true, //validate the expiration and not before values in the token
                        ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                    };

                    //options.Events = new JwtBearerEvents
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        // If the request is for our hub...
                    //        var path = context.HttpContext.Request.Path;
                    //        var accessToken = context.Request.Query["access_token"];

                    //        if (!string.IsNullOrEmpty(accessToken) &&
                    //            (path.StartsWithSegments("/messagehub")))
                    //        {
                    //            // Read the token out of the query string
                    //            context.Token = accessToken.ToString();
                    //        }
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // services.AddSignalR(opt => opt.EnableDetailedErrors = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseSignalR(route => {
            //    route.MapHub<MessagesHub>("/messagehub");
            //});
        }

       
    }
}
