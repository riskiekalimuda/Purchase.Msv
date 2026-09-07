using System.Linq;
using AutoMapper;
using MessageMQCommon.MQ.Messages.PurchaseMsv;
using Purchase.Msv.DTOs;
using Purchase.Msv.Models;

namespace Purchase.Msv.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreatePurchaseRequest, TrxPurchase>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"))
                .ForMember(dest => dest.TrxPurchaseDetails, opt => opt.MapFrom(src => src.Details));

            CreateMap<CreatePurchaseDetail, TrxPurchaseDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseId, opt => opt.Ignore());  
            
            CreateMap<TrxPurchase, PurchaseMessage>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.TrxPurchaseDetails));

            CreateMap<TrxPurchaseDetail, PurchaseDetailMessage>();
        }
    }
}
