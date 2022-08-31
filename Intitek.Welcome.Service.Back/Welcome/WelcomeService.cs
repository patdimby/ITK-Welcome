using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using System;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class WelcomeService : BaseService, IWelcomeService
    {
        private WelcomeDataAccess _welcRepository;

        public WelcomeService(ILogger logger) : base(logger)
        {
            _welcRepository = new WelcomeDataAccess(uow);
        }

        public UpdateWelcomeMessageResponse UpdateWelcomeMessage(UpdateWelcomeMessageRequest request)
        {
            var response = new UpdateWelcomeMessageResponse();

            try
            {
                WelcomeMessage message = _welcRepository.FindBy(new Specification<WelcomeMessage>(w => w.ID_Lang == request.WelcomeMessage.WelcomeMessage.ID_Lang)).FirstOrDefault();
                message.Id = message.ID;

                message.Message = request.WelcomeMessage.WelcomeMessage.Message;
                _welcRepository.Save(message);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "SaveWelcomeMessage",
                    ServiceName = "WelcomeService",

                }, ex);
                throw ex;
            }
        }
    }
}
