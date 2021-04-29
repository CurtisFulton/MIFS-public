using AutoMapper;
using Mifs.Extensions;
using Mifs.MEX.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Mifs.Xero.Mapping
{
    public class PurchaseOrderLineProfile : Profile
    {
        public PurchaseOrderLineProfile()
        {
            this.CreateMap<PurchaseOrderLine, LineItem>()
                .IgnoreAllUnmapped()
                .ForMember(dest => dest.Description, options => options.MapFrom(src => GetXeroLineDescription(src)))
                .ForMember(dest => dest.Quantity, options => options.MapFrom(src => src.OrderedQuantity))
                .ForMember(dest => dest.UnitAmount, options => options.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.DiscountRate, options => options.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.LineAmount, options => options.MapFrom(src => src.TotalExcludingTax))
                .ForMember(dest => dest.TaxAmount, options => options.MapFrom(src => src.TotalTax))
                .ForMember(dest => dest.AccountCode, options => options.MapFrom(src => src.AccountCode.AccountCodeName));
        }

        private string GetXeroLineDescription(PurchaseOrderLine poLine)
        {
            return poLine.SupplierStockNumber.Join(" - ", poLine.PurchaseOrderLineDescription);
        }
    }
}
