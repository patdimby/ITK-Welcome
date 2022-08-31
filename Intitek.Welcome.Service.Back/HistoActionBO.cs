using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.Service.Back
{
    public class HistoActionBO : IHistorize, IDisposable
    {
        private HistoActionsDataAccess _histoAction;
        private List<ActionBO> _actions { get; set; }
        public HistoActionBO(IUnitOfWork uow, List<ActionBO> actions)
        {
            _histoAction = new HistoActionsDataAccess(uow);
            _actions = actions;
        }

        public void SaveHisto()
        {
            if (_actions.Any())
            {
                foreach (var action in _actions)
                {
                    _histoAction.Add(new HistoActions()
                    {
                        ID_Object = action.ID_Object,
                        ID_IntitekUser = action.ID_User,
                        ObjectCode = action.ObjectCode,
                        Action = action.Action,
                        DateAction = DateTime.Now,
                        LinkedObjects = action.LinkObjects

                    });
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
