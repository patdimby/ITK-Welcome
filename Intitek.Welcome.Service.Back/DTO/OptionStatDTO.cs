using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public class OptionStatDTO
    {
        public int Nb { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public List<OptionStatDTO> SubList { get; set; }
        public bool IsManager { get; set; }
    }
}
