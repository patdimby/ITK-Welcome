using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.DataAccess
{
    public interface IMailDataAccess
    {
        string SwapEmail(string oldEmail, string newEmail);
    }
}
