using Intitek.Welcome.UI.Resources;
using System.ComponentModel.DataAnnotations;

namespace Intitek.Welcome.UI.ViewModels
{
    public class CategoryViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Categories")]
        public string Name { get; set; }
        public int? OrdreCategory { get; set; }
       
    }
}
