using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class DocumentLangDataAccess : Repository<DocumentLang, int>, IDocumentLangDataAccess
    {
        public DocumentLangDataAccess(IUnitOfWork uow) : base(uow)
        {
           
        }

        public void Update(DocumentLang docLang)
        {
            MergeEntityToContext(docLang);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(DocumentLang docLang)
        {
            DocumentLang documentLangToUpdate = base.Context.DocumentLang.FirstOrDefault(dl => dl.ID_Document == docLang.ID_Document && dl.ID_Lang == docLang.ID_Lang);
            if (documentLangToUpdate != null)
            {
                documentLangToUpdate.ID_Document = docLang.ID_Document;
                documentLangToUpdate.ID_Lang = docLang.ID_Lang;
                documentLangToUpdate.Data = docLang.Data;
                documentLangToUpdate.Name = docLang.Name;
                documentLangToUpdate.NomOrigineFichier = docLang.NomOrigineFichier;
            }
        }
    }
}
