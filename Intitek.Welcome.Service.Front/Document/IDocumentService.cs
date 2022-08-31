using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Front
{
    public interface IDocumentService
    {
        GetDocumentResponse GetDocument(GetDocumentRequest request);
        List<DocumentCategoryDTO> GetAllCategory(int idLang, int idDefaultLang);
        List<DocumentSubCategoryDTO> GetAllSubCategory(int idLang, int idDefaultLang);
        List<String> GetAllVersion();
        int GetAllDocumentByUserLoginCount(GetUserDocumentRequest request);
        int GetReadedDocumentByUserLoginCount(GetUserDocumentRequest request);
        int GetNoActionDocumentByUserLoginCount(GetUserDocumentRequest request);
        List<DocumentDTO> GetAllDocumentByUserLogin(GetUserDocumentRequest request);
        List<DocumentDTO> GetReadedDocumentByUserLogin(GetUserDocumentRequest request);
        List<DocumentDTO> GetNoActionDocumentByUserLogin(GetUserDocumentRequest request);
        UserQcm FindByUserQcm(int id);
        HistoUserQcmDocVersion FindByHistoUserQcm(int id);
        //List<UserQcmDTO> FindAllUserQcmSuccess(int idUser, int maxDisplay, int idLang, int defaulltIdLang);
        List<HistoUserQcmDocVersionDTO> FindAllUserQcmSuccess(int idUser, int maxDisplay, int idLang, int defaulltIdLang);
        GetUserDocumentResponse GetAllListDocumentByUser(GetUserDocumentRequest request1, GetUserDocumentRequest request2, GetUserDocumentRequest request3);
    }
}
