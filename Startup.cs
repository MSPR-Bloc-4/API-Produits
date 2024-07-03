using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.PubSub.V1;
using Product_Api.Configuration;
using Product_Api.Repository;
using Product_Api.Repository.Interface;
using Product_Api.Service;
using Product_Api.Service.Interface;

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
            GoogleCredential credential;
            if (Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS") != null)
            {
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS"))))
                {
                    credential = GoogleCredential.FromStream(stream);
                }
            }
            else
            {
                using (var stream = new FileStream("firebase_credentials.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream);
                }
            }            // Configuration setup
            services.Configure<FirebaseConfig>(_configuration.GetSection("FirebaseConfig"));
            var firebaseConfig = _configuration.GetSection("FirebaseConfig").Get<FirebaseConfig>();

            FirestoreDbBuilder builder = new FirestoreDbBuilder
            {
                ProjectId = firebaseConfig.ProjectId,
                DatabaseId = "product",
                Credential = credential
            };

            FirestoreDb db = builder.Build();

            services.AddSingleton(db);
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(firebaseConfig.ProjectId, "order-sub");
            services.AddSubscriberClient(builder =>
            {
                builder.SubscriptionName = subscriptionName;
                builder.Credential = credential;
            });
            services.AddHostedService<SubscriberService>();

            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IProductService, ProductService>();

            // Register ASP.NET Core services
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
