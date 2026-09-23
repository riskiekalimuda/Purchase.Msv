using AutoMapper;
using MassTransit;
using MessageMQCommon.MQ.Messages.PurchaseMsv;
using MessageMQCommon.MQ.Names;
using MessageMQCommon.Respones;
using Microsoft.EntityFrameworkCore;
using Purchase.Msv.DTOs;
using Purchase.Msv.Models;

namespace Purchase.Msv.Services
{
    public class PurchaseService
    {
        private readonly ILogger<PurchaseService> _logger;
        private readonly PurchaseMsvDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public PurchaseService(ILogger<PurchaseService> logger, PurchaseMsvDbContext dbContext, IMapper mapper, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<ServiceResult<TrxPurchase>> UpdateStatusPurchaseResult(PurchaseResultMessage purchaseResult)
        {
            try
            {
                if (purchaseResult == null)
                {
                    return new ServiceResult<TrxPurchase>(false)
                    {
                        IsSuccess = false,
                        ErrorMessage = "Purchase result is required.",
                        ErrorCode = "PURCHASE_RESULT_REQUIRED"
                    };
                }
                var purchase = await _dbContext.TrxPurchases.FirstOrDefaultAsync(x => x.PurchaseNumber == purchaseResult.PurchaseNumber);
                if (purchase == null)
                {
                    return new ServiceResult<TrxPurchase>(false)
                    {
                        IsSuccess = false,
                        ErrorCode = "PURVHASE_NOT_FOUND",
                        ErrorMessage = "Purchase not found"
                    };
                }
                purchase.Status = purchaseResult.PurchaseResult;
                _dbContext.TrxPurchases.Update(purchase);
                await _dbContext.SaveChangesAsync();

                return new ServiceResult<TrxPurchase>(true)
                {
                    IsSuccess = true,
                    Data = purchase
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating a purchase.");
                return new ServiceResult<TrxPurchase>(false)
                {
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while updating the purchase.",
                    ErrorCode = "DATABASE_ERROR"
                };
            }
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

        public async Task<ServiceResult<TrxPurchase>> UpdatePurchaseAsync(UpdatePurchaseRequest updatePurchaseRequest)
        {
            if (updatePurchaseRequest == null)
            {
                return new ServiceResult<TrxPurchase>(false)
                {
                    IsSuccess = false,
                    ErrorMessage = "Payload is null."
                };

            }
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var selectedPurchase = await _dbContext.TrxPurchases
                                       .Include(x => x.TrxPurchaseDetails)
                                       .FirstOrDefaultAsync(x => x.Id == updatePurchaseRequest.Id);
                if (selectedPurchase == null)
                {
                    return new ServiceResult<TrxPurchase>(false)
                    {
                        IsSuccess = false,
                        ErrorMessage = "Selected purchase not found."
                    };
                }

                _mapper.Map(updatePurchaseRequest, selectedPurchase);
                await _dbContext.SaveChangesAsync();

                var updateData = _mapper.Map<UpdatePurchaseMessage>(selectedPurchase);
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{QueueNames.PurchaseQueue.UpdatePurchaseQueue}"));
                await sendEndpoint.Send(updateData);

                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();

                return new ServiceResult<TrxPurchase>(true)
                {
                    IsSuccess = true,
                    Data = selectedPurchase
                };

            }
            catch (Exception ex)
            {
                await _dbContext.Database.RollbackTransactionAsync();
                return new ServiceResult<TrxPurchase>(false)
                {
                    IsSuccess = false,
                    ErrorMessage = $"Failled update purchase, with error: {ex.Message}"
                };
            }
        }

    }
}
