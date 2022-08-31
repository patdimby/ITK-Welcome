using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.DataAccess
{
    public class ApprobationUserDTO
    {
        public string Mois { get; set; }
        public string Entité { get; set; }
        public string Agence { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Actif { get; set; }
        public int Present { get; set; }
        public int ID_User { get; set; }
    }
}
