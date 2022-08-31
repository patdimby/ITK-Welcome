using Intitek.Welcome.Service.Front;
using Intitek.Welcome.UI.Web.Models;
using System.Collections.Generic;

namespace Intitek.Welcome.UI.ViewModels
{
    public class WelcomeViewModel
    {
        /// <summary>
        /// Message bienvenue
        /// </summary>
        public string WelcomeMessage { get; set; }
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// DOCUMENTS NÉCESSITANT UNE ACTION DE VOTRE PART
        /// </summary>
        public DocumentGrid Grid1 { get; set; }
        /// <summary>
        /// DOCUMENTS INFORMATIFS
        /// </summary>
        public DocumentGrid Grid2 { get; set; }
        /// <summary>
        /// DOCUMENTS DÉJÀ LUS ET REVUS
        /// </summary>
        public DocumentGrid Grid3 { get; set; }
        /// <summary>
        /// STATISTIQUES
        /// </summary>
        public DocumentGrid Grid4 { get; set; }
        public Service.Back.ManagerStats StatModel { get; set; }
        public List<HistoUserQcmDocVersionDTO> HistoUserQcms { get; set; }
        //public List<UserQcmDTO> HistoUserQcms { get; set; }
        public string CodeLangue { get; set; }
        public string DefaultCodeLangue { get; set; }
    }
}
