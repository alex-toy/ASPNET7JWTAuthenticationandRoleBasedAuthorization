using JWTAuthAPI.Dtos;
using JWTAuthAPI.Entities;
using JWTAuthAPI.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user is null) return new AuthServiceResponseDto(false, "Invalid Credentials");

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordCorrect) return new AuthServiceResponseDto(false, "Invalid Credentials");

        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        List<Claim> authClaims = new ()
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("JWTID", Guid.NewGuid().ToString()),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        string token = GenerateNewJsonWebToken(authClaims);

        return new AuthServiceResponseDto(true, token);
    }

    public async Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

        if (user is null) return new AuthServiceResponseDto(false, "Invalid User name!!!");

        await _userManager.AddToRoleAsync(user, UserRoles.ADMIN);

        return new AuthServiceResponseDto(true, "User is now an ADMIN");
    }

    public async Task<AuthServiceResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

        if (user is null) return new AuthServiceResponseDto(false, "Invalid User name!!!");

        await _userManager.AddToRoleAsync(user, UserRoles.OWNER);

        return new AuthServiceResponseDto(true, "User is now an OWNER");
    }

    public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(registerDto.UserName);

        if (user is not null) return new AuthServiceResponseDto(false, "UserName Already Exists");

        ApplicationUser newUser = new ()
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Mobile = registerDto.Mobile,
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        IdentityResult createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!createUserResult.Succeeded)
        {
            var errorString = "User Creation Failed Because: ";
            foreach (var error in createUserResult.Errors)
            {
                errorString += " # " + error.Description;
            }
            return new AuthServiceResponseDto(false, errorString);
        }

        await _userManager.AddToRoleAsync(newUser, UserRoles.USER);

        return new AuthServiceResponseDto(true, "User Created Successfully");
    }

    public async Task<AuthServiceResponseDto> SeedRolesAsync()
    {
        bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(UserRoles.OWNER);
        bool isAdminRoleExists = await _roleManager.RoleExistsAsync(UserRoles.ADMIN);
        bool isUserRoleExists = await _roleManager.RoleExistsAsync(UserRoles.USER);

        if (isOwnerRoleExists && isAdminRoleExists && isUserRoleExists) return new AuthServiceResponseDto(false, "Roles Seeding is Already Done");

        await _roleManager.CreateAsync(new ApplicationRole(UserRoles.OWNER, UserRoles.OWNER_TITLE));
        await _roleManager.CreateAsync(new ApplicationRole(UserRoles.ADMIN, UserRoles.ADMIN_TITLE));
        await _roleManager.CreateAsync(new ApplicationRole(UserRoles.USER, UserRoles.USER_TITLE));

        return new AuthServiceResponseDto(true, "Role Seeding Done Successfully");
    }

    private string GenerateNewJsonWebToken(List<Claim> claims)
    {
        SymmetricSecurityKey authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

        JwtSecurityToken tokenObject = new (
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(1),
            claims: claims,
            signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
        );

        string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return token;
    }
}
