using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public class Rythme
    {
        public DateTime Date { get; set; }
        public int NbPersonneSuccess { get; set; }
        public int SuccessWithoutTest { get; set; }
        public int TestNotNeeded { get; set; }
        public int Total { get; set; }
        public string Percent {
            get {
                if(NbPersonneSuccess==0 || Total == 0)
                {
                    return "0";
                }
                double p = (double)NbPersonneSuccess * 100 / Total;
                return String.Format("{0:0.##}", p).Replace(",", ".");
            }
        }
        public string CoordonneeXY
        {
            get
            {
                return "{" + string.Format("x:'{0}', y:{1}, valeur:'{2} / {3}'", Date.ToString("yyyy-MM-dd"), Percent, NbPersonneSuccess, Total) + "}" ;
            }
        }
       
    }
    public class ChartDataset
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Label { get; set; }
        public List<Rythme> Rythmes { get; set; }
        public ChartDataset()
        {
            this.Rythmes = new List<Rythme>();
        }
        public string Json
        {
            get
            {
                var index = this.Index + 1;
                if (index > Constante.COLORS_LENGTH)
                {
                    index = index - Constante.COLORS_LENGTH;
                }
                var color = string.Format("COLORS[{0}]", index-1);
                var data =  "[" + string.Join(",", Rythmes.Select(x => x.CoordonneeXY)) + "]";
                var ret = "{"
                    + string.Format("label: \"{0}\",", Label)
                    + string.Format("backgroundColor: {0},", color)
                    + string.Format("borderColor :{0},", color)
                    + string.Format("data :{0},", data)
                    + "pointRadius: 0" +
                 "}";
                return ret;
            }
        }
        
    }

}
