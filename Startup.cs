using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.PubSub.V1;
using Newtonsoft.Json.Linq;
using Product_Api.Helper;
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
            var projectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECTID") ?? JsonReader.GetFieldFromJsonFile("project_id");
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
            }

            FirestoreDbBuilder builder = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                DatabaseId = "product",
                Credential = credential
            };

            FirestoreDb db = builder.Build();

            services.AddSingleton(db);
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, "order-sub");
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
