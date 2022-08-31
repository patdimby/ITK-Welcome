using System.Linq;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;

namespace Intitek.Welcome.Service.Back
{
    public class LangService : BaseService, ILangService
    {
        private readonly LangDataAccess _langrepository;

        public LangService(ILogger logger) : base(logger)
        {
            _langrepository = new LangDataAccess(uow);

        }
        public GetLangResponse Get(GetLangRequest request)
        {
            Lang result = new Lang();
            if(request.Id > 0)
            {
                result = _langrepository.FindBy(request.Id);
            }
            else if (request.Code != null)
            {
                result = _langrepository.FindBy(new Specification<Lang>(l => l.Code.StartsWith(request.Code.Substring(0,2)))).FirstOrDefault();
            }

            return new GetLangResponse()
            {
                Langue = result
            };
        }

        public GetAllLangResponse GetAll(GetAllLangRequest request)
        {
            return new GetAllLangResponse()
            {
                Langues = _langrepository.FindAll().ToList()
            };
        }
    }
}
