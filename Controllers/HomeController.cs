using System.Diagnostics;
using ImageSharingWithCloud.DAL;
using ImageSharingWithCloud.Models;
using ImageSharingWithCloud.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageSharingWithCloud.Controllers
{
    public class HomeController(UserManager<ApplicationUser> userManager, 
        IImageStorage imageStorage, 
        ApplicationDbContext db) : BaseController(userManager, imageStorage, db)
    {

        [HttpGet]
        public async Task<IActionResult> Index(string userName = "Stranger")
        {
            CheckAda();
            ViewBag.Title = "Welcome!";
            var user = await GetLoggedInUser();
            if (user == null)
            {
                ViewBag.UserName = userName;
            }
            else
            {
                ViewBag.UserName = user.UserName;
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errId)
        {
            CheckAda();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrId = errId });
        }
    }
}
