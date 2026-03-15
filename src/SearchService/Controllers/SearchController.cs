
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelper;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchAsync([FromQuery] SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();

        if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
        {
            query.Match(i => i.Make.ToLower().Contains(searchParams.SearchTerm.ToLower())
                || i.Model.ToLower().Contains(searchParams.SearchTerm.ToLower()));
        }

        query = searchParams.OrderBy?.ToLower() switch
        {
            "make" => query.Sort(i => i.Make, Order.Ascending)
                .Sort(i => i.Model, Order.Ascending),
            "new" => query.Sort(i => i.CreatedAt, Order.Descending),
            _ => query.Sort(i => i.AuctionEnd, Order.Ascending)
        };

        query = searchParams.FilterBy?.ToLower() switch
        {
            "finished" => query.Match(i => i.AuctionEnd < DateTime.UtcNow),
            "endingsoon" => query.Match(i => i.AuctionEnd < DateTime.UtcNow.AddHours(6)
                && i.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(i => i.AuctionEnd > DateTime.UtcNow)
        };

        if (!string.IsNullOrWhiteSpace(searchParams.Seller))
        {
            query.Match(i => i.Seller == searchParams.Seller);
        }

        if (!string.IsNullOrWhiteSpace(searchParams.Winner))
        {
            query.Match(i => i.Winner == searchParams.Winner);
        }

        query.PageSize(searchParams.PageSize);
        query.PageNumber(searchParams.PageNumber);

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
