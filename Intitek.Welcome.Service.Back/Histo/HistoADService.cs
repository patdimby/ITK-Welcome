using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public class HistoADService : BaseService, IHistoADService
    {
        private readonly HistoADDataAccess _histoADRepository;
        public HistoADService(ILogger logger) : base(logger)
        {
            _histoADRepository = new HistoADDataAccess(uow);
        }

        public List<HistoADDTO> getAllMonths() {
            List<HistoADDTO> response = _histoADRepository.GetMonths().Select(list =>
            {
                return new HistoADDTO
                {
                    Mois = list.Mois
                };
            }).ToList();
            return response;
        }
    }
}
