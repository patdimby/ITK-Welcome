﻿using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public interface IImportManagerService
    {
        ImportManager GetImportManager(IntitekUser manager);
    }
}