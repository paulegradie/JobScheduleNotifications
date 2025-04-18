using JobScheduleNotifications.Contracts.Authentication;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduleNotifications.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Register(RegisterBusinessOwnerDto request)
        {
            try
            {
                var (owner, token) = await _authenticationService.RegisterBusinessOwnerAsync(
                    request.Email,
                    request.Password,
                    request.BusinessName,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.BusinessAddress,
                    request.BusinessDescription);

                var response = new AuthenticationResponseDto
                {
                    Id = owner.Id,
                    Email = owner.Email,
                    BusinessName = owner.BusinessName,
                    FirstName = owner.FirstName,
                    LastName = owner.LastName,
                    Token = token,
                    TokenExpiration = DateTime.UtcNow.AddDays(7)
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Login(LoginRequestDto request)
        {
            try
            {
                var (owner, token) = await _authenticationService.LoginAsync(
                    request.Email,
                    request.Password);

                var response = new AuthenticationResponseDto
                {
                    Id = owner.Id,
                    Email = owner.Email,
                    BusinessName = owner.BusinessName,
                    FirstName = owner.FirstName,
                    LastName = owner.LastName,
                    Token = token,
                    TokenExpiration = DateTime.UtcNow.AddDays(7)
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("validate-token")]
        public async Task<ActionResult<bool>> ValidateToken([FromBody] string token)
        {
            var isValid = await _authenticationService.ValidateTokenAsync(token);
            return Ok(isValid);
        }
    }
} 