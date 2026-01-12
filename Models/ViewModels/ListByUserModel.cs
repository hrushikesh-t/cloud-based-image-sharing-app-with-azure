using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImageSharingWithCloud.Models.ViewModels
{
    public class ListByUserModel
    {
        public string Id { get; init; }
        public IEnumerable<SelectListItem> Users { get; set; }
    }
}
