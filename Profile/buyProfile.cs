using AutoMapper;
using broker.Dto;
using broker.Models;

namespace broker.Profiles
{
    public class SalesProfile : Profile
    {
        public SalesProfile()
        {
            CreateMap<broker.Models.Sales, SalesDto>()
            .ForMember(dest => dest.SalesId, opt => opt.MapFrom(src => src.SalesId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.BrokerId, opt => opt.MapFrom(src => src.BrokerId))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.Broker, opt => opt.MapFrom(src => src.Broker));

            // .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));

            CreateMap<SalesDto, broker.Models.Sales>();

        }
        //  

    }

}


    