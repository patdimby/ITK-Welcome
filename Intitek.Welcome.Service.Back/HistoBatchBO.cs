using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class HistoBatchBO : IHistorize
    {
        private HistoBatchsDataAccess _histo;
        public  List<BatchBO> _batches { get; set; }
        public HistoBatchBO(IUnitOfWork uow)
        {
            _histo = new HistoBatchsDataAccess(uow);
            _batches = new List<BatchBO>();
        }

        public void SaveHisto()
        {
            if (_batches.Any())
            {
                foreach (var batch in _batches)
                {
                    _histo.Add(new HistoBatchs()
                    {
                        ID = batch.ID,
                        ID_Batch = batch.ID_Batch,
                        Start = batch.Start,
                        Finish = batch.Finish,
                        ReturnCode = batch.ReturnCode,
                        Message = batch.Message,
                    });
                }
            }

        }

        public void SaveHisto(List<BatchBO> Histo)
        {
            throw new NotImplementedException();
        }
    }
}
