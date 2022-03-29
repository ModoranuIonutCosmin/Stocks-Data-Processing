using System;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stocks.General.Exceptions;
using StocksFinalSolution.BusinessLogic.Features.Authentication;
using StocksFinalSolution.BusinessLogic.Features.Companies;
using StocksFinalSolution.BusinessLogic.Features.MyProfile;
using StocksFinalSolution.BusinessLogic.Features.Portofolio;
using StocksFinalSolution.BusinessLogic.Features.Predictions;
using StocksFinalSolution.BusinessLogic.Features.Stocks;
using StocksFinalSolution.BusinessLogic.Features.StocksMarketMetricsCalculator;
using StocksFinalSolution.BusinessLogic.Features.StocksMarketSummaryGenerator;
using StocksFinalSolution.BusinessLogic.Features.Transactions;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Extension_Methods.DI;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Middleware;
using StocksProcessing.General.Exceptions;

namespace StocksProcessing.API;

public class Startup
{
    private readonly IWebHostEnvironment hostingEnvironment;

    public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        Configuration = configuration;
        this.hostingEnvironment = hostingEnvironment;
    }

    public IConfiguration Configuration { get; }


    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo {Title = "StocksProcessing.API", Version = "v1"});
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

        var connectionString = Configuration.GetConnectionString("MyConnection");

        if (hostingEnvironment.IsProduction())
            connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? connectionString;

        services.AddDbContext<StocksMarketContext>(options =>
        {
            options.UseSqlServer(connectionString,
                ma =>
                    ma.MigrationsAssembly(typeof(StocksMarketContext).Assembly.FullName)
            );
            options.EnableSensitiveDataLogging();
        });


        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<StocksMarketContext>()
            .AddDefaultTokenProviders();


        services.AddAuthentication().AddJwtBearer(options =>

        {
            var jwtSecret = Environment.GetEnvironmentVariable("JwtSecret") ??
                            Configuration["Jwt:Secret"];

            options.TokenValidationParameters = new TokenValidationParameters
            {
                //Valideaza faptul ca payload-ul din Token a fost semnat cu secretul 
                //disponibil pe server si nu a fost modificat
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });


        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 3;

            options.SignIn.RequireConfirmedEmail = false;
        });

        services.Configure<DataProtectionTokenProviderOptions>(e =>
            e.TokenLifespan = TimeSpan.FromHours(3));


        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
        });

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddPersistence();
        services.AddEmailServices();

        services
            .AddScoped<IProfileService, ProfileService>()
            .AddScoped<IPortofolioService, PortofolioService>()
            .AddScoped<ICompanyService, CompanyService>()
            .AddScoped<IStocksService, StocksService>()
            .AddScoped<IUserAuthenticationService, UserAuthenticationService>()
            .AddScoped<IUserPasswordResetService, UserPasswordResetService>()
            .AddTransient<IStockMarketDisplayPriceCalculator, StockMarketDisplayPriceCalculator>()
            .AddTransient<IStockMarketOrderTaxesCalculator, StockMarketOrderTaxesCalculator>()
            .AddTransient<IStockMarketProfitCalculator, StockMarketProfitCalculator>()
            .AddTransient<IStocksTrendCalculator, StocksTrendCalculator>()
            .AddTransient<ITransactionSummaryCalculator, TransactionSummaryCalculator>()
            .AddTransient<IPricesDisparitySimulator, PricesDisparitySimulator>()
            .AddTransient<IStocksSummaryGenerator, StocksSummaryGenerator>()
            .AddTransient<IPredictionsDataService, PredictionsDataService>()
            .AddTransient<ITransactionsService, TransactionsService>();


        services.AddProblemDetails(options =>
        {
            options.Map<NullReferenceException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.BadRequest));
            options.Map<ArgumentException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.BadRequest));
            options.Map<NullReferenceException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.BadRequest));
            options.Map<InsufficientFundsException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.BadRequest));
            options.Map<InvalidLeverageValue>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.BadRequest));
            options.Map<InvalidStopLossValueForLeveragedTrade>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.BadRequest));
            options.Map<InvalidTakeProfitValue>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.BadRequest));

            options.Map<AuthenticationException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.Conflict));
            options.Map<InvalidTransactionException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.Conflict));
            options.Map<OrderAlreadySubmitted>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.Conflict));
            options.Map<StockMarketClosedException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.Conflict));
            options.Map<InvalidTransactionOwner>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.Conflict));


            options.Map<InvalidCompanyException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.NotFound));
            options.Map<InvalidConfirmationLinkException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.NotFound));
            options.Map<InvalidPasswordResetLink>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.NotFound));
            options.Map<InvalidTransactionException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.NotFound));
            options.Map<NoStockPricesRecordedException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.NotFound));
            options.Map<UserNotFoundException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.NotFound));

            options.Map<UnauthorizedAccessException>(details =>
                details.MapToProblemDetailsWithStatusCode(HttpStatusCode.Unauthorized));
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StocksProcessing.API v1"));
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.UseProblemDetails();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}