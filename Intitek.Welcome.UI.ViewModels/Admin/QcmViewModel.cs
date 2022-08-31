using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class QcmViewModel
    {
        public const string FOLDER_SAVE_BADGE = "~/Content/images/badges/";
        public const string FOLDER_TEMPLATE_DIPLOME = "~/Templates/Diplome/";
        public const string FOLDER_SAVE_QCM = "~/excel/qcm/";
        public int ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "qcm_Name")]
        public string Name { get; set; }
        public string DefaultTradName { get; set; }
        public bool IsDefaultTradName { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "qcm_DateCreate")]
        public System.DateTime DateCreation { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "qcm_NoteMinimal")]
        public Nullable<int> NoteMinimal { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "qcm_Inactif")]
        public bool Inactif { get; set; }

        public bool IsRemovable { get; set; }

        public bool IsUpdatable { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "qcm_NbQuestions")]
        public Nullable<int> NbQuestions { get; set; }
        public List<QuestionDTO> Questions { get; set; }
        //fr ou  en
        public string CodeLangue { get; set; }
        public string Filename {
            get {
                return string.Format("badge_{0}_{1}.png", this.ID, CodeLangue);
            }
        }
        public string TemplateFilename
        {
            get
            {
                return string.Format("diplome_{0}_{1}.pdf", this.ID, CodeLangue);
            }
        }
    }
}
