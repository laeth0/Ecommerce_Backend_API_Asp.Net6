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



            builder.Services.AddDbContext<EcommerceContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });



            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));




            //builder.Services.AddAutoMapper(typeof(ProductProfile)); // this line is the same as the next line (we can use it if we have only one profile)
            builder.Services.AddAutoMapper(m=> m.AddProfile<MappingProfile>() );



            // add services of the logging
            builder.Services.AddSingleton<ILogging, Logging>();




            // add services of the customize response
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = endPointContext =>
                {

                    var errors= endPointContext.ModelState.Where(p=>p.Value.Errors.Count>0)
                        .SelectMany(ArrayOfErrors => ArrayOfErrors.Value.Errors)
                        .Select(ErrorTXT=> ErrorTXT.ErrorMessage).ToArray();

                    var errorsResponse = new ErrorResponse()
                    {
                        Errors= errors
                    };


                    return new BadRequestObjectResult(errorsResponse);
                };
            });
            // InvalidModelStateResponseFactory => this is the method that will be called when the model is not valid mean when called the method in wrong way like send string insted of id
            // endPointContext => this is the context of the request



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            // the order is important mean i should write UseAuthentication hen UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();
             

            app.MapControllers();

            app.Run();
        }
    }
}
