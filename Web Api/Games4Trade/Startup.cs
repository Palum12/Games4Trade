using Games4Trade.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Games4Trade.Data;
using Games4Trade.Repositories;
using Games4Trade.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.EntityFrameworkCore;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;

namespace Games4Trade
{
    public class Startup
    {
        private readonly AsyncLocal<Scope> scopeProvider = new AsyncLocal<Scope>();
        private IKernel Kernel { get; set; }

        private object Resolve(Type type) => Kernel.Get(type);
        private object RequestScope(IContext context) => scopeProvider.Value;

        private sealed class Scope : DisposableObject { }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton(Configuration);

            var connectionString = Configuration.GetConnectionString("ApplicationContext");
            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IApplicationContext, ApplicationContext>();
            services.AddCors(options =>
            {
                options.AddPolicy("AnyOrigin", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRequestScopingMiddleware(() => scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation(Resolve);
            services.AddCustomViewComponentActivation(Resolve);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.Kernel = this.RegisterApplicationComponents(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseCors("AnyOrigin");

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private IKernel RegisterApplicationComponents(IApplicationBuilder app)
        {
            // IKernelConfiguration config = new KernelConfiguration();
            var kernel = new StandardKernel();

            // Register application services
            foreach (var ctrlType in app.GetControllerTypes())
            {
                kernel.Bind(ctrlType).ToSelf().InScope(RequestScope);
            }

            // This is where our bindings are configurated
            kernel.Bind<IApplicationContext>().To<ApplicationContext>().InScope(RequestScope);
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InScope(RequestScope);
            kernel.Bind<IUserService>().To<UserService>().InScope(RequestScope);
            

            // Cross-wire required framework services
            kernel.BindToMethod(app.GetRequestService<IViewBufferScope>);

            return kernel;
        }

    }

    public sealed class RequestScopingStartupFilter : IStartupFilter
    {
        private readonly Func<IDisposable> requestScopeProvider;

        public RequestScopingStartupFilter(Func<IDisposable> requestScopeProvider)
        {
            if (requestScopeProvider == null)
            {
                throw new ArgumentNullException(nameof(requestScopeProvider));
            }

            this.requestScopeProvider = requestScopeProvider;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> nextFilter)
        {
            return builder =>
            {
                ConfigureRequestScoping(builder);

                nextFilter(builder);
            };
        }

        private void ConfigureRequestScoping(IApplicationBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                using (var scope = this.requestScopeProvider())
                {
                    await next();
                }
            });
        }
    }

   
}
