using System;
using System.Linq;
using Intitek.Welcome.Infrastructure.Specification;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Front
{
    public class UserDocumentService : BaseService, IUserDocumentService
    {
        private readonly UserDocumentDataAccess _repository;
        
        public UserDocumentService(ILogger logger): base(logger)
        {
            _repository = new UserDocumentDataAccess(uow);
        }

        public CreateUserDocumentResponse CreateUserDocument(CreateUserDocumentRequest request)
        {
            var response = new CreateUserDocumentResponse();
            try
            {
               
                var uDocToCreate = new UserDocument()
                {
                    ID_Document = request.UserDocument.DocumentId,
                    ID_IntitekUser = request.UserDocument.UserId,
                    IsRead = request.UserDocument.IsRead ? DateTime.Now : (DateTime?)null,
                    IsApproved = request.UserDocument.IsApproved ? DateTime.Now : (DateTime?)null,
                    IsTested = request.UserDocument.IsTested ? DateTime.Now : (DateTime?)null,
                    UpdateDate = DateTime.Now
                };
                _repository.Add(uDocToCreate);
                response.UserDocument = uDocToCreate;
               

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "CreateUserDocument",
                    ServiceName = "UserDocumentService",

                }, ex);
                throw ex;
            }
        }

        public GetUserDocumentResponse GetUserDocument(GetUserDocumentRequest request)
        {
            var response = new GetUserDocumentResponse();
            try
            {
                var userDoc = _repository.FindBy(new Specification<UserDocument>(ud => ud.ID_Document == request.DocumentID && ud.ID_IntitekUser == request.UserID)).FirstOrDefault();
                response.UserDocument = new UserDocumentDTO() {
                    DocumentId = userDoc != null ? userDoc.ID_Document.Value : request.DocumentID,
                    UserId = userDoc != null ? userDoc.ID_IntitekUser.Value : request.UserID,
                    IsRead = userDoc != null ? userDoc.IsRead != null : false,
                    IsApproved = userDoc != null ? userDoc.IsApproved != null : false,
                    IsTested = userDoc != null ? userDoc.IsTested != null : false,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetUserDocument",
                    ServiceName = "UserDocumentService",

                }, ex);
                throw ex;
            }
           
        }
        public bool IsReadUserDocument(UpdateUserDocumentRequest request)
        {
            var uDocToUpdate = _repository.FindBy(new Specification<UserDocument>(ud => ud.ID_Document == request.UserDocument.DocumentId && ud.ID_IntitekUser == request.UserDocument.UserId)).FirstOrDefault();
            bool result = false;
            if (uDocToUpdate != null)
                result = uDocToUpdate.IsRead.HasValue;
            return result;
        }
        public UpdateUserDocumentResponse UpdateUserDocument(UpdateUserDocumentRequest request)
        {
            var response = new UpdateUserDocumentResponse();
            try
            {
                var uDocToUpdate = _repository.FindBy(new Specification<UserDocument>(ud => ud.ID_Document == request.UserDocument.DocumentId && ud.ID_IntitekUser == request.UserDocument.UserId)).FirstOrDefault();
                if (uDocToUpdate != null) {
                    uDocToUpdate.Id = uDocToUpdate.ID;
                    uDocToUpdate.IsRead =  request.UserDocument.IsRead ? DateTime.Now : (uDocToUpdate.IsRead == null ? (DateTime?)null : uDocToUpdate.IsRead);
                    uDocToUpdate.IsApproved = request.UserDocument.IsApproved ? DateTime.Now : (uDocToUpdate.IsApproved == null ? (DateTime?)null : uDocToUpdate.IsApproved);
                    uDocToUpdate.IsTested = request.UserDocument.IsTested ? DateTime.Now : (uDocToUpdate.IsTested == null ? (DateTime?)null : uDocToUpdate.IsTested);
                    uDocToUpdate.UpdateDate = DateTime.Now;
                    _repository.Save(uDocToUpdate);
                    response.UserDocument = uDocToUpdate;
                }
                else
                {
                    var uDocToCreate = new UserDocument()
                    {
                        ID_Document = request.UserDocument.DocumentId,
                        ID_IntitekUser = request.UserDocument.UserId,
                        UpdateDate = DateTime.Now,
                        IsRead = request.UserDocument.IsRead ? DateTime.Now : (DateTime?)null,
                        IsApproved = request.UserDocument.IsApproved ? DateTime.Now : (DateTime?)null,
                        IsTested = request.UserDocument.IsTested ? DateTime.Now : (DateTime?)null,
                   };
                    _repository.Add(uDocToCreate);
                    response.UserDocument = uDocToCreate;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "UpdateUserDocument",
                    ServiceName = "UserDocumentService",

                }, ex);
                throw ex;
            }
        }
        public Boolean CheckDocUserGrant(int idDocument, int idUser)
        {
            return _repository.CheckDocUserGrant(idDocument, idUser);
        }
    }
}
