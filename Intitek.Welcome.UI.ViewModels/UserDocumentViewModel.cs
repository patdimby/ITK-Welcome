using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels
{
    public class UserDocumentViewModel
    {
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentVersion { get; set; }
        public int QcmID { get; set; }
        public bool IsNoActionRequired { get; set; }
        public bool IsRead { get; set; }
        public bool IsApproved { get; set; }
        public bool IsTested { get; set; }
        public bool IsDocumentToRead { get; set; }
        public bool IsDocumentToApprouve { get; set; }
        public bool IsDocumentToTest { get; set; }
        public string PdfFile { get; set; }
        public string Url { get; set; }
        public string Error { get; set; }
        public bool IsMagazine { get; set; }

    }
}
