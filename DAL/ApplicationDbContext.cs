using Microsoft.EntityFrameworkCore;
using ImageSharingWithCloud.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ImageSharingWithCloud.DAL
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
    }

}