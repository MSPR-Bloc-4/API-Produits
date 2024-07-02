using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Product_Api.Configuration;
using Product_Api.Repository;
using Product_Api.Repository.Interface;

namespace Product_Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FirebaseConfig>(_configuration.GetSection("FirebaseConfig"));
            var firebaseConfig = _configuration.GetSection("FirebaseConfig").Get<FirebaseConfig>();

            GoogleCredential credential;
            using (var stream = new FileStream(firebaseConfig.ServiceAccountPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream);
            }

            FirestoreDbBuilder builder = new FirestoreDbBuilder
            {
                ProjectId = firebaseConfig.ProjectId,
                DatabaseId = "product",
                Credential = credential
            };

            FirestoreDb db = builder.Build();
            
            services.AddSingleton(db);
            services.AddSingleton<IProductRepository, ProductRepository>();

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddAuthorization();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
        }
    }
}
