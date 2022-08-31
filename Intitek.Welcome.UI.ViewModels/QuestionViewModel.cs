using Intitek.Welcome.Service.Front;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels
{
    public class QuestionViewModel
    {
        public int InternalOrder { get; set; }
        public QuestionDTO Question { get; set; }
    }
}
