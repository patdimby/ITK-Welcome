using System;
using System.Linq;
using System.Web.Mvc;
using Intitek.Welcome.Service.Front;


using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.UI.ViewModels;
using Intitek.Welcome.UI.Web.Filters;

namespace Intitek.Welcome.UI.Web.Controllers
{
    [Authorize]
    [UrlRestrictAccessFilter]
    public class QcmController : CommunController
    {
        private readonly IQcmService _qcmService;
        private readonly Service.Back.ILangService _langService;

        public QcmController()
        {
            _qcmService = new QcmService(new FileLogger());
            _langService = new Service.Back.LangService(new FileLogger());
        }

        // GET: Qcm
        public ActionResult Index(int Id, int DocumentId, string DocumentVersion = "")
        {
            var lang = _langService.Get(new Service.Back.GetLangRequest() { Id = GetIdLang() }).Langue;
            var response = _qcmService.GetQcm(new GetQcmRequest() {
                Id = Id,
                IdLang = lang != null ? lang.ID: GetDefaultLang(),
            });
            var qcmVM = new QcmViewModel()
            {
                DocumentID = DocumentId,
                DocumentVersion = DocumentVersion,            
                DateCre = DateTime.Now,
                Qcm = response.Qcm,
                Questions = response.Qcm.Questions
                            .RandomizeQuestion(response.Qcm.Qcm.NbQuestions)                            
                            .OrderBy(q => q.Question.OrdreQuestion)
                            .Select((q, idx) => new QuestionViewModel() { Question = q, InternalOrder = idx + 1})
                            .ToList()
            };
            
            return View(qcmVM);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Save(UserQcmViewModel model)
        {
            var request = new SaveUserQcmRequest()
            {
                UserQcm = new UserQcmDTO()
                {
                    DocumentID = model.DocumentID,
                    DocumentVersion = model.DocumentVersion,
                    QcmID = model.QcmID,
                    QcmScoreMinimal = _qcmService.GetQcm( new GetQcmRequest() { Id = model.QcmID }).Qcm.Qcm.NoteMinimal.HasValue ?
                        _qcmService.GetQcm(new GetQcmRequest() { Id = model.QcmID }).Qcm.Qcm.NoteMinimal.Value : 0,
                    UserID = GetUserIdConnected(),
                    NbQuestions = model.NbQuestions,
                    DateCre = model.DateCre,
                    Reponses = model.Reponses.Select(rp => new Domain.UserQcmReponse()
                    {
                        ID_Reponse = rp
                    }).ToList()
                }
            };

            
            var response = _qcmService.SaveUserQcm(request);

            return Json(new { success = true, data = request.UserQcm }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult CheckUserQcm(UserQcmViewModel model)
        {
            var request = new CheckUserQcmRequest()
            {                
                QcmId = model.QcmID,
                UserId = GetUserIdConnected(),
                DocumentID = model.DocumentID
            };

            var response = _qcmService.CheckUserQcm(request);

            return Json(new { success = response.Exist, data = response.UserQcm }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Result(int qcmId, int userId)
        {
            var lang = _langService.Get(new Service.Back.GetLangRequest() { Id = GetIdLang() }).Langue;
            var request = new GetUserQcmResultRequest()
            {
                IdLang = lang != null ? lang.ID : GetDefaultLang(),
                UserQcm = new UserQcmDTO()
                {
                    QcmID = qcmId,
                   
                    UserID = userId
                }
            };
            //var response = _qcmService.GetUserQcmResult(request);
            var response = _qcmService.GetUserQcmResult(request, userId);
            var userResponses = response.UserReponses.OrderBy(q => q.Question.Question.OrdreQuestion).Select((ur, idx) => new UserReponseQcmViewModel()
            {
                IsCorrect = ur.IsCorrect,
                QuestionId = ur.QuestionId,
                QuestionOrder = idx + 1,
                UserReponse = ur.Reponse,
                TexteJustification = ur.Question.QuestionTrad == null ? ur.Question.DefaultTrad.TexteJustification : ur.Question.QuestionTrad.TexteJustification,
                TexteQuestion = ur.Question.QuestionTrad == null ? ur.Question.DefaultTrad.TexteQuestion : ur.Question.QuestionTrad.TexteQuestion,
            });
           
            var qcm = _qcmService.GetQcm(new GetQcmRequest()
            {
                Id = qcmId,
                IdLang = lang != null ? lang.ID : GetDefaultLang(),
            });

            var resultatVM = new UserQcmResultViewModel()
            {
                QcmId = request.UserQcm.QcmID,
                QcmName = qcm.Qcm.QcmTrad == null ? qcm.Qcm.DefaultTrad.QcmName : qcm.Qcm.QcmTrad.QcmName,
                QcmNombreQuestions = qcm.Qcm.Qcm.NbQuestions,
                QcmMinScore = qcm.Qcm.Qcm.NoteMinimal.Value,
                QcmUserScore = userResponses.Where(ur => ur.IsCorrect).Count(),
                QcmRightPercent = userResponses.Sum(ur => ur.IsCorrect ? 1 : 0), // qcm.Qcm.Qcm.NbQuestions,
                QcmWrongPercent = userResponses.Sum(ur => !ur.IsCorrect ? 1: 0), // qcm.Qcm.Qcm.NbQuestions,
                IsQcmPassed = userResponses.Where(ur => ur.IsCorrect).Count() >= qcm.Qcm.Qcm.NoteMinimal.Value,
                UserReponses = userResponses.ToList()

            };

            return View(resultatVM);
        }
    }
}