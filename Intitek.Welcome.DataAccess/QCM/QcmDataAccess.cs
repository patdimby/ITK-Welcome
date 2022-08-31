using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;

namespace Intitek.Welcome.DataAccess
{
    public class QcmDataAccess: Repository<Qcm, int>, IQcmDataAccess
    {
        public QcmDataAccess(IUnitOfWork uow): base(uow)
        {
            
        }

        public new void Remove(Qcm qcm)
        {
            var questionsToRemove = base.Context.Question.Where(q => q.Id_Qcm == qcm.Id);
            foreach(var q in questionsToRemove)
            {
                var reponsesToRemove = base.Context.Reponse.Where(r => r.ID_Question == q.Id);
                foreach(var r in reponsesToRemove)
                {
                    base.Context.Reponse.Remove(r);
                }
                base.Context.Question.Remove(q);
            }

            base.Context.Qcm.Remove(qcm);
            UnitOfWork.SaveChanges();
        }

        public void Update(Qcm question)
        {
            MergeEntityToContext(question);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(Qcm qcm)
        {

            Qcm qcmToUpdate = base.Context.Qcm.FirstOrDefault(q => q.Id == qcm.Id);

            if (qcmToUpdate != null)
            {

                qcmToUpdate.Id = qcm.Id;
                qcmToUpdate.DateCreation = qcm.DateCreation;
                //qcmToUpdate.QcmName = qcm.QcmName;
                qcmToUpdate.NbQuestions = qcm.NbQuestions;
                qcmToUpdate.NoteMinimal = qcm.NoteMinimal;
                qcmToUpdate.Inactif = qcm.Inactif;

            }

            if (qcm.QcmLang != null && qcm.QcmLang.Any())
            {
                foreach (QcmLang ql in qcm.QcmLang)
                {
                    //Comparaison SOurce du context par rapport à l'objet envoyé                
                    if (base.Context.QcmLang.Any(r => ql.ID_Qcm == r.ID_Qcm && ql.ID_Lang == r.ID_Lang))
                    {
                        QcmLang qlToUpdate = base.Context.QcmLang.FirstOrDefault(r => ql.ID_Qcm == r.ID_Qcm && ql.ID_Lang == r.ID_Lang);
                        if (qlToUpdate != null)
                        {
                            qlToUpdate.ID_Qcm = ql.ID_Qcm;
                            qlToUpdate.ID_Lang = ql.ID_Lang;
                            qlToUpdate.QcmName = ql.QcmName;
                        }
                    }
                    else
                    {
                        this.Context.Entry(ql).State = EntityState.Added;
                    }
                }
            }
        }

    }
}
