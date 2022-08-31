using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Specification;
using Intitek.Welcome.Infrastructure.Histo;

namespace Intitek.Welcome.Service.Front
{
    public class QcmService : BaseService, IQcmService
    {

        private readonly QcmDataAccess _qcmRepository;
        private readonly QcmLangDataAccess _qcmlangRepository;
        private readonly QuestionDataAccess _questionRepository;
        private readonly QuestionLangDataAccess _questionlangRepository;
        private readonly ReponseDataAccess _reponseRepository;
        private readonly ReponseLangDataAccess _reponselangRepository;
        private readonly UserDocumentDataAccess _userDocRepository;
        private readonly UserQcmDataAccess _userQcmRepository;
        private readonly UserQcmReponseDataAccess _userReponseRepository;
        private readonly Encoding utf8Encoding = Encoding.UTF8;

        public QcmService(ILogger logger) : base(logger)
        {
            _qcmRepository = new QcmDataAccess(uow);
            _qcmlangRepository = new QcmLangDataAccess(uow);
            _questionRepository = new QuestionDataAccess(uow);
            _questionlangRepository = new QuestionLangDataAccess(uow);
            _reponseRepository = new ReponseDataAccess(uow);
            _reponselangRepository = new ReponseLangDataAccess(uow);
            _userQcmRepository = new UserQcmDataAccess(uow);
            _userReponseRepository = new UserQcmReponseDataAccess(uow);
            _userDocRepository = new UserDocumentDataAccess(uow);
        }

        public CheckUserQcmResponse CheckUserQcm(CheckUserQcmRequest request)
        {

            try
            {
                var response = _userQcmRepository.FindBy(new Specification<UserQcm>(uq => uq.ID_IntitekUser == request.UserId && uq.ID_Qcm == request.QcmId && uq.ID_Document==request.DocumentID));
                return new CheckUserQcmResponse()
                {
                    Exist = response.Any(),
                    UserQcm = response.Select(uq => new UserQcmDTO() {
                        ID = uq.ID,
                        QcmID = uq.ID_Qcm,
                        UserID = uq.ID_IntitekUser,
                        IsPassed = uq.Score >= uq.ScoreMinimal
                    }).FirstOrDefault()

                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetQcm",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }

        }
        public GetQcmResponse GetQcm(GetQcmRequest request)
        {
           
            try
            {
                var qcm = _qcmRepository.FindBy(request.Id);
                var questions = _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == request.Id && q.inactif == 0))
                    .Select(q => new QuestionDTO()
                    {
                        Question = q,
                        QuestionTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == q.Id && ql.ID_Lang == request.IdLang)).FirstOrDefault(),
                        DefaultTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == q.Id && ql.ID_Lang != request.IdLang)).FirstOrDefault(),
                        //IsMultipleReponse = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && r.IsRight)).Count() > 1,
                        IsMultipleReponse = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && r.IsRight && r.inactif == null )).Count() > 1,
                        //Reponses = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && (!r.inactif.HasValue || !r.inactif.Value))).OrderBy(r => r.OrdreReponse)
                        Reponses = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && r.inactif == null )).OrderBy(r => r.OrdreReponse)
                        .Select(rep =>
                                         {
                                             var rTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang == request.IdLang)).FirstOrDefault();
                                             var dTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang != request.IdLang)).FirstOrDefault();
                                             return new ReponseDTO
                                             {
                                                 Reponse = rep,
                                                 ReponseTrad = rTrad,
                                                 DefaultTrad = dTrad,
                                                 IdLang = request.IdLang
                                             };
                                         })
                                         .RandomizeReponse()
                                         .ToList()
                    })
                    .Where(q => q.Reponses.Any());

                var response = new GetQcmResponse()
                {
                    Qcm = new QcmDTO()
                    {
                        Qcm = qcm,
                        QcmTrad = _qcmlangRepository.FindBy(new Specification<QcmLang>(q => q.ID_Qcm == request.Id && q.ID_Lang == request.IdLang)).FirstOrDefault(),
                        DefaultTrad = _qcmlangRepository.FindBy(new Specification<QcmLang>(q => q.ID_Qcm == request.Id && q.ID_Lang != request.IdLang)).FirstOrDefault(),
                        Questions = questions.ToList()
                    },

                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetQcm",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }

        }

        public GetQuestionResponse GetQuestion(GetQuestionRequest request)
        {
            var response = new GetQuestionResponse();
            try
            {

                var question = _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == request.QcmId && q.Id == request.Id && q.inactif == 0))
                    .Select(q => new QuestionDTO()
                    {
                        Question = q,
                        QuestionTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == q.Id && ql.ID_Lang == request.IdLang)).FirstOrDefault(),
                        DefaultTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == q.Id && ql.ID_Lang != request.IdLang)).FirstOrDefault(),
                        //Reponses = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && (!r.inactif.HasValue || !r.inactif.Value))).OrderBy(r => r.OrdreReponse)
                        Reponses = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && r.inactif == null)).OrderBy(r => r.OrdreReponse)
                        .Select(rep =>
                            {
                                var rTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang == request.IdLang)).FirstOrDefault();
                                return new ReponseDTO
                                {
                                    Reponse = rep,
                                    ReponseTrad = rTrad,
                                    IdLang = request.IdLang
                                };
                            })
                            .ToList()
                    })
                    .FirstOrDefault();

                response = new GetQuestionResponse()
                {
                    Question = question
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetQuestion",
                    ServiceName = "QcmService",
                }, ex);
                throw ex;
            }

        }

        public GetUserQcmResultResponse GetUserQcmResult(GetUserQcmResultRequest request, int userId)
        {
            var response = new GetUserQcmResultResponse();
            try
            {
                var userQcm = _userQcmRepository.FindBy(new Specification<UserQcm>(uq => uq.ID_Qcm == request.UserQcm.QcmID && uq.ID_IntitekUser == request.UserQcm.UserID)).FirstOrDefault();
                if (userQcm != null)
                {
                    var userReponse = _userReponseRepository.FindBy(new Specification<UserQcmReponse>(ur => ur.ID_UserQcm == userQcm.ID));
                    
                    var results = userReponse.Select(                           
                             ur =>
                                 {
                                     var reponseTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Lang == request.IdLang && rl.ID_Reponse == ur.ID_Reponse)).FirstOrDefault();
                                     var defaultReponseTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Lang != request.IdLang && rl.ID_Reponse == ur.ID_Reponse)).FirstOrDefault();
                                     return new UserQcmReponseDTO()
                                     {
                                         //
                                         //QuestionId = _reponseRepository.FindBy(new Specification<Reponse>(r => r.Id == ur.ID_Reponse)).FirstOrDefault().ID_Question.Value,
                                         QuestionId = _reponseRepository.FindBy(new Specification<Reponse>(r => r.Id == ur.ID_Reponse && r.inactif == null )).FirstOrDefault().ID_Question.Value,
                                         ReponseId = ur.ID_Reponse,
                                         Reponse = reponseTrad == null ? defaultReponseTrad.Texte : reponseTrad.Texte,
                                         //UserId = ur.ID_UserQcm,
                                         UserId = userId,
                                         UserQcmId = ur.ID_UserQcm,
                                         IsCorrect = _reponseRepository.FindBy(ur.ID_Reponse).IsRight
                                     };
                                 }
                             ).ToList();
                    var qcm = _qcmRepository.FindBy(request.UserQcm.QcmID);
                   

                    response = new GetUserQcmResultResponse()
                    {
                        Qcm = qcm,
                        UserReponses = results
                        .GroupBy(q => q.QuestionId)
                        .Select(r => new UserQcmReponseDTO()
                        {
                            IsCorrect = r.All(i => i.IsCorrect) && r.Count() == _questionRepository.CountRightResponse(r.Key),
                            QuestionId = r.Key,
                            Question = new QuestionDTO()
                            {
                                Question = _questionRepository.FindBy(r.Key),
                                QuestionTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Lang == request.IdLang && ql.ID_Question == r.Key)).FirstOrDefault(),
                                DefaultTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Lang != request.IdLang && ql.ID_Question == r.Key)).FirstOrDefault()
                            },
                            Reponse = string.Join(", ", r.Select(i => i.Reponse))
                            
                        }).OrderBy(r => r.Question.Question.OrdreQuestion).ToList()
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
                    MethodName = "GetUserQcmResult",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public SaveUserQcmResponse SaveUserQcm(SaveUserQcmRequest request)
        {
            var response = new SaveUserQcmResponse();
            var ActionsFO = new List<ActionFO>();
            try
            {
                var userQcmToRemove = _userQcmRepository.FindBy(new Specification<UserQcm>(uq => uq.ID_IntitekUser == request.UserQcm.UserID && uq.ID_Qcm == request.UserQcm.QcmID)).FirstOrDefault();
                if (userQcmToRemove != null)
                {
                    var userRepQcmToRemove = _userReponseRepository.FindBy(new Specification<UserQcmReponse>(uq => uq.ID_UserQcm == userQcmToRemove.ID));
                    if (userRepQcmToRemove.Any())
                    {
                            _userReponseRepository.RemoveAll(userRepQcmToRemove);
                       
                    }
                    _userQcmRepository.Remove(userQcmToRemove);

                }

                // Calculate Score (request.UserQcm.Reponses)
                var userReponses = request.UserQcm.Reponses.Select(
                             ur => new UserQcmReponseDTO()
                             {
                                 //QuestionId = _reponseRepository.FindBy(new Specification<Reponse>(r => r.Id == ur.ID_Reponse)).FirstOrDefault().ID_Question.Value,
                                 QuestionId = _reponseRepository.FindBy(new Specification<Reponse>(r => r.Id == ur.ID_Reponse && r.inactif == null )).FirstOrDefault().ID_Question.Value,
                                 ReponseId = ur.ID_Reponse,
                                 UserId = ur.ID_UserQcm,
                                 UserQcmId = ur.ID_UserQcm,
                                 IsCorrect = _reponseRepository.FindBy(ur.ID_Reponse).IsRight,
                                 
                             })
                             .GroupBy(q => q.QuestionId).Select(r => new UserQcmReponseDTO()
                             {
                                 IsCorrect = r.All(i => i.IsCorrect) && r.Count() == _questionRepository.CountRightResponse(r.Key),
                                 QuestionId = r.Key,
                                 Question = new QuestionDTO() {
                                     Question =_questionRepository.FindBy(r.Key)                                   
                                 }
                             })
                             .OrderBy(r => r.Question.Question.OrdreQuestion).ToList();

                var qmScore = userReponses.Sum(usr => usr.IsCorrect ? 1 : 0);

                var userQcmToAdd = new UserQcm()
                {
                    ID_IntitekUser = request.UserQcm.UserID,
                    ID_Qcm = request.UserQcm.QcmID,
                    DateCre = request.UserQcm.DateCre,
                    DateFin = DateTime.Now,
                    Score = userReponses.Sum(usr => usr.IsCorrect ? 1 : 0),
                    NbQuestions = request.UserQcm.NbQuestions,
                    ScoreMinimal = request.UserQcm.QcmScoreMinimal,
                    Version = request.UserQcm.DocumentVersion,
                    ID_Document = request.UserQcm.DocumentID
                };

               
                foreach (var rep in request.UserQcm.Reponses)
                {
                    userQcmToAdd.UserQcmReponse.Add(new UserQcmReponse()
                     {
                         ID_Reponse = rep.ID_Reponse,
                         //ID_UserQcm = userQcm.ID,
                         DateCre = DateTime.Now,
                     });
                }                

                _userQcmRepository.Add(userQcmToAdd);

                var userDocToUpdate = _userDocRepository.FindBy(new Specification<UserDocument>(ud => ud.ID_Document == request.UserQcm.DocumentID && ud.ID_IntitekUser == request.UserQcm.UserID)).FirstOrDefault();
                if (userDocToUpdate != null)
                {
                    userDocToUpdate.Id = userDocToUpdate.ID;
                    userDocToUpdate.IsTested = null;
                    userDocToUpdate.IsRead = DateTime.Now.Date;
                    userDocToUpdate.IsApproved = DateTime.Now.Date;
                    userDocToUpdate.IsTested = null;
                    if (qmScore >= request.UserQcm.QcmScoreMinimal)
                    {
                       
                        userDocToUpdate.IsTested = DateTime.Now.Date;

                    }
                   
                    _userDocRepository.Save(userDocToUpdate);
                }

                ActionsFO.Add(new ActionFO()
                {
                    ID_User = request.UserQcm.UserID,
                    ID_Qcm = request.UserQcm.QcmID,
                    ID_Document = request.UserQcm.DocumentID,
                    Version = !string.IsNullOrEmpty(request.UserQcm.DocumentVersion) ? request.UserQcm.DocumentVersion: string.Empty,
                    Score = qmScore,
                    ScoreMinimal = request.UserQcm.QcmScoreMinimal,
                    DateCre = request.UserQcm.DateCre,
                    DateFin = DateTime.Now,

                });

                _histoActionFO = new HistoActionFO(base.uow, ActionsFO);
                _histoActionFO.SaveHisto();
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "SaveUserQcm",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

    
    }
}
