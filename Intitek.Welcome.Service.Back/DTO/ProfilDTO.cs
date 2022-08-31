namespace Intitek.Welcome.Service.Back
{
    public class ProfilDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Affecte { get; set; }
        public bool IsSessionChecked { get; set; }
        public int DocumentRead { get; set; }
        public int DocumentApproved { get; set; }
        public int DocumentTested { get; set; }
        public Statistiques Statistiques { get; set; }
    }
}
