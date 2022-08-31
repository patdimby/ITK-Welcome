using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class QuestionLangDataAccess : Repository<QuestionLang, int>, IQuestionLangDataAccess
    {
        public QuestionLangDataAccess(IUnitOfWork uow) : base(uow)
        {

        }

        public void Delete(int id, int lanId)
        {
            MergeEntityToContext(id, lanId);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(int id, int lanId)
        {
            var qsToUpdate = base.Context.QuestionLang.FirstOrDefault(q => q.ID_Question == id && q.ID_Lang == lanId);
            if (qsToUpdate != null)
            {
                qsToUpdate.Illustration = null;
            }
        }

    }
}
