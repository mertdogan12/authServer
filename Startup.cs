using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using authServer.Settings;
using MongoDB.Driver;
using authServer.Repositories;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

namespace authServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static string databaseName = "";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddAuthentication(options =>
                             {
                                 options.DefaultAuthenticateScheme = JwtAuthenticationDefaults.AuthenticationScheme;
                                 options.DefaultChallengeScheme = JwtAuthenticationDefaults.AuthenticationScheme;
                             })
                        .AddJwt(options =>
                             {
                                 options.Keys = new[] { "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk" };

                                 options.VerifySignature = true;
                             });

            services.AddSingleton<IMongoClient>(ServiceProvider =>
            {
                databaseName = settings.Database;
                return new MongoClient(settings.connectionString);
            });

            services.AddSingleton<IUserRepository, MongoDbUserRepository>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "authServer", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "authServer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}