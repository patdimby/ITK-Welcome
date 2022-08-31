using System.Collections.Generic;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;

namespace Intitek.Welcome.DataAccess
{
    public class ProfileDataAccess : Repository<Profile, int>, IProfileDataAccess
    {
        public ProfileDataAccess(IUnitOfWork uow) : base(uow)
        {

        }

        public void SaveOrUpdate(Profile profil, List<int> lProfileToDelete)
        {
            if (profil.Id == 0)
            {
                this.Add(profil);
            }
            else
            {
                MergeEntityToContext(profil, lProfileToDelete);
                UnitOfWork.SaveChanges();
            }
        }

        private void MergeEntityToContext(Profile profil, List<int> lProfileToDelete)
        {

            var profilToUpdate = base.Context.Profile.Include("ProfileDocument").SingleOrDefault(p => p.ID == profil.ID);

            if (profilToUpdate != null)
            {
                profil.Id = profil.ID;
                profilToUpdate.Name = profil.Name;

                List<ProfileDocument> lProfileUpdateToDelete = new List<ProfileDocument>();
                foreach (ProfileDocument c in profilToUpdate.ProfileDocument)
                {
                    c.Id = c.ID;
                    if (lProfileToDelete != null && lProfileToDelete.Where(x => x == c.ID_Document).Any())
                    {
                        lProfileUpdateToDelete.Add(c);
                    }
                }
                foreach (ProfileDocument c in lProfileUpdateToDelete)
                {
                    profilToUpdate.ProfileDocument.Remove(c);
                    this.Context.Entry<ProfileDocument>(c).State = EntityState.Deleted;
                }
                foreach (ProfileDocument pd in profil.ProfileDocument)
                {
                    //Comparaison SOurce du context par rapport à l'objet envoyé                
                    if (!profilToUpdate.ProfileDocument.Where(x => x.ID_Document == pd.ID_Document).Any())
                    {
                        pd.ID_Profile = profil.ID;
                        this.Context.Entry<ProfileDocument>(pd).State = EntityState.Added;
                    }
                }
            }
        }

    }
}
