using AutoMapper;
using Xero.NetStandard.OAuth2.Model.Accounting;
using MEXContact = Mifs.MEX.Domain.Contact;

namespace Mifs.Xero.Mapping
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            this.CreateMap<Address, MEXContact>()
                .IgnoreAllUnmapped()
                .ForMember(dest => dest.Address1, options => options.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.Address2, options => options.MapFrom(src => src.AddressLine2))
                .ForMember(dest => dest.City, options => options.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, options => options.MapFrom(src => src.Country))
                .ForMember(dest => dest.State, options => options.MapFrom(src => src.Region))
                .ForMember(dest => dest.City, options => options.MapFrom(src => src.City))
                .ForMember(dest => dest.PostCode, options => options.MapFrom(src => src.PostalCode));
        }
    }
}
