
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelper;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController: ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchAsync([FromQuery] SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();

        query.Sort(i => i.Make, Order.Ascending);

        if(!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }

        query = searchParams.OrderBy?.ToLower() switch
        {
            "make" => query.Sort(i => i.Make, Order.Ascending),
            "new" => query.Sort(i => i.CreatedAt, Order.Descending),
            _ => query.Sort(i => i.AuctionEnd, Order.Ascending)
        };

        query = searchParams.FilterBy?.ToLower() switch
        {
            "finished" => query.Match(i => i.AuctionEnd < DateTime.UtcNow),
            "endingsoon" => query.Match(i => i.AuctionEnd < DateTime.UtcNow.AddHours(6)
                && i.AuctionEnd > DateTime.UtcNow),
            "ongoing" => query.Match(i => i.AuctionEnd > DateTime.UtcNow),
            _ => query
        };

        if (!string.IsNullOrWhiteSpace(searchParams.Seller))
        {
            query.Match(i => i.Seller.Contains(searchParams.Seller, StringComparison.OrdinalIgnoreCase));
        }

        if(!string.IsNullOrWhiteSpace(searchParams.Winner))
        {
            query.Match(i => i.Winner.Contains(searchParams.Winner, StringComparison.OrdinalIgnoreCase));
        }
            
        query.PageSize(searchParams.PageSize);
        query.PageNumber(searchParams.PageNumber);    

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            items = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
