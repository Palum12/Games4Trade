using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Games4TradeAPI.Data;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Hubs;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;
using Games4TradeAPI.Models;
using Games4TradeAPI.Repositories;
using Games4TradeAPI.Services;
using Games4TradeAPI.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Games4TradeAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().AddFluentValidation();
            services.AddSingleton(Configuration);

            var connectionString = Configuration.GetConnectionString("ApplicationContext");
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<IAnnouncementService, AnnouncementService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IStateService, StateService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<ISystemRepository, SystemRepository>();
            services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            services.AddScoped<IRepository<Photo>, Repository<Photo>>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IAdvertisementReposiotry, AdvertisementRepository>();
            services.AddScoped<IRepository<AdvertisementItem>, AdvertisementItemRepository>();
            services.AddScoped<IRepository<Region>, Repository<Region>>();
            services.AddScoped<IRepository<State>, Repository<State>>();

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
            .AddJwtBearer(ConfigureJwtBearer);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSignalR(opt => opt.EnableDetailedErrors = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationContext context)
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
                endpoints.MapHub<MessagesHub>("/messagehub", options => options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets);
            });

            context.Database.Migrate();

            if (env.EnvironmentName == "Development" && !context.Users.AnyAsync(u => u.Role == "Admin").Result)
            {
                context.Users.Add(new User
                {
                    Login = "admin",
                    Email = "admin@games4trade.pl",
                    Role = "Admin",
                    Salt = "fd97ee1734377936bf51ac4ada3d1763",
                    Password = "tCw/FKbLk78uwFEh120FVO7+lBd/p5ExJIeEpXgDiW0oI5dGgX1Mt+52Yd7wa/FGG7D0Awz+IXm1Hhibahb1DXT4SHlmwuLm1MOY5fvqoebnh1hVCZEZieK62Fk+6MLrJGn+3tdW90af8AuAZVTFRuQxft7XHBGA3Qop/qfsyNRrE064gQ17e2CSW2HYOzN/zHPFnwrj6JmdPgDtZiPxZyE7tLCYJ0nyPM6HLD01xd1rdS4rHqcn5EL5yEhsiYsIMR9g+6+XLR/IwpqmXW1beNf6t6m1wv8C6RltsB5j5rsfgcCapGLbW0TGmuyR0pC/HOdJ6o/1GQp2RRVS7GLyVA=="
                });
                context.SaveChanges();
            }                  
        }     

        private void ConfigureJwtBearer (JwtBearerOptions options)
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

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    var accessToken = context.Request.Query["access_token"];

                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/messagehub")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken.ToString();
                    }
                    return Task.CompletedTask;
                }
            };
        }
    }
}
