using Intitek.Welcome.Domain;
using Intitek.Welcome.Service.Back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public int Id_Lang { get; set; }
        public int Id_Qcm { get; set; }
        public string TexteQuestion { get; set; }
        public string DefaultTexteQuestion { get; set; }
        public string TexteJustification { get; set; }
        public string DefaultTexteJustification { get; set; }
        public string Photo { get; set; }
        public int OrdreQuestion { get; set; }
        public int inactif { get; set; }
        public bool IsMultipleReponse { get; set; }
        public List<ReponseDTO> Reponses { get; set; }

        public byte[] Illustration { get; set; }
    }
}
