using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.DataAccess
{
    public class SensibilisationDataAccess : Repository<IntitekUser, int>, ISensibilisationDataAccess
    {
        public SensibilisationDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
        public IEnumerable<f_ListSensibilisations_Result> ListAll(string requestMonth)
        {
            return this.Context.Database.SqlQuery<f_ListSensibilisations_Result>("SELECT * FROM f_ListSensibilisations('" + requestMonth + "') ORDER BY 2,3,4,5,6,7").ToList();
        }
        public IEnumerable<SensibilisationUserDTO> ListAllUsers(string requestMonth)
        {
            return this.Context.Database.SqlQuery<SensibilisationUserDTO>("SELECT DISTINCT Mois, Entité, Agence, FullName, Email, Actif, Present, ID_User FROM f_ListSensibilisations('" + requestMonth + "') ORDER BY 2, 3, 4, 5, 6, 7").ToList();
        }
        public IEnumerable<SensibilisationDocPerUserDTO> ListAllDocsPerUser(string requestMonth)
        {
            return this.Context.Database.SqlQuery<SensibilisationDocPerUserDTO>("SELECT DISTINCT ID_User, ID_Document, Resultat FROM f_ListSensibilisations('" + requestMonth + "') ORDER BY 1").ToList();
        }
        public IEnumerable<SensibilisationDocDTO> ListAllDocs(string requestMonth)
        {
            return this.Context.Database.SqlQuery<SensibilisationDocDTO>("SELECT DISTINCT Mois, Nom_Document, ID_Document, Version FROM f_ListSensibilisations('" + requestMonth + "') ORDER BY 1, 2, 3, 4").ToList();
        }
    }
}
