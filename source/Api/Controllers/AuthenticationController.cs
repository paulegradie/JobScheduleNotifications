using System.Security.Authentication;
using Api.Controllers.Base;
using Api.Infrastructure.Auth;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Api.Controllers;

[ApiController]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthenticator _authenticator;
    private readonly AppDbContext _uow;

    public AuthenticationController(IAuthenticator authenticator, AppDbContext uow)
    {
        _authenticator = authenticator;
        _uow = uow;
    }

    [HttpPost(RegisterNewAdminRequest.Route)]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterNewAdminRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticator.Register(request, cancellationToken);

            var org = new Organization()
            {
                Customers = { },
                Name = $"{result.Email}'s Org",
                OrganizationUsers = { },
            };
            org.OrganizationUsers.Add(new OrganizationUser
            {
                IdentityUserId = result.IdentityUserId,
                Role = OrganizationRole.Owner
            });
            _uow.Organizations.Add(org);
            await _uow.SaveChangesAsync(cancellationToken);

            return Ok(new RegisterResponse(result.Email));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost(SignInRequest.Route)]
    [AllowAnonymous]
    public async Task<ActionResult<TokenInfo>> Login([FromBody] SignInRequest request)
    {
        try
        {
            var (email, authToken, refreshToken) = await _authenticator.SignIn(request.Email, request.Password, CancellationToken.None);

            var tokenInfo = new TokenInfo
            {
                AccessToken = authToken,
                RefreshToken = refreshToken,
                Email = email
            };

            return Ok(tokenInfo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost(TokenRefreshRequest.Route)]
    [AllowAnonymous]
    public async Task<ActionResult<AppSignInResult>> RefreshToken([FromBody] TokenRefreshRequest request)
    {
        if (request is null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Invalid client request");
        }

        try
        {
            var result = await _authenticator.RefreshToken(request.AccessToken, request.RefreshToken, HttpContext.RequestAborted);
            return Ok(result);
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost(SignOutRequest.Route)]
    [Authorize]
    public async Task<ActionResult> SignOut(SignOutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authenticator.SignOut(request, cancellationToken);
            return Ok("Signed Out");
        }
        catch (Exception ex)
        {
            return BadRequest("Couldn't sign you out");
        }
    }

    // [Authorize] ensures the caller is authenticated
    // [HttpPost("{orgId}/invite")]
    // public async Task<IActionResult> Invite(
    //     Guid orgId,
    //     InviteDto dto,
    //     [FromServices] AppDbContext db,
    //     [FromServices] ICurrentUserService curUser,
    //     [FromServices] ICrudRepository<>)
    // {
    //     // 1) verify caller is an Owner of orgId
    //     
    //     var membership = curUser.OrganizationUsers
    //         .SingleOrDefault(x => x.OrganizationId == orgId);
    //     if (membership?.Role != OrganizationRole.Owner)
    //         return Forbid();
    //
    //     // 2) ensure the invitee user exists (or create)
    //     var invitee = await _userManager.FindByEmailAsync(dto.Email) ?? new ApplicationUserRecord(false, dto.Email);
    //     if (invitee.Id == default)
    //         await _userManager.CreateAsync(invitee, dto.TempPassword);
    //
    //     // 3) add them as an Admin (or Employee)
    //     db.OrganizationUsers.Add(new OrganizationUser {
    //         UserId         = invitee.Id,
    //         OrganizationId = orgId,
    //         Role           = dto.MakeAdmin 
    //             ? OrganizationRole.Admin 
    //             : OrganizationRole.Employee
    //     });
    //     await db.SaveChangesAsync();
    //
    //     return Ok();
    // }
}