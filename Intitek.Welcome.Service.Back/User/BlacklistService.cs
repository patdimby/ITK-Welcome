using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class BlacklistService: BaseService, IBlacklistService
    {
        private readonly BlackListDataAccess _blacklistrepository;
        private readonly CityEntityBlackListedDataAccess _cityentityBLrepository;
        private readonly UserDataAccess _userrepository;
        public BlacklistService(ILogger logger) : base(logger)
        {
            _blacklistrepository = new BlackListDataAccess(uow);
            _cityentityBLrepository = new CityEntityBlackListedDataAccess(uow);
            _userrepository = new UserDataAccess(uow);

        }
        public GetAllBlackListResponse GetAll()
        {
            var query = this._blacklistrepository.RepositoryQuery.Select(x => new BlackListDTO()
            {
                Path = x.Path,
                DateCre = x.DateCre
            }).OrderBy(x=> x.DateCre);
            var query2 = this._cityentityBLrepository.RepositoryQuery.Select(x => new CityEntityBlacklistDTO()
            {
                City = x.City,
                Entity = x.Entity,
                DateCre = x.DateCre
            }).OrderBy(x=> x.City).ThenBy(x=>x.Entity);
            GetAllBlackListResponse response = new GetAllBlackListResponse();
            response.BlackLists = query.ToList();
            var cityEntityBlacklists = query2.ToList();
            foreach (var item in cityEntityBlacklists)
            {
                var queryUsers = this._userrepository.RepositoryQuery.Where(x => x.AgencyName.ToLower().Equals(item.City.ToLower()) && x.EntityName.ToLower().Equals(item.Entity.ToLower())).Select(x=> new { x.ID, x.Active});
                item.NbCollabActif = queryUsers.Count(x=> x.Active);
                item.NbCollabInactif = queryUsers.Count(x => !x.Active);
            }
            response.CityEntityBlacklists = cityEntityBlacklists;
            return response;
        }
        
        public GetBlackListResponse GetBlackList(GetBlackListRequest request)
        {
            var response = new GetBlackListResponse();
            try
            {
                var black = _blacklistrepository.RepositoryQuery.Where(x => x.Path.Equals(request.Path)).SingleOrDefault();
                if (black != null)
                {
                    response = new GetBlackListResponse()
                    {
                        BlackList = black
                    };

                }
                else
                {
                    response = new GetBlackListResponse()
                    {
                        BlackList = new Domain.BlackList()
                        {
                        }
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetBlackList",
                    ServiceName = "BlacklistService",

                }, ex);
                throw ex;
            }
        }
        public DeleteBlackListResponse DeleteBlackList(DeleteBlackListRequest request)
        {
            var response = new DeleteBlackListResponse();
            try
            {
                var black = _blacklistrepository.RepositoryQuery.Where(x => x.Path.Equals(request.Id)).SingleOrDefault();
                if (black != null) { 
                    _blacklistrepository.Remove(black);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteBlackList",
                    ServiceName = "BlacklistService",

                }, ex);
                throw ex;
            }
        }
        public bool IsBlackListExist(SaveBlackListRequest request)
        {
            if (!string.IsNullOrEmpty(request.Id))
            {
                var cat = this._blacklistrepository.RepositoryQuery.Where(x => !x.Path.Equals(request.Id) && x.Path.Equals(request.BlackList.Path)).FirstOrDefault();
                if (cat != null) return true;
            }
            else
            {
                var cat = this._blacklistrepository.RepositoryQuery.Where(x => x.Path.Equals(request.BlackList.Path)).FirstOrDefault();
                if (cat != null) return true;
            }
           return false;
        }
        public SaveBlackListResponse SaveBlackList(SaveBlackListRequest request)
        {
            var response = new SaveBlackListResponse();
            try
            {
                var catToSave = request.BlackList;

                if (string.IsNullOrEmpty(request.Id))
                {
                    _blacklistrepository.Save(catToSave);
                }
                else
                {
                    BlackList blackToUpdate = this._blacklistrepository.RepositoryQuery.Where(x => x.Path.Equals(request.Id)).FirstOrDefault();
                    if (blackToUpdate != null)
                    {
                        _blacklistrepository.Remove(blackToUpdate);
                        _blacklistrepository.Save(catToSave);
                    }
                    
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "SaveBlackList",
                    ServiceName = "BlacklistService",

                }, ex);
                throw ex;
            }
        }
    }
}
