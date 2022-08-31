using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;

namespace Intitek.Welcome.DataAccess
{
    public class DocumentCategoryDataAccess : Repository<DocumentCategory, int>, IDocumentCategoryDataAccess
    {
        public DocumentCategoryDataAccess(IUnitOfWork uow) : base(uow)
        {
            
        }

        public void Update(DocumentCategory categ)
        {
            MergeEntityToContext(categ);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(DocumentCategory categ)
        {

            DocumentCategory categToUpdate = base.Context.DocumentCategory.Include("DocumentCategoryLang").FirstOrDefault(ctg => ctg.ID == categ.ID);

            if (categToUpdate != null)
            {

                categToUpdate.Id = categ.Id;             
                categToUpdate.OrdreCategory = categ.OrdreCategory;

                foreach (DocumentCategoryLang lang in categ.DocumentCategoryLang)
                {
                    //Comparaison SOurce du context par rapport à l'objet envoyé             
                    if (categToUpdate.DocumentCategoryLang.Any(ct => ct.ID_DocumentCategory == lang.ID_DocumentCategory && ct.ID_Lang == lang.ID_Lang))
                    {
                        DocumentCategoryLang ctlToUpdate = base.Context.DocumentCategoryLang.FirstOrDefault(q => q.ID_DocumentCategory == lang.ID_DocumentCategory && q.ID_Lang == lang.ID_Lang);
                        if (ctlToUpdate != null)
                        {
                            ctlToUpdate.Name = lang.Name;
                        }
                    }
                    else
                    {
                        this.Context.Entry<DocumentCategoryLang>(lang).State = EntityState.Added;
                    }
                }
            }

        }
    }
}
