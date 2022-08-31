using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class ChangeEmailsService : BaseService, IChangeEmailsService
    {
        private readonly MailDataAccess _mailrepository;
        public ChangeEmailsService(ILogger logger) : base(logger)
        {
            _mailrepository = new MailDataAccess(uow);
        }
        public string SwapEmail(string oldEmail, string newEmail)
        {
            return _mailrepository.SwapEmail(oldEmail, newEmail);
        }
    }
}
