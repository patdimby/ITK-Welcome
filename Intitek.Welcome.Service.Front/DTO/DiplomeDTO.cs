using Intitek.Welcome.Domain;
using System;

namespace Intitek.Welcome.Service.Front
{
    public class DiplomeDTO
    {
        public const string TEMPLATE_FILENAME = "_sensibilisation.pdf";
        /// <summary>
        /// ID Qcm
        /// </summary>
        public int ID { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserQcm UserQcm { get; set; }
        public DateTime Date { get; set; }
        public string FullName { get; set; }
        public string CodeLangue { get; set; }
        public string CodeDefaultLangue { get; set; }
        public string NameOrFullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FullName))
                    return this.Name;
                return FullName;
            }
        }
        public string DiplomeTemplateFilename
        {
            get
            {
                return string.Format("diplome_{0}_{1}.pdf", this.ID, CodeLangue);
            }
        }
        public string DefaultTemplateFilename
        {
            get
            {
                return string.Format("diplome_{0}_{1}.pdf", this.ID, CodeDefaultLangue);
            }
        }
    }
}
