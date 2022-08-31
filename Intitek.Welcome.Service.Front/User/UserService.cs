using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using System;
using System.Linq;

namespace Intitek.Welcome.Service.Front
{
    public class UserService : BaseService, IUserService
    {
        private readonly UserDataAccess _userrepository;
        private readonly LogoDataAccess _logoRepository;
        public UserService(ILogger logger) : base(logger)
        {
            _userrepository = new UserDataAccess(uow);
            _logoRepository = new LogoDataAccess(uow);
        }

        public GetIntitekUserByLoginResponse GetIntitekUserByLogin(string username)
        {
            var user = _userrepository.FindBy(new Specification<IntitekUser>(p => p.Username.Equals(username))).FirstOrDefault();
            if (user == null)
                return null;

            var defaultlogo = _logoRepository.FindBy(new Specification<Logos>(l => l.EntityName == "DEFAULT")).FirstOrDefault();
            var logo = _logoRepository.FindBy(new Specification<Logos>(l => user.EntityName.Contains(l.EntityName))).FirstOrDefault();
            return new GetIntitekUserByLoginResponse() {
                User = user,
                Logo = logo != null ? logo.LogoName : defaultlogo.LogoName
            };
        }
        public IntitekUser GetById(int id)
        {
            try
            {

                var user = _userrepository.FindBy(id);
                return user;
                
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Get",
                    ServiceName = "UserService",

                }, ex);
                throw ex;
            }
        }

        public GetIntitekUserByEmailResponse GetIntitekUserByEmail(string email, bool isactive)
        {
            var spec = new Specification<IntitekUser>(p => p.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
            if (isactive)
            {
                spec = new Specification<IntitekUser>(p => p.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && p.Active);
            }
            var user = _userrepository.FindBy(spec).FirstOrDefault();
            if (user == null)
                return null;

            //var defaultlogo = _logoRepository.FindBy(new Specification<Logos>(l => l.EntityName == "DEFAULT")).FirstOrDefault();
            //var logo = _logoRepository.FindBy(new Specification<Logos>(l => user.EntityName.Contains(l.EntityName))).FirstOrDefault();
            return new GetIntitekUserByEmailResponse()
            {
                User = user,
                Logo = GetLogo(user.EntityName.ToUpper())
            };
        }
        
        private string GetLogo(string entityName)
        {

            var defaultlogo = _logoRepository.FindBy(new Specification<Logos>(l => l.EntityName == "DEFAULT")).FirstOrDefault();
            string intiteklogo = "IFI,IFP,IFM,INTITEK";
            var logo = intiteklogo.Contains(entityName) ? 
                _logoRepository.FindBy(new Specification<Logos>(l => l.EntityName.ToUpper() == "INTITEK")).FirstOrDefault() : 
                _logoRepository.FindBy(new Specification<Logos>(l => entityName.Contains(l.EntityName.ToUpper()))).FirstOrDefault();

            return logo != null ? logo.LogoName : defaultlogo.LogoName;
            
        }
    }
}
