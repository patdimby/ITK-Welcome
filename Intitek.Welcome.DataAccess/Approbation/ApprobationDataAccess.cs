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
    public class ApprobationDataAccess : Repository<IntitekUser, int>, IApprobationDataAccess
    {
        public ApprobationDataAccess(IUnitOfWork uow) : base(uow)
        {
        }
        public IEnumerable<f_ListApprobations_Result> ListAll(string requestMonth)
        {
            return this.Context.Database.SqlQuery<f_ListApprobations_Result>("SELECT * FROM f_ListApprobations('" + requestMonth + "') ORDER BY 2,3,4,5,6,7").ToList();
        }
        public IEnumerable<ApprobationUserDTO> ListAllUsers(string requestMonth)
        {
            return this.Context.Database.SqlQuery<ApprobationUserDTO>("SELECT DISTINCT Mois, Entité, Agence, FullName, Email, Actif, Present, ID_User FROM f_ListApprobations('" + requestMonth + "') ORDER BY 2, 3, 4, 5, 6, 7").ToList();
        }
        public IEnumerable<ApprobationDocPerUserDTO> ListAllDocsPerUser(string requestMonth)
        {
            return this.Context.Database.SqlQuery<ApprobationDocPerUserDTO>("SELECT DISTINCT ID_User, ID_Document, Resultat FROM f_ListApprobations('" + requestMonth + "') ORDER BY 1").ToList();
        }
        public IEnumerable<ApprobationDocDTO> ListAllDocs(string requestMonth)
        {
            return this.Context.Database.SqlQuery<ApprobationDocDTO>("SELECT DISTINCT Mois, Nom_Document, ID_Document, Version FROM f_ListApprobations('" + requestMonth + "') ORDER BY 1, 2, 3, 4").ToList();
        }
    }
}
