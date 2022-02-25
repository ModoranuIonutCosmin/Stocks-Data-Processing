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
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Email;
using StocksProcessing.API.Email.Interfaces;
using System;
using System.Reflection;
using System.Text;

namespace StocksProcessing.API
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

            services.AddDbContext<StocksMarketContext>(options =>
                options.UseSqlServer(DatabaseSettings.ConnectionString,
                    ma => 
                        ma.MigrationsAssembly(typeof(StocksMarketContext).Assembly.FullName)
                ));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<StocksMarketContext>()
                    .AddDefaultTokenProviders();


            services.AddAuthentication().AddJwtBearer(options =>

            {
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
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]))
                };
            });


            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 3;

                options.SignIn.RequireConfirmedEmail = true;
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

            services.AddTransient<IDirectEmailSender, DirectEmailSender>()
                .AddTransient<ITemplatedEmailSender, TemplatedEmailSender>()
                .AddTransient<IGeneralPurposeEmailService, GeneralPurposeEmailService>()
                .AddTransient<IUsersRepository, UsersRepository>()
                .AddTransient<IOrdersRepository, OrdersRepository>()
                .AddTransient<ITransactionsRepository, TransactionsRepository>()
                .AddTransient<IStockPricesRepository, StockPricesRepository>()
                .AddTransient<ICompaniesRepository, CompaniesRepository>()
                .AddTransient<IStockSummariesRepository, StockSummariesRepository>()
                .AddTransient<IStockMarketDisplayPriceCalculator, StockMarketDisplayPriceCalculator>()
                .AddTransient<IStockMarketOrderTaxesCalculator, StockMarketOrderTaxesCalculator>()
                .AddTransient<IPricesDisparitySimulator, PricesDisparitySimulator>()
                .AddTransient<IStocksSummaryGenerator, StocksSummaryGenerator>()
                .AddTransient<IStockMarketProfitCalculator, StockMarketProfitCalculator>()
                .AddTransient<IStocksTrendCalculator, StocksTrendCalculator>()
                .AddTransient<ITransactionSummaryCalculator, TransactionSummaryCalculator>();
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



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
