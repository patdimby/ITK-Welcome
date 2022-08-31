using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public interface IMailTemplateService
    {
        GetAllMailTemplateResponse GetAll(GetAllMailTemplateRequest request);
        List<MailTemplateDTO> GetAllTemplateRemind();
        GetMailTemplateResponse Get(GetMailTemplateRequest request);
        DeleteMailTemplateResponse Delete(DeleteMailTemplateRequest request);
        SaveMailTemplateResponse Save(SaveMailTemplateRequest request);
        bool ContentIsValid(string content);
        List<string> GetUnrecognizedKeywords(string content);
        string GetMailPreview(string content);
    }
}
