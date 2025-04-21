using System.Security.Authentication;
using Api.Infrastructure.Auth.AccessPolicies;
using Api.Infrastructure.DbTables;
using Microsoft.AspNetCore.Identity;
using Server.Contracts.Client.Endpoints.Auth;

namespace Api.Infrastructure.Auth;

public class Authenticator : IAuthenticator
{
    private readonly IJwt _jwt;
    private readonly UserManager<ApplicationUserRecord> _userManager;
    private readonly SignInManager<ApplicationUserRecord> _signInManager;

    public Authenticator(
        IJwt jwt,
        UserManager<ApplicationUserRecord> userManager,
        SignInManager<ApplicationUserRecord> signInManager)
    {
        _jwt = jwt;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AppSignInResult> SignIn(string email, string password, CancellationToken cancellationToken)
    {
        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            true,
            false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user?.UserName is null)
                throw new AuthenticationException("Could not find user after login - this should not happen!");

            var token = _jwt.GenerateJwtToken(user.IsAdmin, user.UserName);
            var refreshToken = _jwt.GenerateRefreshToken();

            // Store refresh token in the database
            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpiryTime = refreshToken.Expires;
            await _userManager.UpdateAsync(user);

            return new AppSignInResult(user.UserName, token, refreshToken.Token);
        }


        if (result.IsLockedOut)
        {
            throw new AuthenticationException("User account locked out");
        }

        if (result.IsNotAllowed)
        {
            throw new AuthenticationException("Not allowed to sign in");
        }

        if (result.RequiresTwoFactor)
        {
            // Handle 2FA requirement here, if applicable
            throw new NotImplementedException("This should be implemented later");
        }

        throw new AuthenticationException("Invalid login attempt");
    }

    public async Task SignOut(SignOutRequest request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
    
        // Invalidate the refresh token
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user != null)
        {
            user.RefreshToken = null!;
            await _userManager.UpdateAsync(user);
        }
    
        await _signInManager.SignOutAsync();
    }

    public async Task<RegistrationResult> Register(
        RegisterNewAdminRequest req,
        CancellationToken cancellationToken)
    {
        var newAdminUser = new ApplicationUserRecord(true, req.Email)
        {
            
        };
        var newUserResult = await _userManager.CreateAsync(newAdminUser, req.Password);
        if (newUserResult is null || !newUserResult.Succeeded || newAdminUser?.Email is null)
        {
            var msg = newUserResult?.Errors.Select(x => x.Description);
            throw new AuthenticationException(msg is null ? "Failed to create new user" : string.Join(", ", msg));
        }

        await _userManager.AddToRoleAsync(newAdminUser, UserRoles.AdminRole);
        return new RegistrationResult(newAdminUser.Email);
    }

    public async Task<AppSignInResult> RefreshToken(string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var principal = _jwt.GetPrincipalFromExpiredToken(accessToken);
        var username = principal.Identity?.Name;
    
        var user = await _userManager.FindByNameAsync(username);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new AuthenticationException("Invalid refresh token or token expired");
        }
    
        var newAccessToken = _jwt.GenerateJwtToken(user.IsAdmin, user.UserName);
        var newRefreshToken = _jwt.GenerateRefreshToken();
    
        user.RefreshToken = newRefreshToken.Token;
        user.RefreshTokenExpiryTime = newRefreshToken.Expires;
        await _userManager.UpdateAsync(user);
    
        return new AppSignInResult(user.UserName, newAccessToken, newRefreshToken.Token);
    }

}