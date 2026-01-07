using AutoMapper;
using Contracts;
using SearchService.Models;

namespace SearchService.RequestHelper;

public class MapperProfiles: Profile
{
    public MapperProfiles()
    {
        CreateMap<AuctionCreated, Item>();
    }
}
