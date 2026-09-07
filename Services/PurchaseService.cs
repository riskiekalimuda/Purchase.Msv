using AutoMapper;
using MessageMQCommon.Respones;
using Purchase.Msv.DTOs;
using Purchase.Msv.Models;

namespace Purchase.Msv.Services
{
    public class PurchaseService
    {
        private readonly ILogger<PurchaseService> _logger;
        private readonly PurchaseMsvDbContext _dbContext;
        private readonly IMapper _mapper;
        public PurchaseService(ILogger<PurchaseService> logger, PurchaseMsvDbContext dbContext, IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ServiceResult<TrxPurchase>> CreatePurchaseAsync(CreatePurchaseRequest purchase)
        {
            try
            {
                if (purchase == null)
                {
                   // throw new ArgumentNullException(nameof(purchase), "Purchase data is required.");
                    return new ServiceResult<TrxPurchase>(false)
                    {
                        IsSuccess = false,
                        ErrorMessage = "Purchase data is required.",
                        ErrorCode = "PURCHASE_DATA_REQUIRED"
                    };  
                }
                var purchaseEntity = _mapper.Map<TrxPurchase>(purchase);    
                _dbContext.TrxPurchases.Add(purchaseEntity);
                await _dbContext.SaveChangesAsync();
                return new ServiceResult<TrxPurchase>(true)
                {
                    IsSuccess = true,
                    Data = purchaseEntity
                };  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a purchase.");
                return new ServiceResult<TrxPurchase>(false)
                {
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while creating the purchase.",
                    ErrorCode = "DATABASE_ERROR"
                };
            }
        }

    }
}
