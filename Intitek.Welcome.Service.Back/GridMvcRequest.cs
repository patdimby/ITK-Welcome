namespace Intitek.Welcome.Service.Back
{
    public class GridMvcRequest
    {
        public string GridName { get; set; }
        /// <summary>
        /// Page size
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
        /// <summary>
        /// Categories collapse
        /// </summary>
        public string Categories { get; set; }
        public string SortAscDesc {
            get {
                return this.SortDirection == 1 ? " DESC" : " ASC";
            }
        }
    }
}
