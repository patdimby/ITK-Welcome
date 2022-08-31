using Intitek.Welcome.UI.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Intitek.Welcome.UI.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Message_ValidationRequiredParams")]
        [Display(ResourceType = typeof(Resource), Name = "login_Username")]
        public string Login { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Message_ValidationRequiredParams")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resource), Name = "login_Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "login_RememberMe")]
        public bool RememberMe { get; set; }

        public string MaintenanceMode { get; set; }
        public string ReturnUrl { get; set; }
    }
}
