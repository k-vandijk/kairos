using Carter;
using kairos_api.DTOs.AccountDTOs;
using kairos_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace kairos_api.Endpoints;

public class AccountEndpoints : BaseEndpoint, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account")
            .RequireAuthorization();

        group.MapGet("", GetAccount);
        
        group.MapPost("update", UpdateAccount);
    }

    public async Task<IResult> GetAccount(HttpContext context, IUnitOfWork unitOfWork)
    {
        var currentUser = await GetCurrentUserAsync(context, unitOfWork);
        if (currentUser == null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(currentUser);
    }

    [HttpPost("update")]
    public async Task<IResult> UpdateAccount(HttpContext context, IUnitOfWork unitOfWork, [FromBody] UpdateAccountDTO dto)
    {
        var currentUser = await GetCurrentUserAsync(context, unitOfWork);
        if (currentUser == null)
        {
            return Results.Unauthorized();
        }

        currentUser.FirstName = dto.FirstName;
        currentUser.LastName = dto.LastName;
        currentUser.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Users.Update(currentUser);
        await unitOfWork.CompleteAsync();

        return Results.Ok("User updated successfully.");
    }
}
