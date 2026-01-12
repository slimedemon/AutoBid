using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MapperProfiles: Profile
{
    public MapperProfiles()
    {
        // Mapp Auction to AuctionDto including Item properties
        CreateMap<Auction, AuctionDto>().IncludeMembers(s => s.Item);
        CreateMap<Item, AuctionDto>();

        // Map CreateAuctionDto to Auction and Item
        CreateMap<CreateAuctionDto, Auction>().ForMember(dest => dest.Item, opt => opt.MapFrom(src=> src));
        CreateMap<CreateAuctionDto, Item>();

        // Map UpdateAuctionDto to Auction - skip null values to preserve existing data
        CreateMap<UpdateAuctionDto, Auction>()
            .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateAuctionDto, Item>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Map AuctionCreateDto and AuctionCreated
        CreateMap<AuctionDto, AuctionCreated>();
    }

}
