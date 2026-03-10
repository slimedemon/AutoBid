using System.Formats.Asn1;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController: ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IAuctionRepository _auctionRepository;

    public AuctionController(IAuctionRepository auctionRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _auctionRepository = auctionRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string updatedAfter = null)
    {
        return await _auctionRepository.GetAuctionsAsync(updatedAfter);
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<AuctionDto>> GetAutionById(Guid id)
    {
        var auction = await _auctionRepository.GetAuctionByIdAsync(id);

        if(auction is null) return NotFound();

        return Ok(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = _mapper.Map<Auction>(createAuctionDto);

        auction.Seller = User.Identity.Name;
        _auctionRepository.AddAuction(auction);

        // Publish event to RabbitMQ
        var newAuction = _mapper.Map<AuctionDto>(auction);
        var auctionCreatedEvent = _mapper.Map<AuctionCreated>(newAuction);
        
        await _publishEndpoint.Publish(auctionCreatedEvent);
        
        var result = await _auctionRepository.SaveChangesAsync();
        
        if(!result) return BadRequest("Failed to create auction");       

        return CreatedAtAction(
            nameof(GetAutionById), 
            new { id = auction.Id }, 
            newAuction
        );
    }

    [Authorize]
    [HttpPut("{id:Guid}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        // Check if the auction exists
        var auction = await _auctionRepository.GetAuctionEntityByIdAsync(id);

        if(auction is null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        // Map the updated fields
        _mapper.Map(updateAuctionDto, auction);
        auction.UpdatedAt = DateTime.UtcNow;

        // Publish event to RabbitMQ
        var updatedAuction = _mapper.Map<AuctionDto>(auction);
        var auctionUpdatedEvent = _mapper.Map<AuctionUpdated>(updatedAuction);
        
        await _publishEndpoint.Publish(auctionUpdatedEvent);

        var result = await _auctionRepository.SaveChangesAsync();

        if(!result) return BadRequest("Failed to update auction");

        return Ok(_mapper.Map<AuctionDto>(auction));
    }

    [Authorize]
    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _auctionRepository.GetAuctionEntityByIdAsync(id);

        if(auction is null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        _auctionRepository.RemoveAuction(auction);

        // Publish event to RabbitMQ
        var deletedAuction = _mapper.Map<AuctionDto>(auction);
        var auctionDeletedEvent = _mapper.Map<AuctionDeleted>(deletedAuction);
        
        await _publishEndpoint.Publish(auctionDeletedEvent);

        var result = await _auctionRepository.SaveChangesAsync();

        if(!result) return BadRequest("Failed to delete auction");
        
        return Ok();
    }
}
