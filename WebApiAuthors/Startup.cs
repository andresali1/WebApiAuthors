using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAuthors.Filters;
using WebApiAuthors.Middlewares;
using WebApiAuthors.Services;
using WebApiAuthors.Utilities;

[assembly: ApiConventionType(typeof(DefaultApiConventions))] //To manage response type conventions in all the endpoints in the application
namespace WebApiAuthors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
                options.Conventions.Add(new SwaggerGroupByVersion());
            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
              .AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JWTkey"])
                    ),
                    ClockSkew = TimeSpan.Zero
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "WebApiAuthors",
                    Version = "v1",
                    Description = "Éste es un WebApi para trabajar con autores y libros",
                    Contact = new OpenApiContact
                    {
                        Email = "andresali1@hotmail.es",
                        Name = "Andrés Salinas"
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiAuthors", Version = "v2" });
                c.OperationFilter<AddHATEOASParameter>();
                c.OperationFilter<AddXVersionParameter>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

                var xmlArchive = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlRoute = Path.Combine(AppContext.BaseDirectory, xmlArchive);
                c.IncludeXmlComments(xmlRoute);
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim("isAdmin"));
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader()
                        .WithExposedHeaders(new string[] { "totalAmountData" });
                });
            });

            services.AddTransient<LinksGenerator>();
            services.AddTransient<HATEOASAuthorFilterAttribute>();
            services.AddSingleton < IActionContextAccessor, ActionContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseResponseHttpLog();          

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAuthors v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAuthors v2");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
