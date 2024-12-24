using JWTAuthAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthAPI.Controllers;

public class HomeController : Controller
{
    private static readonly string[] UserInfo = new[] { "user 1", "user 2", "user 3", "user 4" };
    private static readonly string[] OwnerInfo = new[] { "owner 1", "owner 2", "owner 3", "owner 4" };
    private static readonly string[] AdminInfo = new[] { "admin 1", "admin 2", "admin 3", "admin 4" };

    [HttpGet]
    [Route("Get")]
    public IActionResult Get()
    {
        return Ok(UserInfo);
    }

    [HttpGet]
    [Route("GetUserInfo")]
    [Authorize(Roles = UserRoles.USER)]
    public IActionResult GetUserInfo()
    {
        return Ok(UserInfo);
    }

    [HttpGet]
    [Route("GetAdminInfo")]
    [Authorize(Roles = UserRoles.ADMIN)]
    public IActionResult GetAdminInfo()
    {
        return Ok(AdminInfo);
    }

    [HttpGet]
    [Route("GetOwnerInfo")]
    [Authorize(Roles = UserRoles.OWNER)]
    public IActionResult GetOwnerInfo()
    {
        return Ok(OwnerInfo);
    }
}
