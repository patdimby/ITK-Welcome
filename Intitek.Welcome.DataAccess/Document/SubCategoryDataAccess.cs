using System.Data.Entity;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class SubCategoryDataAccess : Repository<SubCategory, int>, ISubCategoryDataAccess
    {
        public SubCategoryDataAccess(IUnitOfWork uow) : base(uow)
        {

        }

        public void Update(SubCategory subcateg)
        {
            MergeEntityToContext(subcateg);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(SubCategory subcateg)
        {

            SubCategory scategToUpdate = base.Context.SubCategory.Include("SubCategoryLang").FirstOrDefault(ctg => ctg.ID == subcateg.ID);

            if (scategToUpdate != null)
            {
                scategToUpdate.ID_DocumentCategory = subcateg.ID_DocumentCategory;
                scategToUpdate.Ordre = subcateg.Ordre;

                foreach (SubCategoryLang lang in subcateg.SubCategoryLang)
                {
                    //Comparaison SOurce du context par rapport à l'objet envoyé             
                    if (scategToUpdate.SubCategoryLang.Any(sct => sct.ID_SubCategory == lang.ID_SubCategory && sct.ID_Lang == lang.ID_Lang))
                    {
                        SubCategoryLang sclToUpdate = base.Context.SubCategoryLang.FirstOrDefault(q => q.ID_SubCategory == lang.ID_SubCategory && q.ID_Lang == lang.ID_Lang);
                        if (sclToUpdate != null)
                        {
                            sclToUpdate.Name = lang.Name;
                        }
                    }
                    else
                    {
                        this.Context.Entry<SubCategoryLang>(lang).State = EntityState.Added;
                    }
                }
            }
        }
    }
}
