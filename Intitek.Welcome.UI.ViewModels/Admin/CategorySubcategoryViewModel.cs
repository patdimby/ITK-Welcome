using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class CategorySubcategoryViewModel
    {
        public int ID_Category { get; set; }
        public int ID_SubCategory { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
    }
}