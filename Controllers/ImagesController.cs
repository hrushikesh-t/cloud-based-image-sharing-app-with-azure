using Azure;
using ImageSharingWithCloud.DAL;
using ImageSharingWithCloud.Models;
using ImageSharingWithCloud.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImageSharingWithCloud.Controllers
{
    // TODO require authorization by default
    [Authorize]
    public class ImagesController(UserManager<ApplicationUser> userManager,
        ApplicationDbContext userContext,
        ILogContext logContext,
        IImageStorage imageStorage,
        ILogger<ImagesController> logger) : BaseController(userManager, imageStorage, userContext)
    {
        
        // TODO
       [HttpGet]
        public ActionResult Upload()
        {
            CheckAda();

            ViewBag.Message = "";
            var imageView = new ImageView();
            return View(imageView);
        }

        // TODO prevent CSRF
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(ImageView imageView)
        {
            CheckAda();

            logger.LogDebug("Processing the upload of an image....");

            await TryUpdateModelAsync(imageView);

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please correct the errors in the form!";
                return View();
            }

            logger.LogDebug("...getting the current logged-in user....");
            var user = await GetLoggedInUser();

            if (imageView.ImageFile == null || imageView.ImageFile.Length <= 0)
            {
                ViewBag.Message = "No image file specified!";
                return View(imageView);
            }

            logger.LogDebug("....saving image metadata in the database....");

            string imageId = null;

            // TODO save image metadata in the database 
            Image image = new Image
            {
                UserId = user.Id,
                UserName = user.UserName,
                Caption = imageView.Caption,
                Description = imageView.Description,
                DateTaken = imageView.DateTaken,
                Valid = true,
                Approved = true
            };
            imageId = await ImageStorage.SaveImageInfoAsync(image);
            
            // end TODO

            logger.LogDebug("...saving image file on disk....");

            // TODO save image file on disk
            await ImageStorage.SaveImageFileAsync(imageView.ImageFile , user.Id , imageId);
            
            logger.LogDebug("....forwarding to the details page, image id = {imageId}", imageId);

            return RedirectToAction("Details", new { UserId = user.Id, Id = imageId });
        }

        // TODO
        [HttpGet]
        public async Task<ActionResult> Details(string userId, string Id)
        {
            CheckAda();

            var image = await ImageStorage.GetImageInfoAsync(userId, Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = $"Details: {Id}" });
            }

            var imageView = new ImageView
            {
                Id = image.Id,
                Caption = image.Caption,
                Description = image.Description,
                DateTaken = image.DateTaken,
                Uri = ImageStorage.ImageUri(image.UserId, image.Id),

                UserName = image.UserName,
                UserId = image.UserId
            };

            // TODO Log this view of the image
            var user = await GetLoggedInUser();
            if (user != null)
            {
                await logContext.AddLogEntryAsync(user.Id, user.UserName,imageView);
            }

            return View(imageView);
        }

        // TODO
        [HttpGet]
        public async Task<ActionResult> Edit(string userId, string Id)
        {
            CheckAda();
            var user = await GetLoggedInUser();
            if (user == null || !user.Id.Equals(userId))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotAuth" });
            }

            var image = await ImageStorage.GetImageInfoAsync(userId, Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotFound" });
            }

            ViewBag.Message = "";

            var imageView = new ImageView()
            {
                Id = image.Id,
                Caption = image.Caption,
                Description = image.Description,
                DateTaken = image.DateTaken,

                UserId = image.UserId,
                UserName = image.UserName
            };

            return View("Edit", imageView);
        }

        // TODO prevent CSRF
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DoEdit(string userId, string Id, ImageView imageView)
        {
            CheckAda();

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please correct the errors on the page";
                imageView.Id = Id;
                return View("Edit", imageView);
            }

            var user = await GetLoggedInUser();
            if (user == null || !user.Id.Equals(userId))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotAuth" });
            }

            logger.LogDebug("Saving changes to image " + Id);
            var image = await ImageStorage.GetImageInfoAsync(imageView.UserId, Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotFound" });
            }

            image.Caption = imageView.Caption;
            image.Description = imageView.Description;
            image.DateTaken = imageView.DateTaken;
            await ImageStorage.UpdateImageInfoAsync(image);

            return RedirectToAction("Details", new { UserId = userId, Id = Id });
        }

        // TODO
        [HttpGet]
        public async Task<ActionResult> Delete(string userId, string Id)
        {
            CheckAda();
            var user = await GetLoggedInUser();
            if (user == null || !user.Id.Equals(userId))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotAuth" });
            }

            var image = await ImageStorage.GetImageInfoAsync(user.Id, Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotFound" });
            }

            ImageView imageView = new ImageView()
            {
                Id = image.Id,
                Caption = image.Caption,
                Description = image.Description,
                DateTaken = image.DateTaken,
                UserName = image.UserName
            };
            return View(imageView);
        }

        // TODO prevent CSRF
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DoDelete(string userId, string Id)
        {
            CheckAda();
            var user = await GetLoggedInUser();
            if (user == null || !user.Id.Equals(userId))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotAuth" });
            }

            var image = await ImageStorage.GetImageInfoAsync(user.Id, Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotFound" });
            }

            await ImageStorage.RemoveImageAsync(image);

            return RedirectToAction("Index", "Home");

        }

        // TODO
        [HttpGet]
        public async Task<ActionResult> ListAll()
        {
            CheckAda();
            var user = await GetLoggedInUser();

            var images = await ImageStorage.GetAllImagesInfoAsync();
            ViewBag.UserId = user.Id;
            return View(images);
        }

        // TODO
        [HttpGet]
        public async Task<IActionResult> ListByUser()
        {
            CheckAda();

            // Return form for selecting a user from a drop-down list
            var userView = new ListByUserModel();
            var defaultId = (await GetLoggedInUser()).Id;

            userView.Users = new SelectList(ActiveUsers(), "Id", "UserName", defaultId);
            return View(userView);
        }

        // TODO
        [HttpGet]
        public async Task<ActionResult> DoListByUser(ListByUserModel userView)
        {
            CheckAda();

            var user = await GetLoggedInUser();
            ViewBag.UserId = user.Id;

            var theUser = await UserManager.FindByIdAsync(userView.Id);
            if (theUser == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "ListByUser" });
            }

            // TODO
            // List all images uploaded by the user in userView
            var images = await ImageStorage.GetImageInfoByUserAsync(theUser);
            return View("ListAll",images);

            // End TODO

        }

        // TODO
        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public ActionResult ImageViews()
        {
            CheckAda();
            return View();
        }


        // TODO
        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public ActionResult ImageViewsList(string today)
        {
            CheckAda();
            logger.LogDebug("Looking up log views, \"Today\"={today}", today);
            AsyncPageable<LogEntry> entries = logContext.Logs("true".Equals(today));
            logger.LogDebug("Query completed, rendering results....");
            return View(entries);
        }

    }

}
