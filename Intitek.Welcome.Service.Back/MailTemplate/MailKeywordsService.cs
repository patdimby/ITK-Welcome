using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class MailKeywordsService : BaseService, IMailKeywordsService
    {
        private readonly MailKeywordsDataAccess _mailKeywordsRepository;

        public MailKeywordsService(ILogger logger) : base(logger)
        {
            _mailKeywordsRepository = new MailKeywordsDataAccess(uow);
        }

        public GetAllMailKeywordsResponse GetAll()
        {
            var response = new GetAllMailKeywordsResponse();
            try
            {
                var mailKeywords = _mailKeywordsRepository.FindAll().Select(keyword => new MailKeywordsDTO()
                {
                    Code = keyword.Code,
                    Description = keyword.Description
                });

                response.MailKeywords = mailKeywords.ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "MailKeywordsService",

                }, ex);
                throw ex;
            }
        }
    }

}
