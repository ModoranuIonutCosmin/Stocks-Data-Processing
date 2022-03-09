using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksFinalSolution.BusinessLogic.StocksMarketSummaryGenerator;
using StocksProccesing.Relational;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using Hellang.Middleware.ProblemDetails;
using Stocks.General.Exceptions;
using StocksFinalSolution.BusinessLogic.Features.Authentication;
using StocksFinalSolution.BusinessLogic.Features.Portofolio;
using StocksFinalSolution.BusinessLogic.Features.Companies;
using StocksFinalSolution.BusinessLogic.Features.Stocks;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksFinalSolution.BusinessLogic.Services;
using StocksProccesing.Relational.Extension_Methods.DI;
using StocksProcessing.API.Middleware;
using StocksProcessing.General.Exceptions;

namespace StocksProcessing.API
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment hostingEnvironment;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StocksProcessing.API", Version = "v1" });
            });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            string connectionString = Configuration.GetConnectionString("MyConnection");

            if (hostingEnvironment.IsProduction())
            {
                connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? connectionString;
            }

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
                
                .AddTransient<IPredictionsDataService, PredictionsDataService>();
            
            
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
