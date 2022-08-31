namespace Intitek.Welcome.Service.Front
{
    public class GetUserDocumentRequest
    {
        public int IDLang { get; set; }
        public int IDDefaultLang { get; set; }
        public string GridName { get; set; }
        /// <summary>
        /// ID utilisateur
        /// </summary>
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        /// <summary>
        /// Nombre de documents à afficher par page
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// Page
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Nom de colonne à trier
        /// </summary>
        public string OrderColumn { get; set; }
        /// <summary>
        /// Sort direction
        /// </summary>
        public int SortDirection { get; set; }
        /// <summary>
        /// Filtres colonnes
        /// </summary>
        public string[] Filtres { get; set; }
        /// <summary>
        /// Input multi-recherche search*
        /// </summary>
        public string Search { get; set; }
        public string SortAscDesc
        {
            get
            {
                return this.SortDirection == 1 ? " DESC" : " ASC";
            }
        }

    }
}
