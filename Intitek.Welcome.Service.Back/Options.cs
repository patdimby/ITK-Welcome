using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intitek.Welcome.Service.Back
{
    public class IntOption
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class StringOption
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class Options
    {
        public const int True = 1;
        public const int False = 2 ;
    }
}