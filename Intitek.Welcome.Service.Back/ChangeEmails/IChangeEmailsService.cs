using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public interface IChangeEmailsService
    {
        string SwapEmail(string oldEmail, string newEmail);
    }
}
