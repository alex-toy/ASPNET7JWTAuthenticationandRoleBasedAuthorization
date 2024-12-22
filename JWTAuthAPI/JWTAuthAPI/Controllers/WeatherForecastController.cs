﻿using JWTAuthAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthAPI.Controllers;

public class WeatherForecastController : Controller
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    [Route("Get")]
    public IActionResult Get()
    {
        return Ok(Summaries);
    }

    [HttpGet]
    [Route("GetUserRole")]
    [Authorize(Roles = UserRoles.USER)]
    public IActionResult GetUserRole()
    {
        return Ok(Summaries);
    }

    [HttpGet]
    [Route("GetAdminRole")]
    [Authorize(Roles = UserRoles.ADMIN)]
    public IActionResult GetAdminRole()
    {
        return Ok(Summaries);
    }

    [HttpGet]
    [Route("GetOwnerRole")]
    [Authorize(Roles = UserRoles.OWNER)]
    public IActionResult GetOwnerRole()
    {
        return Ok(Summaries);
    }
}