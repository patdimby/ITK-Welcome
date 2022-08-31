using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public class GetRythmeResponse
    {
        public List<StatistiquesDTO> _Lststat { get; set; }
        public DateTime _Periode { get; set; }
        public int _NbDay { get; set; }
        private readonly GetStatsRequest _StatsRequest;
        public List<ChartDataset> Datasets { get; set; }
        public List<DocumentDTO> Documents { get; set; }

        public GetRythmeResponse(GetStatsRequest statsRequest)
        {
            this._StatsRequest = statsRequest;
            this.Datasets = new List<ChartDataset>();
            this._Periode = statsRequest.Periode.Value;
            this._NbDay = statsRequest.NbDay;
         }
        private Rythme GetRythme(int docId, DateTime date)
        {
            Rythme rt = new Rythme() { Date = date};
            if (this._Lststat != null)
            {
                var statDocs = this._Lststat.Where(x => x.ID == docId).ToList();
                List<int> userIds = statDocs.Select(x => x.IdUser).Distinct().ToList();
                var total = userIds.Count();
                rt.Total = total;
                int without_test = statDocs.Where(x => userIds.Contains(x.IdUser) && (x.IsTested == null && x.Test == 0)) .Select(x => x.IdUser).Distinct().Count();
                rt.TestNotNeeded = without_test;
                int accepted_without_test= statDocs.Where(x => userIds.Contains(x.IdUser)
                         && (x.IsRead.HasValue && x.IsRead <= date)
                         && ((x.Approbation.HasValue && x.Approbation == 1) && x.IsApproved.HasValue && x.IsApproved <= date))
                         .Select(x => x.IdUser).Distinct().Count();
                rt.SuccessWithoutTest = accepted_without_test;
                int nbSucces = statDocs.Where(x => userIds.Contains(x.IdUser)
                        && (x.IsRead.HasValue && x.IsRead <= date)
                        && ((x.Approbation.HasValue && x.Approbation == 1) && x.IsApproved.HasValue && x.IsApproved <= date)
                        && ((x.Test.HasValue && x.Test == 1 ) && x.IsTested.HasValue && x.IsTested <= date))
                        .Select(x => x.IdUser).Distinct().Count();
               if(without_test == total)
               {
                    nbSucces = total - accepted_without_test;
               }
                rt.NbPersonneSuccess = nbSucces;
            }
           
            return rt;
        }

        public void CorrectDataSet()        {
            DateTime dtDebut = _Periode.AddDays(-_NbDay + 1);         
            if (_Lststat != null)
            {
                List<int> docIds = _Lststat.Select(x => x.ID).Distinct().ToList();
                var documents = Documents.Where(x => docIds.Contains(x.ID));
                var index = 0;
                foreach (var doc in documents)
                {
                    var dataset = new ChartDataset()
                    {
                        Id = doc.ID,
                        Index = index,
                        Label = doc.Name,
                    };
                    for (int i = 0; i < this._NbDay; i++)
                    {
                        var date = dtDebut;
                        if (i > 0)
                        {
                            date = dtDebut.AddDays(i);
                        }
                        dataset.Rythmes.Add(GetRythme(doc.ID, date));
                    }
                    this.Datasets.Add(dataset);
                    index++;
                }
            }
            
        }
        public List<ChartDataset> GetDatasets(List<StatistiquesDTO> lststat)
        {
            DateTime dtDebut = _Periode.AddDays(-this._NbDay+1);
            _Lststat = lststat;
            if (_Lststat != null)
            {
                List<int> docIds = this._Lststat.Select(x => x.ID).Distinct().ToList();
                var documents = Documents.Where(x=> docIds.Contains(x.ID));
                var index = 0;
                foreach (var doc in documents)
                {
                    var dataset = new ChartDataset()
                    {
                        Id = doc.ID,
                        Index = index,
                        Label = doc.Name, 
                    };
                    for (int i = 0; i < this._NbDay; i++)
                    {
                        var date = dtDebut;
                        if (i > 0)
                        {
                            date = dtDebut.AddDays(i);
                        }
                        dataset.Rythmes.Add(GetRythme(doc.ID, date));
                    }
                    this.Datasets.Add(dataset);
                    index++;
                }
            }
            return Datasets;
        }
    }
}
