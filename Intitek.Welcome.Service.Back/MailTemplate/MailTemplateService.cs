using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using Intitek.Welcome.Infrastructure.Specification;

namespace Intitek.Welcome.Service.Back
{
    public class MailTemplateService : BaseService, IMailTemplateService
    {
        private readonly MailTemplateDataAccess _mailTemplateRepository;
        private readonly DocumentCategoryDataAccess _documentCategoryRepository;
        private readonly SubCategoryDataAccess _subCategoryRepository;
        private readonly IMailKeywordsService keywordsService;

        public MailTemplateService(ILogger logger) : base(logger)
        {
            _mailTemplateRepository = new MailTemplateDataAccess(uow);
            _documentCategoryRepository = new DocumentCategoryDataAccess(uow);
            _subCategoryRepository = new SubCategoryDataAccess(uow);
            keywordsService = new MailKeywordsService(new FileLogger());
        }

        public IQueryable<MailTemplateDTO> GetAllMailTemplateAsQueryable(GetAllMailTemplateRequest allrequest)
        {
            var request = allrequest.GridRequest;
            var query = this._mailTemplateRepository.RepositoryQuery
               .Select((mailTemplate) => new MailTemplateDTO()
               {
                   Id = mailTemplate.ID,
                   Name = mailTemplate.Name,
                   Comment = mailTemplate.Commentaire
               });

            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.Search))
                {
                    string search = request.Search.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(search)
                            || x.Comment.ToLower().Contains(search)
                    );
                }
                query = this.FiltrerQuery(request.Filtres, query);
            }

            return query;
        }

        public GetAllMailTemplateResponse GetAll(GetAllMailTemplateRequest allRequest)
        {
            string orderBy = "";
            var request = allRequest.GridRequest;
            if (string.IsNullOrEmpty(request.OrderColumn))
            {
                orderBy = "Name DESC";
            }
            else
            {
                orderBy = request.OrderColumn + request.SortAscDesc;
            }
            var response = new GetAllMailTemplateResponse();
            try
            {
                IQueryable<MailTemplateDTO> query = GetAllMailTemplateAsQueryable(allRequest)
                    .OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                var list = query.ToList();
                response.MailTemplates = list;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "MailTemplateService",

                }, ex);
                throw ex;
            }
        }
        public List<MailTemplateDTO> GetAllTemplateRemind()
        {
            List<MailTemplateDTO> ret = new List<MailTemplateDTO>();
            string orderBy = "Name DESC";
            var response = new GetAllMailTemplateResponse();
            try
            {
                IQueryable<MailTemplateDTO> query = this._mailTemplateRepository.RepositoryQuery.Where(x=> x.Name.StartsWith("TemplateRemindEntitie") || x.Name.StartsWith("TemplateRemindEmploy"))
               .Select(x => new MailTemplateDTO()
               {
                   Id = x.ID,
                   Name = x.Name,
                   Comment = x.Commentaire,
                   Content = x.Content,
                   Object = x.Object
               }).OrderBy(orderBy);
               ret = query.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAllTemplateRemind",
                    ServiceName = "MailTemplateService",

                }, ex);
                throw ex;
            }
            return ret;
        }

        public GetMailTemplateResponse Get(GetMailTemplateRequest request)
        {
            var response = new GetMailTemplateResponse();
            try
            {
                var mailTemplate = string.IsNullOrEmpty(request.TemplateName) ? 
                    _mailTemplateRepository.RepositoryTable.Include("DocumentCategory").Include("SubCategory").Where(x=> x.ID == request.Id).SingleOrDefault(): 
                    _mailTemplateRepository.RepositoryTable.Include("DocumentCategory").Include("SubCategory").Where(m => m.Name.Equals(request.TemplateName, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();

                if(mailTemplate != null)
                {
                    response.MailTemplate = new MailTemplateDTO()
                    {
                        Id = mailTemplate.ID,
                        Name = mailTemplate.Name,
                        Comment = mailTemplate.Commentaire,
                        Content = mailTemplate.Content,
                        Object = mailTemplate.Object,
                        IsGlobal = mailTemplate.isGlobal==1 ? true : false,
                        IsDocNoCategory = mailTemplate.isDocNoCategory == 1 ? true : false,
                        IsDocNoSubCategory = mailTemplate.isDocNoSubCategory == 1 ? true : false,
                        Categories = mailTemplate.DocumentCategory.ToList(),
                        SubCategories =mailTemplate.SubCategory.ToList()
                    };
                } else
                {
                    response.MailTemplate = new MailTemplateDTO()
                    {
                        Name = string.Empty,
                        Object = string.Empty,
                        Comment = string.Empty,
                        Content = string.Empty,
                        IsGlobal = true
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
                    ServiceName = "MailTemplateService",

                }, ex);
                throw ex;
            }
        }

        public DeleteMailTemplateResponse Delete(DeleteMailTemplateRequest request)
        {
            var response = new DeleteMailTemplateResponse();
            try
            {
                MailTemplate templateToDelete = _mailTemplateRepository.RepositoryTable.Include("DocumentCategory").Include("SubCategory").Include("HistoEmails").First(x => x.ID == request.Id);
                if (templateToDelete != null)
                {
                    templateToDelete.DocumentCategory.Clear();
                    templateToDelete.SubCategory.Clear();
                    templateToDelete.HistoEmails.Clear();
                    _mailTemplateRepository.Remove(templateToDelete);
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
                    ServiceName = "MailTemplateService",

                }, ex);
                throw ex;
            }
        }
        public SaveMailTemplateResponse Save(SaveMailTemplateRequest request)
        {
            var categs = new List<DocumentCategory>();
            var subcategs = new List<SubCategory>();
            
            if (request.MailTemplate.isGlobal ==0 && request.CategorySubCategories != null)
            {
                var tmpcategs = request.CategorySubCategories.Where(x => x.EndsWith("|categ")).Select(x => Int32.Parse(x.Replace("|categ", ""))).ToList();
                var tmpsubcategs = request.CategorySubCategories.Where(x => x.EndsWith("|subcateg")).Select(x => Int32.Parse(x.Replace("|subcateg", ""))).ToList();
                if (tmpcategs.Any())
                    categs = _documentCategoryRepository.FindBy(new Specification<DocumentCategory>(lg => tmpcategs.Contains(lg.ID))).ToList();
                if (tmpsubcategs.Any())
                    subcategs = _subCategoryRepository.FindBy(new Specification<SubCategory>(lg => tmpsubcategs.Contains(lg.ID))).ToList();
            }
            
            var response = new SaveMailTemplateResponse();
            try
            {
                var templateToSave = _mailTemplateRepository.RepositoryTable.Include("DocumentCategory").Include("SubCategory").SingleOrDefault(p => p.ID == request.MailTemplate.ID);
                if (templateToSave == null)
                {
                    templateToSave = new MailTemplate()
                    {
                        Date = DateTime.Now,
                        Name = request.MailTemplate.Name,
                        Commentaire = request.MailTemplate.Commentaire,
                        Content = request.MailTemplate.Content,
                        Object = request.MailTemplate.Object,
                        isGlobal = request.MailTemplate.isGlobal,
                        isDocNoCategory = request.MailTemplate.isDocNoCategory,
                        isDocNoSubCategory = request.MailTemplate.isDocNoSubCategory,
                        DocumentCategory = categs,
                        SubCategory = subcategs
                    };
                }
                else
                {
                    templateToSave.Id = templateToSave.ID;
                    templateToSave.Name = request.MailTemplate.Name;
                    templateToSave.Commentaire = request.MailTemplate.Commentaire;
                    templateToSave.Content = request.MailTemplate.Content;
                    templateToSave.Object = request.MailTemplate.Object;
                    templateToSave.isGlobal = request.MailTemplate.isGlobal;
                    templateToSave.isDocNoCategory = request.MailTemplate.isDocNoCategory;
                    templateToSave.isDocNoSubCategory = request.MailTemplate.isDocNoSubCategory;
                    //Delete DocumentCategory
                    List<DocumentCategory> lCategToDelete = new List<DocumentCategory>();
                    foreach (DocumentCategory cUpd in templateToSave.DocumentCategory)
                    {
                        if (!categs.Any(c => c.ID == cUpd.ID))
                        {
                            lCategToDelete.Add(cUpd);
                        }
                    }
                    foreach (DocumentCategory c in lCategToDelete)
                    {
                        templateToSave.DocumentCategory.Remove(c);
                    }
                    //Ajout DocumentCategory
                    foreach (DocumentCategory cAdd in categs)
                    {
                        if (!templateToSave.DocumentCategory.Any(c => c.ID == cAdd.ID))
                        {
                            templateToSave.DocumentCategory.Add(cAdd);
                        }
                    }
                    //Delete SubCategory
                    List<SubCategory> lSubCategToDelete = new List<SubCategory>();
                    foreach (SubCategory cUpd in templateToSave.SubCategory)
                    {
                        if (!subcategs.Any(c => c.ID == cUpd.ID))
                        {
                            lSubCategToDelete.Add(cUpd);
                        }
                    }
                    foreach (SubCategory c in lSubCategToDelete)
                    {
                        templateToSave.SubCategory.Remove(c);
                    }
                    //Ajout SubCategory
                    foreach (SubCategory cAdd in subcategs)
                    {
                        if (!templateToSave.SubCategory.Any(c=> c.ID == cAdd.ID))
                        {
                            templateToSave.SubCategory.Add(cAdd);
                        }
                    }
                }

                _mailTemplateRepository.Save(templateToSave);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Save",
                    ServiceName = "MailTemplateService",

                }, ex);
                throw ex;
            }
      
           
        }
        

        //Check if the keywords in the mail template's content are all valid
        public bool ContentIsValid(string content)
        {
            var allKeywords = keywordsService.GetAll().MailKeywords.Select(k => k.Code);

            string keywordTemp = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Equals('['))
                {
                    i++;
                    while (!content[i].Equals(']'))
                    {
                        keywordTemp += content[i];
                        i++;
                    }

                    if (!allKeywords.Contains(keywordTemp) && i < content.Length) return false;

                    keywordTemp = "";
                }
            }

            return true;
        }

        public List<string> GetUnrecognizedKeywords(string content)
        {
            List<string> result = new List<string>();

            var allKeywords = keywordsService.GetAll().MailKeywords.Select(k => k.Code);

            string keywordTemp = string.Empty;
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Equals('['))
                {
                    i++;
                    while (!content[i].Equals(']') && i < content.Length - 1)
                    {
                        keywordTemp += content[i];
                        i++;

                        if (i == content.Length - 1) keywordTemp = string.Empty;
                    }

                    if (!keywordTemp.Equals(string.Empty) && !allKeywords.Contains(keywordTemp))
                        result.Add(keywordTemp);

                    keywordTemp = "";
                }
            }

            return result;
        }

        public string GetMailPreview(string content)
        {
            string result = content;

            var allKeywords = keywordsService.GetAll().MailKeywords.Select(k => k.Code);

            List<string> fictiveDataKeys = KeywordFictiveData.FictiveDatas.Select(d => d.Keyword).ToList();

            string keywordTemp = string.Empty,
                   keywordMatch = string.Empty,
                   fictiveData = string.Empty;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].Equals('['))
                {
                    i++;
                    while (!result[i].Equals(']') && i < result.Length - 1)
                    {
                        keywordTemp += result[i];
                        i++;

                        if (i == result.Length - 1) keywordTemp = string.Empty;
                    }

                    if (!keywordTemp.Equals(string.Empty) && allKeywords.Contains(keywordTemp))
                    {
                        keywordMatch = allKeywords.Single(k => k.Equals(keywordTemp));
                        result = result.Remove(i - keywordTemp.Length - 1, keywordTemp.Length + 2);

                        fictiveData = fictiveDataKeys.Contains(keywordMatch) ? KeywordFictiveData.FictiveDatas.Single(d => d.Keyword.Equals(keywordMatch)).FictiveData : KeywordFictiveData.DefaultFictiveData;
                        result = result.Insert(i - keywordTemp.Length - 1, fictiveData);

                        i = i + (fictiveData.Length - (keywordTemp.Length + 2));
                    }

                    keywordTemp = "";
                }
            }

            return result;
        }
    }
}
