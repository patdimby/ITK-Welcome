﻿using Intitek.Welcome.Infrastructure.Domain;
using Intitek.Welcome.Domain;
namespace Intitek.Welcome.DataAccess
{
    public interface IUserDataAccess : IRepository<IntitekUser, int>
    {
    }
}
