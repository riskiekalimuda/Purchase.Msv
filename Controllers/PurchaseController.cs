using AutoMapper;
using MassTransit;
using MessageMQCommon.MQ.Messages.PurchaseMsv;
using MessageMQCommon.MQ.Names;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchase.Msv.DTOs;
using Purchase.Msv.Models;
using Purchase.Msv.Services;

namespace Purchase.Msv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly ILogger<PurchaseController> _logger;
        private readonly PurchaseService _purchaseService;
        private readonly PurchaseMsvDbContext _dbContext;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IMapper _mapper;
        public PurchaseController(ILogger<PurchaseController> logger, PurchaseService purchaseService,
            PurchaseMsvDbContext dbContext, ISendEndpointProvider sendEndpointProvider, IMapper mapper)
        {
            _logger = logger;
            _purchaseService = purchaseService;
            _dbContext = dbContext;
            _sendEndpointProvider = sendEndpointProvider;
            _mapper = mapper;
        }


        [HttpPost("CreatePurchase")]
        public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest purchase)
        {
            if(purchase == null)
            {
                return BadRequest("Purchase data is required.");
            }
            var purchaseResult = await _purchaseService.CreatePurchaseAsync(purchase);
            if (!purchaseResult.IsSuccess)
            {
                return BadRequest(purchaseResult.ErrorMessage);
            }

            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdStr) || string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("Unauthorized: User info not found in headers.");
            }

            if (!Guid.TryParse(userIdStr.ToString(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }
            try {
                var message = _mapper.Map<PurchaseMessage>(purchaseResult.Data);
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{QueueNames.PurchaseQueue.PurchaseCreatedQueue}"));
                await sendEndpoint.Send(message);
                await _dbContext.SaveChangesAsync();
                return Ok(new {PurchaseId= purchaseResult.Data.PurchaseNumber, Message= "Purchase created successfully."}); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending purchase created event.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }   
        }
    }
}
