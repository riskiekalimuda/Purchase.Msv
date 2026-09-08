using MassTransit;
using MessageMQCommon.MQ.Messages.PurchaseMsv;
using Purchase.Msv.Services;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace Purchase.Msv.Consumers
{
    public class PurchaseCreatedResultConsumer:IConsumer<PurchaseResultMessage>
    {
        private readonly PurchaseService _purchaseService;
        private readonly ILogger<PurchaseCreatedResultConsumer> _logger;
        public PurchaseCreatedResultConsumer(PurchaseService purchaseService, ILogger<PurchaseCreatedResultConsumer> logger)
        {
            _purchaseService = purchaseService;
            _logger = logger;   
        }
        public async Task Consume(ConsumeContext<PurchaseResultMessage>context)
        {
            var purchaseCreatedResult = context.Message;
            try
            {
                var updateResult = await _purchaseService.UpdatePurchaseResult(purchaseCreatedResult);
                _logger.LogInformation($"PurchaseCreatedResultMessage: PurchaseNumber={purchaseCreatedResult.PurchaseNumber}, Result={purchaseCreatedResult.PurchaseResult}");

            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error processing PurchaseCreatedResultMessage");
            }
        }
    }
}
