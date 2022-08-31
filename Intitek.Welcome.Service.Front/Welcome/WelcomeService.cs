using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using System.Linq;

namespace Intitek.Welcome.Service.Front
{
    public class WelcomeService : BaseService, IWelcomeService
    {
        private WelcomeDataAccess _welcRepository;
        public WelcomeService(ILogger logger) : base(logger)
        {
            _welcRepository = new WelcomeDataAccess(uow);
        }

        public string GetWelcomeMessageByLang(int idLang)
        {
            return _welcRepository.FindBy(new Specification<WelcomeMessage>(w => w.ID_Lang == idLang)).Select(w => w.Message).FirstOrDefault();
        }

        public string GetWelcomeMessageByLang(string culture)
        {
            return _welcRepository.FindBy(new Specification<WelcomeMessage>(w => w.Lang.Code.StartsWith(culture))).Select(w => w.Message).FirstOrDefault();
        }
    }
}
