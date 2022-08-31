using System.Data.Entity;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;

namespace Intitek.Welcome.DataAccess
{
    public class QcmLangDataAccess : Repository<QcmLang, int>, IQcmLangDataAccess
    {
        public QcmLangDataAccess(IUnitOfWork uow) : base(uow)
        {
            
        }

        public new void Save(QcmLang qcmLang)
        {
            if (base.Context.QcmLang.Any(r => qcmLang.ID_Qcm == r.ID_Qcm && qcmLang.ID_Lang == r.ID_Lang))
            {
                QcmLang qlToUpdate = base.Context.QcmLang.FirstOrDefault(r => qcmLang.ID_Qcm == r.ID_Qcm && qcmLang.ID_Lang == r.ID_Lang);
                if (qlToUpdate != null)
                {
                    qlToUpdate.ID_Qcm = qcmLang.ID_Qcm;
                    qlToUpdate.ID_Lang = qcmLang.ID_Lang;
                    qlToUpdate.QcmName = qcmLang.QcmName;
                }
            }
            else
            {
                this.Context.Entry(qcmLang).State = EntityState.Added;
            }

        }
    }
}
