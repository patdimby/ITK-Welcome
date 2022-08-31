using System.Collections.Generic;

namespace Intitek.Welcome.Service.Front
{
    public class GetUserDocumentResponse
    {
        public UserDocumentDTO UserDocument { get; set; }

        /// <summary>
        /// DOCUMENTS NÉCESSITANT UNE ACTION DE VOTRE PART
        /// </summary>
        public List<DocumentDTO> LstActionDocuments { get; set; }
        public int NbActionDocuments { get; set; }
        /// <summary>
        /// DOCUMENTS INFORMATIFS
        /// </summary>
        public List<DocumentDTO> LstInformatifDocuments { get; set; }
        public int NbInformatifDocuments { get; set; }
        /// <summary>
        /// DOCUMENTS DÉJÀ LUS ET REVUS
        /// </summary>
        public List<DocumentDTO> LstReadDocuments { get; set; }
        public int NbReadDocuments { get; set; }

    }
}
