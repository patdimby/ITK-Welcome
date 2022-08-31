using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class HistoEmailBO : IHistorize
    {
        private HistoEmailsDataAccess _histo;
        public List<EmailBO> Emails { get; set; }
        public HistoEmailBO(IUnitOfWork uow)
        {
            _histo = new HistoEmailsDataAccess(uow);
            Emails = new List<EmailBO>();
        }

        public void SaveHisto()
        {
            if (Emails.Any())
            {
                foreach (var email in Emails)
                {
                    _histo.Add(new Domain.HistoEmails()
                    {
                        Id_IntitekUser = email.Id_IntitekUser,
                        Id_MailTemplate = email.Id_MailTemplate,
                        Date = DateTime.Now,
                        Message = email.Message,
                        ID_Batch = email.ID_Batch,
                    });
                }
            }

        }

        public void SaveHisto(List<EmailBO> emails)
        {
            throw new NotImplementedException();
        }
    }
}
