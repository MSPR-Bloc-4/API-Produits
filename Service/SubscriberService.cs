using Google.Cloud.PubSub.V1;
using Product_Api.Service.Interface;

namespace Product_Api.Service
{
    public class SubscriberService : BackgroundService
    {
        private readonly ILogger<SubscriberService> _logger;
        private readonly IProductService _productService;
        private readonly SubscriberClient _subscriberClient;

        public SubscriberService(ILogger<SubscriberService> logger, IProductService productService, SubscriberClient subscriberClient)
        {
            _logger = logger;
            _productService = productService;
            _subscriberClient = subscriberClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _subscriberClient.StartAsync(async (msg, token) =>
            {
                try
                {
                    if (msg.Attributes.ContainsKey("StockIncrement"))
                    {
                        var productIds = msg.Attributes["StockIncrement"].Split(',');
                        await _productService.IncrementStock(new List<string>(productIds));
                    }
                    else if (msg.Attributes.ContainsKey("StockDecrement"))
                    {
                        var productIds = msg.Attributes["StockDecrement"].Split(',');
                        await _productService.DecrementStock(new List<string>(productIds));
                    }

                    _logger.LogInformation($"Received message {msg.MessageId}: {msg.Data.ToStringUtf8()}");
                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Pub/Sub message");
                    return SubscriberClient.Reply.Nack;
                }
            });
        }

        public override async Task StopAsync(CancellationToken stoppingToken) =>
            await _subscriberClient.StopAsync(stoppingToken);
    }
}