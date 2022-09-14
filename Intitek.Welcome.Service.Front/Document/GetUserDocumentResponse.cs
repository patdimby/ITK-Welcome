using System.Collections.Generic;

namespace Intitek.Welcome.Service.Front
{
    public class GetUserDocumentResponse
    {
        public UserDocumentDTO UserDocument { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// DOCUMENTS NÉCESSITANT UNE ACTION DE VOTRE PART
        /// </summary>
        public List<DocumentDTO> LstActionDocuments { get; set; }
        public int ActionsCount { get; set; }
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
        public int Treated { get; set; }
        public int NbReadDocuments { get; set; }

        public int Total() {
            return ActionsCount + NbInformatifDocuments + NbReadDocuments;
        }

        public override string ToString()
        {
            return "Email="+ Email +", A faire="+ LstActionDocuments.Count+ ", Acted=" + NbActionDocuments + ", Readed="+ NbReadDocuments;
        }

    }
}
