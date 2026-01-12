using System.Formats.Asn1;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController: ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string updatedAfter = null)
    {
        var query = _context.Auctions.AsQueryable();
            
        if(!string.IsNullOrWhiteSpace(updatedAfter) && DateTime.TryParse(updatedAfter, out DateTime date))
        {
            var utcDate = date.ToUniversalTime();
            query = query.Where(a => 
                a.UpdatedAt.ToUniversalTime() > utcDate
            );
        }

        var auctions = await query
            .Include(a => a.Item)
            .OrderBy(a => a.Item.Make)
            .AsNoTracking()
            .ToListAsync();
            
        return Ok(_mapper.Map<List<AuctionDto>>(auctions));
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<AuctionDto>> GetAutionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if(auction is null) return NotFound();

        return Ok(_mapper.Map<AuctionDto>(auction));
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = _mapper.Map<Auction>(createAuctionDto);

        auction.Seller = User?.Identity?.Name ?? "Anonymous";
       
        _context.Auctions.Add(auction);
        
        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Failed to create auction");

        // Publish event to RabbitMQ
        var newAuction = _mapper.Map<AuctionDto>(auction);
        var auctionCreatedEvent = _mapper.Map<AuctionCreated>(newAuction);
        await _publishEndpoint.Publish(auctionCreatedEvent);

        Console.WriteLine($"Published AuctionCreated event for AuctionId: {auction.Id}");

        return CreatedAtAction(
            nameof(GetAutionById), 
            new { id = auction.Id }, 
            newAuction
        );
    }

    [HttpPut("{id:Guid}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        // Check if the auction exists
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(a => a.Id == id);

        if(auction is null) return NotFound();

        // Map the updated fields
        _mapper.Map(updateAuctionDto, auction);
        auction.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Failed to update auction");

        return Ok(_mapper.Map<AuctionDto>(auction));
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FirstOrDefaultAsync(a => a.Id == id);

        if(auction is null) return NotFound();

        // TODO: Check seller == User.Identity.Name

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Failed to delete auction");

        return Ok();
    }
}
