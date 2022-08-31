using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class ReponseDataAccess : Repository<Reponse, int>, IReponseDataAccess
    {
        public ReponseDataAccess(IUnitOfWork uow) : base(uow)
        {

        }

        public void Update(Reponse reponse)
        {
            MergeEntityToContext(reponse);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(Reponse rep)
        {

            Reponse repToUpdate = base.Context.Reponse.FirstOrDefault(q => q.Id == rep.Id);

            if (repToUpdate != null)
            {

                repToUpdate.Id = rep.Id;
               // repToUpdate.Texte = rep.Texte;
                repToUpdate.OrdreReponse = rep.OrdreReponse;
                repToUpdate.ID_Question = rep.ID_Question;
                repToUpdate.IsRight = rep.IsRight;
                repToUpdate.inactif = rep.inactif;
            }

        }

        public void Update(Reponse reponse, int ID_Question)
        {
            MergeEntityToContext(reponse, ID_Question);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(Reponse reponse, int ID_Question)
        {
            Reponse repToUpdate = base.Context.Reponse.FirstOrDefault(q => q.ID_Question == ID_Question);

            if (repToUpdate != null)
            {
                repToUpdate.Id = reponse.Id;
                repToUpdate.ID_Question = reponse.ID_Question;
                repToUpdate.inactif = reponse.inactif;
            }

        }

    }
}

