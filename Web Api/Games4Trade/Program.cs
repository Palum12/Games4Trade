using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Games4TradeAPI;
using Games4TradeAPI.Data;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Hubs;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;
using Games4TradeAPI.Models;
using Games4TradeAPI.Repositories;
using Games4TradeAPI.Services;
using Games4TradeAPI.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("ApplicationContext");

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAutoMapper(configuration => configuration.AddProfile<MappingProfile>());

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IStateService, StateService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ISystemRepository, SystemRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IRepository<Photo>, Repository<Photo>>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IAdvertisementReposiotry, AdvertisementRepository>();
builder.Services.AddScoped<IRepository<AdvertisementItem>, AdvertisementItemRepository>();
builder.Services.AddScoped<IRepository<Region>, Repository<Region>>();
builder.Services.AddScoped<IRepository<State>, Repository<State>>();

builder.Services.AddTransient<IValidator<UserRegisterDto>, UserRegisterDtoValidator>();
builder.Services.AddTransient<IValidator<UserRecoverDto>, UserRecoverDtoValidator>();
builder.Services.AddTransient<IValidator<AnnouncementSaveDto>, AnnoucementSaveValidator>();
builder.Services.AddTransient<IValidator<ObservedUsersRelationshipDto>, ObservedUsersRelationshipValidator>();
builder.Services.AddTransient<IValidator<AdvertisementSaveDto>, AdvertisementSaveValidator>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        ConfigureJwtBearer(options);
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR(opt => opt.EnableDetailedErrors = true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseRouting();

app.UseCors(policyBuilder => policyBuilder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessagesHub>("/messagehub", options => options.Transports = HttpTransportType.WebSockets);

await ApplyMigrationsAndSeedAsync(app.Services, app.Environment.IsDevelopment());

app.Run();

static void ConfigureJwtBearer(JwtBearerOptions options)
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7uUzzYky7Lxb4pkGLRzU77dxpazhWEr4")),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;
            var accessToken = context.Request.Query["access_token"];

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/messagehub"))
            {
                context.Token = accessToken.ToString();
            }

            return Task.CompletedTask;
        }
    };
}

static async Task ApplyMigrationsAndSeedAsync(IServiceProvider services, bool isDevelopment)
{
    const string developmentAdminLogin = "admin";
    const string developmentAdminEmail = "admin@games4trade.pl";
    const string developmentAdminPassword = "Admin123!";
    const string legacyDevelopmentAdminSalt = "fd97ee1734377936bf51ac4ada3d1763";

    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();

    await context.Database.MigrateAsync();

    if (isDevelopment && !await context.Users.AnyAsync(u => u.Login == developmentAdminLogin))
    {
        var seededAdmin = new User
        {
            Login = "admin",
            Email = "admin@games4trade.pl",
            Role = "Admin",
            Salt = "fd97ee1734377936bf51ac4ada3d1763",
            Password = "tCw/FKbLk78uwFEh120FVO7+lBd/p5ExJIeEpXgDiW0oI5dGgX1Mt+52Yd7wa/FGG7D0Awz+IXm1Hhibahb1DXT4SHlmwuLm1MOY5fvqoebnh1hVCZEZieK62Fk+6MLrJGn+3tdW90af8AuAZVTFRuQxft7XHBGA3Qop/qfsyNRrE064gQ17e2CSW2HYOzN/zHPFnwrj6JmdPgDtZiPxZyE7tLCYJ0nyPM6HLD01xd1rdS4rHqcn5EL5yEhsiYsIMR9g+6+XLR/IwpqmXW1beNf6t6m1wv8C6RltsB5j5rsfgcCapGLbW0TGmuyR0pC/HOdJ6o/1GQp2RRVS7GLyVA=="
        };

        seededAdmin.Login = developmentAdminLogin;
        seededAdmin.Email = developmentAdminEmail;
        seededAdmin.Salt = loginService.GetSalt();
        seededAdmin.Password = loginService.ComputeHash(seededAdmin.Salt, developmentAdminPassword);
        context.Users.Add(seededAdmin);

        await context.SaveChangesAsync();
    }

    if (isDevelopment)
    {
        var legacyAdmin = await context.Users.SingleOrDefaultAsync(u => u.Login == developmentAdminLogin);
        if (legacyAdmin?.Email == developmentAdminEmail && legacyAdmin.Salt == legacyDevelopmentAdminSalt)
        {
            legacyAdmin.Salt = loginService.GetSalt();
            legacyAdmin.Password = loginService.ComputeHash(legacyAdmin.Salt, developmentAdminPassword);
            await context.SaveChangesAsync();
        }
    }
}
