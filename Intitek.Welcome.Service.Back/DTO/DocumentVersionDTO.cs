namespace Intitek.Welcome.Service.Back
{
    public class DocumentVersionDTO
    {
        public long ID { get; set; }
        public int ID_Document { get; set; }
        public int ID_Lang { get; set; }
        public int ID_UserCre { get; set; }
        public string UserName { get; set; }
        public bool IsMajor { get; set; }
        public string DateCre { get; set; }
        public string ContentType { get; set; }
        public string Extension { get; set; }
        public byte[] Data { get; set; }
        public string Name { get; set; }
        public string NomOrigineFichier { get; set; }
        public string Version { get; set; }
    }
}
