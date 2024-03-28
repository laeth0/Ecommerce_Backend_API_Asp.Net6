using Microsoft.EntityFrameworkCore;
using Ecommerce.Repository.Data;
using Ecommerce.Core.IGenericRepository;
using Ecommerce.Repository.GenericRepository;
using Ecommerce.PL.MappingProfiles;
using Serilog;
using Ecommerce.Core.ILog;
using Ecommerce.Repository.Log;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.PL.CustomizeResponses;
using Microsoft.AspNetCore.Identity;
using Ecommerce.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Any;

namespace Ecommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Serilog Configuration
            //// create the Configuration of Serilog (we here just create the configuration )
            //Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            //    .WriteTo.File("Log/EcommerceLog.txt", rollingInterval: RollingInterval.Infinite).CreateLogger();

            //// Add Serilog configuration to the container.(we here use the configuration)
            //builder.Host.UseSerilog();
            #endregion

            // Add services to the container.
            builder.Services.AddControllers(option =>
            {
                option.CacheProfiles.Add("500SecondsDuration", new CacheProfile
                {
                    Duration = 500,
                    Location = ResponseCacheLocation.Client,
                });
            });


            // Add services of the database 
            string ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<EcommerceContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });


            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));



            //builder.Services.AddAutoMapper(typeof(ProductProfile)); // this line is the same as the next line (we can use it if we have only one profile)
            builder.Services.AddAutoMapper(m => m.AddProfile<MappingProfile>());



            // add services of the logging
            builder.Services.AddSingleton<ILogging, Logging>();




            // add services of the customize response
            //builder.Services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = endPointContext =>
            //    {

            //        var errors = endPointContext.ModelState.Where(p => p.Value.Errors.Count > 0)
            //            .SelectMany(ArrayOfErrors => ArrayOfErrors.Value.Errors)
            //            .Select(ErrorTXT => ErrorTXT.ErrorMessage).ToList();

            //        var errorsResponse = new ErrorResponse()
            //        {
            //            Errors = errors
            //        };


            //        return new BadRequestObjectResult(errorsResponse);
            //    };
            //});
            // InvalidModelStateResponseFactory => this is the method that will be called when the model is not valid mean when called the method in wrong way like send string insted of id
            // endPointContext => this is the context of the request



            // Enable CORS
            // see this vedio for more information about CORS => https://www.youtube.com/watch?v=_YXcxm-O06I&list=PL62tSREI9C-c_yZ0a7Yui1U22Tv4mBjSF&index=6  
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });




            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<EcommerceContext>();


            builder.Services.AddAuthentication(Authentication_Options =>
            {
                Authentication_Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Authentication_Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(b =>
            {
                b.RequireHttpsMetadata = false;
                b.SaveToken = true;
                b.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:SecretKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            // Configer Swagger UI => Add Options/Authorization Token With Swagger
            builder.Services.AddSwaggerGen(SwaggerGenOptions =>
            {
                // to modify the ui like adding contact information and version
                SwaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Ecommerce",
                    Description = "Ecommerce API",
                    TermsOfService = new Uri("https://wa.me/972569768777"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Laeth Raed Nueirat",
                        Email = "laethraed0@gmail.com",
                    }
                });

                // to add the security to the swagger => for endpoint that need the token
                SwaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                });

                // to add the security to the swagger => we add this for endpoint that dont need the token => for dont send the token with the request that dont need it
                SwaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name="Bearer",
                            In= ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

            });


            var app = builder.Build();


            app.MapGet("/", () => "Welcome to Our Electronic Ecommerce");


            app.UseStaticFiles();



            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}




            // the order is important mean i should write UseAuthentication hen UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
