using JWTAuthAPI.Dtos;
using JWTAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            AuthServiceResponseDto result = await _authService.SeedRolesAsync();

            if (!result.IsSucceed) return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            AuthServiceResponseDto registerResult = await _authService.RegisterAsync(registerDto);

            if (registerResult.IsSucceed) return Ok(registerResult.Message);

            return BadRequest(registerResult.Message);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            AuthServiceResponseDto loginResult = await _authService.LoginAsync(loginDto);

            if (loginResult.IsSucceed) return Ok(loginResult.Message);

            return Unauthorized(loginResult.Message);
        }

        [HttpPost]
        [Route("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            AuthServiceResponseDto operationResult = await _authService.MakeAdminAsync(updatePermissionDto);

            if (operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }

        [HttpPost]
        [Route("make-owner")]
        public async Task<IActionResult> MakeOwner([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            AuthServiceResponseDto operationResult = await _authService.MakeOwnerAsync(updatePermissionDto);

            if (operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }
    }
}
