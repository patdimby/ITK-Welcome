using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Front
{
    public class HistoActionFO : IHistorize
    {
        private HistoUserQcmDocVersionDataAccess _histo;

        public List<ActionFO> _actions { get; set; }
        public HistoActionFO(IUnitOfWork uow, List<ActionFO> actions)
        {
            _histo = new HistoUserQcmDocVersionDataAccess(uow);
            _actions = actions;
        }

        public void SaveHisto()
        {
            if (_actions.Any())
            {
                foreach (var action in _actions)
                {
                    _histo.Add(new HistoUserQcmDocVersion()
                    {
                        ID_IntitekUser = action.ID_User,
                        ID_Qcm = action.ID_Qcm,
                        ID_Document = action.ID_Document,
                        Version = action.Version,
                        Score = action.Score,
                        ScoreMinimal = action.ScoreMinimal,
                        DateCre = action.DateCre,
                        DateFin = action.DateFin,
                    });
                }
            }
        }
    }
}