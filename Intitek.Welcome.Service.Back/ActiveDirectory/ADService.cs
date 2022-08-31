using Intitek.Welcome.DataAccess.User;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Linq;
using System.Linq.Dynamic;

namespace Intitek.Welcome.Service.Back
{
    public class ADService: BaseService, IADService
    {
        private readonly ADDataAccess _adrepository;

        public ADService(ILogger logger) : base(logger)
        {
            _adrepository = new ADDataAccess(uow);
          

            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }

        public IQueryable<ADDTO> GetAllADAsQueryable(GetAllADRequest allrequest)
        {
            var request = allrequest.GridRequest;

            var query = this._adrepository.RepositoryQuery
               .Select(x => new ADDTO()
               {
                   ID = x.ID,
                   Name = x.Name,
                   Address = x.Address,
                   Domain = x.Domain,
                   Username = x.Username,
                   Password = x.Password,
                   ToBeSynchronized = x.ToBeSynchronized,
                   LastSynchronized = x.LastSynchronized
               });

            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.Search))
                {
                    string search = request.Search.ToLower();
                    string searchSansA = Utils.RemoveAccent(request.Search);
                    query = query.Where(x => x.Username.ToLower().Contains(search)
                            || EdmxFunction.RemoveAccent(x.Name).Contains(searchSansA)
                            || EdmxFunction.RemoveAccent(x.Domain).Contains(searchSansA)
                            || EdmxFunction.RemoveAccent(x.Address).Contains(searchSansA)
                    );
                }
                query = this.FiltrerQuery(request.Filtres, query);
            }

            return query;
        }

        public GetAllADResponse GetAll(GetAllADRequest allRequest)
        {
            string orderBy = "";
            var request = allRequest.GridRequest;
            if (string.IsNullOrEmpty(request.OrderColumn))
            {
                orderBy = "Name";
            }
            else
            {
                orderBy = request.OrderColumn + request.SortAscDesc;
            }
            var response = new GetAllADResponse();
            try
            {
                IQueryable<ADDTO> query = GetAllADAsQueryable(allRequest)
                    .OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                var list = query.ToList();
                response.ADs = list;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "ADService",

                }, ex);
                throw ex;
            }
        }

        public GetAllADResponse GetAll()
        {
            var response = new GetAllADResponse();
            try
            {
                var result = _adrepository.FindAll();

                return new GetAllADResponse() {
                    ADs = result.Select(ad => new ADDTO()
                    {
                        ID = ad.ID,
                        Name = ad.Name,
                        Address = ad.Address,
                        Domain = ad.Domain,
                        Username = ad.Username,
                        Password = ad.Password,
                        ToBeSynchronized = ad.ToBeSynchronized,
                        LastSynchronized = ad.LastSynchronized
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "ADService",

                }, ex);
                throw ex;
            }
        }

        public GetADResponse Get(GetADRequest request)
        {
            var response = new GetADResponse();
            try
            {
                var ad = _adrepository.FindBy(request.Id);

                if (ad != null)
                {
                    response.AD = new ADDTO()
                    {
                        ID = ad.ID,
                        Name = ad.Name,
                        Address = ad.Address,
                        Domain = ad.Domain,
                        Username = ad.Username,
                        ToBeSynchronized = ad.ToBeSynchronized,
                        LastSynchronized = ad.LastSynchronized
                    };

                    try
                    {
                        response.AD.Password = EncryptionHelper.Decrypt(ad.Password, request.PwdKey, request.PwdIV);
                    }
                    catch (Exception)
                    {
                        response.AD.Password = ad.Password;
                    }
                }
                else
                {
                    response.AD = new ADDTO()
                    {
                        Name = string.Empty,
                        Address = string.Empty,
                        Domain = string.Empty,
                        Username = string.Empty,
                        Password = string.Empty,
                        ToBeSynchronized = false
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
                    MethodName = "Get",
                    ServiceName = "ADService",

                }, ex);
                throw ex;
            }
        }

        public SaveADResponse Save(SaveADRequest request)
        {
            var response = new SaveADResponse();
            try
            {
                var adToSave = _adrepository.FindBy(request.AD.ID);
                if (adToSave == null)
                {
                    adToSave = new AD()
                    {
                        Name = request.AD.Name,
                        Address = request.AD.Address,
                        Domain = request.AD.Domain,
                        Username = request.AD.Username,
                        Password = EncryptionHelper.Encrypt(request.AD.Password, request.PwdKey, request.PwdIV),
                        ToBeSynchronized = request.AD.ToBeSynchronized,
                    };
                }
                else
                {
                    adToSave.Id = adToSave.ID;
                    adToSave.Name = request.AD.Name;
                    adToSave.Address = request.AD.Address;
                    adToSave.Domain = request.AD.Domain;
                    adToSave.Username = request.AD.Username;
                    adToSave.Password = EncryptionHelper.Encrypt(request.AD.Password, request.PwdKey, request.PwdIV);
                    adToSave.ToBeSynchronized = request.AD.ToBeSynchronized;
                    adToSave.LastSynchronized = request.fromBatch ? DateTime.Now : adToSave.LastSynchronized; //Ticket 2020
                }

                _adrepository.Save(adToSave);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Save",
                    ServiceName = "ADService",

                }, ex);
                throw ex;
            }
        }

        public DeleteADResponse Delete(DeleteADRequest request)
        {
            var response = new DeleteADResponse();
            try
            {
                var adToDelete = _adrepository.FindBy(request.Id);
                if (adToDelete != null)
                {
                    _adrepository.Remove(adToDelete);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Delete",
                    ServiceName = "ADService",

                }, ex);
                throw ex;
            }
        }
    }
}
