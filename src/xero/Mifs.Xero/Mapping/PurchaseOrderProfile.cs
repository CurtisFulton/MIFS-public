using AutoMapper;
using System;

using MEXPurchaseOrder = Mifs.MEX.Domain.PurchaseOrder;
using XeroPurchaseOrder = Xero.NetStandard.OAuth2.Model.Accounting.PurchaseOrder;
using XeroCurrencyCode = Xero.NetStandard.OAuth2.Model.Accounting.CurrencyCode;
using XeroLineAmountTypes = Xero.NetStandard.OAuth2.Model.Accounting.LineAmountTypes;

namespace Mifs.Xero.Mapping
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            this.CreateMap<MEXPurchaseOrder, XeroPurchaseOrder>()
                .IgnoreAllUnmapped()
                .ForMember(dest => dest.Date, options => options.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, options => options.MapFrom(src => XeroPurchaseOrder.StatusEnum.AUTHORISED))
                .ForMember(dest => dest.DeliveryDate, options => options.MapFrom(src => src.DueDateTime))
                .ForMember(dest => dest.PurchaseOrderNumber, options => options.MapFrom(src => src.PurchaseOrderNumber))
                .ForMember(dest => dest.CurrencyCode, options => options.MapFrom(src => Enum.Parse<XeroCurrencyCode>(src.CurrencyType != null ? src.CurrencyType.CurrencyTypeCode 
                                                                                                                                              : "AUD")))
                .ForMember(dest => dest.SentToContact, options => options.MapFrom(src => false))
                .ForMember(dest => dest.DeliveryAddress, options => options.MapFrom(src => "TODO"))
                .ForMember(dest => dest.AttentionTo, options => options.MapFrom(src => src.Extension_PurchaseOrder.SupplierAttentionContactFullName))
                .ForMember(dest => dest.DeliveryInstructions, options => options.MapFrom(src => src.SpecialInstruction))
                .ForMember(dest => dest.LineAmountTypes, options => options.MapFrom(src => XeroLineAmountTypes.Exclusive));
        }
    }
}
