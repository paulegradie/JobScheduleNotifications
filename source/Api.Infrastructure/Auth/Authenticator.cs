using System.Security.Authentication;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Identity;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Api.Infrastructure.Auth;

/// <summary>
/// Handles sign-in, registration, sign-out, and token refresh flows.
/// </summary>
public class Authenticator : IAuthenticator
{
    private readonly IJwt _jwt;
    private readonly UserManager<ApplicationUserRecord> _userManager;
    private readonly SignInManager<ApplicationUserRecord> _signInManager;
    private readonly AppDbContext _context;

    public Authenticator(
        IJwt jwt,
        UserManager<ApplicationUserRecord> userManager,
        SignInManager<ApplicationUserRecord> signInManager,
        AppDbContext context)
    {
        _jwt = jwt;
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<AppSignInResult> SignIn(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                throw new AuthenticationException("User account locked out");
            if (result.IsNotAllowed)
                throw new AuthenticationException("Not allowed to sign in");
            if (result.RequiresTwoFactor)
                throw new NotImplementedException("Two-factor authentication not implemented");

            throw new AuthenticationException("Invalid login attempt");
        }

        var user = await _userManager.FindByNameAsync(email) ?? throw new AuthenticationException("User not found after sign-in");

        // fetch all roles for this user
        var roles = await _userManager.GetRolesAsync(user);

        // optionally resolve a single customerId here if needed
        CustomerId? custId = null;
        // e.g. if (roles.Contains(Roles.Customer)) { ... lookup from CustomerUsers }

        var orgId = await _context.OrganizationUsers
            .Where(ou => ou.IdentityUserId == user.Id)
            .Select(ou => ou.OrganizationId)
            .SingleOrDefaultAsync(cancellationToken);
        if (orgId == null)
            throw new AuthenticationException("User not assigned to an organization");


        // generate tokens
        var accessToken = _jwt.GenerateJwtToken(user.Id, user.UserName!, roles, customerId: custId, organizationId: orgId);
        var refreshToken = _jwt.GenerateRefreshToken();

        // persist refresh token
        user.RefreshToken = refreshToken.Token;
        user.RefreshTokenExpiryTime = refreshToken.Expires;
        await _userManager.UpdateAsync(user);

        return new AppSignInResult(user.UserName!, accessToken, refreshToken.Token);
    }

    public async Task SignOut(
        SignOutRequest request,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var user = await _userManager.FindByNameAsync(request.Email);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
        }

        await _signInManager.SignOutAsync();
    }

    public async Task<RegistrationResult> Register(
        RegisterNewAdminRequest req,
        CancellationToken cancellationToken)
    {
        var newUser = new ApplicationUserRecord(req.Email, req.PhoneNumber);
        var createRes = await _userManager.CreateAsync(newUser, req.Password);
        if (!createRes.Succeeded)
        {
            var errors = string.Join(", ", createRes.Errors.Select(e => e.Description));
            throw new AuthenticationException(
                string.IsNullOrWhiteSpace(errors)
                    ? "Failed to create new user"
                    : errors);
        }

        // assign to Admin role
        await _userManager.AddToRoleAsync(newUser, Roles.OrganizationOwner);
        await _userManager.AddToRoleAsync(newUser, Roles.OrganizationAdmin);
        await _userManager.AddToRoleAsync(newUser, Roles.OrganizationMember);

        return new RegistrationResult(newUser.UserName!, newUser.Id);
        ;
    }

    public async Task<AppSignInResult> RefreshToken(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var principal = _jwt.GetPrincipalFromExpiredToken(accessToken);
        var userName = principal.Identity?.Name
                       ?? throw new AuthenticationException("Invalid token principal");

        var user = await _userManager.FindByNameAsync(userName)
                   ?? throw new AuthenticationException("User not found for token refresh");

        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new AuthenticationException("Invalid refresh token or token expired");

        var roles = await _userManager.GetRolesAsync(user);

        // Get organization ID for the user
        var orgId = await _context.OrganizationUsers
            .Where(ou => ou.IdentityUserId == user.Id)
            .Select(ou => ou.OrganizationId)
            .SingleOrDefaultAsync();

        // refresh tokens
        var newAccessToken = _jwt.GenerateJwtToken(user.Id, user.UserName!, roles, customerId: null, organizationId: orgId);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken.Token;
        user.RefreshTokenExpiryTime = newRefreshToken.Expires;
        await _userManager.UpdateAsync(user);

        return new AppSignInResult(user.UserName!, newAccessToken, newRefreshToken.Token);
    }
}