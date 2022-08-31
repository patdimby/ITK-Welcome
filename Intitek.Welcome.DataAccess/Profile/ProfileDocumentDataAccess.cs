using System.Collections.Generic;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Infrastructure.Specification;

namespace Intitek.Welcome.DataAccess
{
    public class ProfileDocumentDataAccess : Repository<ProfileDocument, int>, IProfileDocumentDataAccess
    {
        private readonly ProfileDataAccess profileRepository;
        public ProfileDocumentDataAccess(IUnitOfWork uow) : base(uow)
        {
            profileRepository = new ProfileDataAccess(uow);
        }

        public IEnumerable<ProfileDocument> FindBy(Specification<ProfileDocument> Specification)
        {
            var query = this.RepositoryQuery.Join(this.profileRepository.RepositoryQuery,
               profileDoc => profileDoc.ID_Profile,
               profile => profile.ID,
               (profileDoc, profile) => new { profileDoc, profile })
               .Select(pdp => new
               {
                   pdp.profileDoc,
                   pdp.profile
               }).ToList()
               .Select(pd => new ProfileDocument()
               {
                   ID = pd.profileDoc.ID,
                   ID_Profile = pd.profileDoc.ID_Profile,
                   ID_Document = pd.profileDoc.ID_Document,
                   Date = pd.profileDoc.Date,
                   Profile = pd.profile,
               }); //.ToList();

            return query.AsQueryable().Where(Specification.Predicate);
        }
    }
}
