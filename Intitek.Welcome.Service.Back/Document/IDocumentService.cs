using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public interface IDocumentService
    {
        List<DocumentCategoryDTO> GetAllCategory(GetAllCategoryRequest request);
        List<DocumentSubCategoryDTO> GetAllSubCategory(int idLang, int idDefaultLang);
        List<String> GetAllVersion();
        int GetAllCount(GetAllDocumentRequest request);
        GetAllDocumentResponse GetAll(GetAllDocumentRequest request);
        GetAllDocumentNameResponse GetAllName(GetAllDocumentNameRequest request);
        GetDocumentResponse Get(GetDocumentRequest request);
        CreateDocumentResponse Create(CreateDocumentRequest request);
        UpdateDocumentResponse Update(UpdateDocumentRequest request);

        SaveDocumentResponse Save(SaveDocumentRequest request);
        DeleteDocumentResponse Delete(DeleteDocumentRequest request);

        CheckDocumentNameResponse CheckName(CheckDocumentNameRequest request);

        GetProfileDocumentResponse GetProfileDocument(GetProfileDocumentRequest request);
        GetEntityDocumentResponse GetEntityDocument(GetEntityDocumentRequest request);

        List<CategoryDTO> GetAllCategories(int idLang);
        SaveCategoryResponse SaveCategory(SaveCategoryRequest request);
        bool IsCategoryNameExist(SaveCategoryRequest request);

        GetCategoryResponse GetCategory(GetCategoryRequest request);
        DeleteCategoryResponse DeleteCategory(DeleteCategoryRequest request);
        GetDocumentVersionsResponse GetVersions(GetDocumentVersionsRequest request);

        GetDocumentVersionResponse GetDocumentVersion(GetDocumentVersionRequest request);

        ExistDocumentVersionLangResponse ExistDocumentVersionLang(ExistDocumentVersionLangRequest request);

        GetAllSubCategoryResponse GetAllSubcategory(GetAllSubCategoryRequest request);
        GetSubCategoryResponse GetSubcategory(GetSubCategoryRequest request);
        SaveSubCategoryResponse SaveSubcategory(SaveSubCategoryRequest request);
        DeleteSubCategoryResponse DeleteSubcategory(DeleteSubCategoryRequest request);
       

    }
}
