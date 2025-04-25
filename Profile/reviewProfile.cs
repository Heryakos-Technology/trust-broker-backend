
using AutoMapper;
using broker.Dto;
using broker.Models;

namespace broker.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<broker.Models.Review, ReviewDto>()
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.ReviewId))
            .ForMember(dest => dest.rate, opt => opt.MapFrom(src => src.rate))
             .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.BrokerId, opt => opt.MapFrom(src => src.BrokerId))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.Broker, opt => opt.MapFrom(src => src.Broker));
           CreateMap<ReviewDto, broker.Models.Review>();
        }
        // 
        
    }

}
 