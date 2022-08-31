using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class InvalidKeywordsViewModel
    {
        public List<string> InvalidKeywords { get; set; }

        public string InvalidKeywordsString
        {
            get
            {
                return string.Join(", ", InvalidKeywords);
            }
        }
    }
}
