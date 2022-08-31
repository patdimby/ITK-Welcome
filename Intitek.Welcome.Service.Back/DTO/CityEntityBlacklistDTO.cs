namespace Intitek.Welcome.Service.Back
{
    public class CityEntityBlacklistDTO
    {
        public string City { get; set; }
        public string Entity { get; set; }
        public System.DateTime DateCre { get; set; }
        public int NbCollabInactif { get; set; }
        public int NbCollabActif { get; set; }
    }
}
