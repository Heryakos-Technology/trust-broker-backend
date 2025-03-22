using System;

using broker.Data;
using broker.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ImageUploader.Handler;
using Azure.Storage.Blobs;
using AzureBlob.Api.Logics;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
namespace broker_service
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

            // services.AddDbContext<DataContext>(opt => opt.UseSqlServer(
            //     Configuration.GetConnectionString("brokerConnection"),
            //      o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)

            //     ));
     var sqlConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL")  ?? Configuration.GetConnectionString("PostgreSqlConnectionString");
  
            services.AddDbContext<DataContext>(options => 
            {
                options.UseNpgsql(sqlConnectionString)
                       .EnableSensitiveDataLogging()  // Enable sensitive data logging
                       .LogTo(Console.WriteLine, LogLevel.Information);
            });

            services.AddControllers();
            services.AddHealthChecks();
            //    var appSettingsSection = Configuration.GetSection("AppSettings");
            //     services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            // var appSettings = appSettingsSection.Get<AppSettings>();


            var key = Encoding.ASCII.GetBytes("THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddControllersWithViews()

           .AddNewtonsoftJson(options =>
           options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors(option =>
            {
                option.AddPolicy("allowedOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                    );
            });
            services.AddSwaggerGen(c =>
            {
                // c.SwaggerDoc("v1", new OpenApiInfo { Title = "broker_service", Version = "v1" });
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "broker_service", Description = "Swagger Core API" });
            });


            services.AddTransient<IImageHandler, ImageHandler>();
            services.AddTransient<ImageWriter.Interface.IImageWriter,
                                  ImageWriter.Classes.ImageWriter>();

            services.AddScoped<IRepository<Broker>, BrokerRepository>();
            services.AddScoped(_ =>
            {
                return new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage"));
            });

            services.AddScoped<IFileManagerLogic, FileManagerLogic>();
            services.AddScoped<IRepository<Broker>, BrokerRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Category>, CategoryRepository>();
            services.AddScoped<IRepository<Skills>, SkillsRepository>();
            services.AddScoped<IRepository<Portfolio>, PortfolioRepository>();
            services.AddScoped<IRepository<Delivery>, DeliveryRepository>();
            services.AddScoped<IRepository<Deals>, DealsRepository>();
            services.AddScoped<IRepository<Review>, ViewRepository>();
            services.AddScoped<IRepository<Customer>, CustomerRepository>();
            services.AddScoped<IRepository<Sales>, SalesRepository>();
            services.AddScoped<IRepository<Buy>, BuyRepository>();
            services.AddScoped<IRepository<City>, CityRepository>();
            services.AddScoped<IRepository<Saving>, SavingRepository>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            using (var connection = context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tableName = reader.GetString(0);
                            Console.WriteLine($"Table: {tableName}");
                            using (var sampleCommand = connection.CreateCommand())
                            {
                                sampleCommand.CommandText = $"SELECT * FROM {tableName} LIMIT 1";
                                using (var sampleReader = sampleCommand.ExecuteReader())
                                {
                                    if (sampleReader.Read())
                                    {
                                        var values = new List<string>();
                                        for (int i = 0; i < sampleReader.FieldCount; i++)
                                        {
                                            values.Add(sampleReader[i].ToString());
                                        }
                                        Console.WriteLine($"Sample row: {string.Join(", ", values)}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("No data in this table");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error logging database tables: {ex.Message}");
    }
});
            // using (var scope = app.ApplicationServices.CreateScope())
            // {
            //     var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            //     context.Database.Migrate();
            // }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "broker_service v1"));
            }
            app.UseStaticFiles();
            // app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHealthChecks("/health");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
