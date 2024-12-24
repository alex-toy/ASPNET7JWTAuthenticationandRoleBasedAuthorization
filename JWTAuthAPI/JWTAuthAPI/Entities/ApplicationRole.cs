using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JWTAuthAPI.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    private ApplicationRole() { }

    public ApplicationRole(string role, string faTitle) : base(role)
    {
        FaTitle = faTitle;
    }


    [MaxLength(50)]
    public string FaTitle { get; set; }
}
