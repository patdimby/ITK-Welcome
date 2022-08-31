using System;

namespace Intitek.Welcome.Service.Front
{
    public interface IUserDocumentService
    {
        bool IsReadUserDocument(UpdateUserDocumentRequest request);
        UpdateUserDocumentResponse UpdateUserDocument(UpdateUserDocumentRequest request);

        CreateUserDocumentResponse CreateUserDocument(CreateUserDocumentRequest request);

        GetUserDocumentResponse GetUserDocument(GetUserDocumentRequest request);
        Boolean CheckDocUserGrant(int idDocument, int idUser);
    }
}
