using Carter;
using kairos_api.DTOs.CapsuleDTOs;
using kairos_api.Repositories;
using kairos_api.Services.CapsuleService;
using Microsoft.AspNetCore.Mvc;

namespace kairos_api.Endpoints;

public class CapsuleEndpoints : BaseEndpoint, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/capsules")
            .RequireAuthorization();

        group.MapGet("", GetCapsules);

        group.MapGet("{capsuleId}", GetCapsule);

        group.MapPost("", CreateCapsule);
    }

    public async Task<IResult> GetCapsules(HttpContext context, IUnitOfWork unitOfWork, ICapsuleService capsuleService)
    {
        var currentUser = await GetCurrentUserAsync(context, unitOfWork);
        if (currentUser == null)
        {
            return Results.Unauthorized();
        }

        var capsules = await capsuleService.GetCapsulesForUserAsync(currentUser);
        return Results.Ok(capsules);
    }

    public async Task<IResult> GetCapsule(
        HttpContext context,
        IUnitOfWork unitOfWork,
        ICapsuleService capsuleService,
        [FromBody] GetCapsuleDTO dto,
        [FromRoute] Guid capsuleId)
    {
        var currentUser = await GetCurrentUserAsync(context, unitOfWork);
        if (currentUser == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var capsule = await capsuleService.GetCapsuleForUserAsync(currentUser, capsuleId, dto);
            return Results.Ok(capsule);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public async Task<IResult> CreateCapsule(
        HttpContext context,
        IUnitOfWork unitOfWork,
        ICapsuleService capsuleService,
        [FromBody] CreateCapsuleDTO dto)
    {
        var currentUser = await GetCurrentUserAsync(context, unitOfWork);
        if (currentUser == null)
        {
            return Results.Unauthorized();
        }

        var result = await capsuleService.CreateCapsuleForUserAsync(currentUser, dto);
        return Results.Ok(result);
    }
}
