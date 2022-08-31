using Intitek.Welcome.DataAccess;

using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;

namespace Intitek.Welcome.Service.Back
{
    public class QcmService : BaseService, IQcmService
    {

        private readonly QcmDataAccess _qcmRepository;
        private readonly QcmLangDataAccess _qcmlangRepository;
        private readonly UserQcmDataAccess _userQcmRepository;
        private readonly UserDataAccess _userRepository;
        private readonly QuestionDataAccess _questionRepository;
        private readonly QuestionLangDataAccess _questionlangRepository;
        private readonly ReponseDataAccess _reponseRepository;
        private readonly ReponseLangDataAccess _reponselangRepository;
        private readonly DocumentDataAccess _docRepository;
        private readonly LangDataAccess _langRepository;

        public QcmService(ILogger logger) : base(logger)
        {
            _qcmRepository = new QcmDataAccess(uow);
            _qcmlangRepository = new QcmLangDataAccess(uow);
            _userQcmRepository = new UserQcmDataAccess(uow);
           _userRepository = new UserDataAccess(uow);
            _questionRepository = new QuestionDataAccess(uow);
            _questionlangRepository = new QuestionLangDataAccess(uow);
            _reponseRepository = new ReponseDataAccess(uow);
            _reponselangRepository = new ReponseLangDataAccess(uow);
            _docRepository = new DocumentDataAccess(uow);
            _langRepository = new LangDataAccess(uow);

            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }
        public int GetAllCount(GetAllQcmRequest request)
        {
            return GetAllQueryable(request).Count();

        }
        public override string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            if ("Name".Equals(columnFilter.ColumnName) && !string.IsNullOrEmpty(filterValue))
            {
                columnFilter.Field = "EdmxFunction.RemoveAccent(Name)";
                columnFilter.FilterValue = Utils.RemoveAccent(filterValue);
            }
            return columnFilter.GetOperator(type, index, filterValue);
        }
        public IQueryable<QcmDTO> GetAllQueryable(GetAllQcmRequest allrequest)
        {
            var query = _qcmRepository.RepositoryQuery.Select(qcm => new QcmDTO()
            {
                Id = qcm.Id,
                DateCreation = qcm.DateCreation,
                Inactif = qcm.Inactif == 1 ? true : false,
                IsUpdatable = !qcm.Document.Any(d=> d.IdQcm == qcm.Id),
                NbQuestions = qcm.NbQuestions,
                NoteMinimal = qcm.NoteMinimal,
                Name = qcm.QcmLang.Where(i => i.ID_Lang == allrequest.IdLang).FirstOrDefault() != null ? 
                        qcm.QcmLang.Where(i => i.ID_Lang == allrequest.IdLang).FirstOrDefault().QcmName :
                        qcm.QcmLang.Where(i => i.ID_Lang != allrequest.IdLang).FirstOrDefault().QcmName,
                IsDefaultTradName = qcm.QcmLang.Where(i => i.ID_Lang == allrequest.IdLang).FirstOrDefault() == null

            });
            var request = allrequest.GridRequest;
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.Search))
                {
                    string search = request.Search.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(search));
                }
                query = this.FiltrerQuery(request.Filtres, query);
            }
            return query;
        }

        public GetAllQcmResponse GetAll(GetAllQcmRequest allrequest)
        {
            var request = allrequest.GridRequest;
            string orderBy = request.OrderColumn + request.SortAscDesc;
            var query = GetAllQueryable(allrequest).OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
            try
            {
                return new GetAllQcmResponse()
                {
                    Qcms = query.ToList()
                };

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public GetAllQcmResponse GetAll(int IdLang = 0)
        {

            var query = _qcmRepository.FindAll().Where(q => q.Inactif == 0);

            try
            {
                return new GetAllQcmResponse()
                {
                    Qcms = query.Select(q =>
                    {
                        var qcmTrad = _qcmlangRepository.FindBy(new Specification<QcmLang>(qt => qt.ID_Qcm == q.Id && qt.ID_Lang == IdLang)).FirstOrDefault();
                        var defaultTrad = _qcmlangRepository.FindBy(new Specification<QcmLang>(qt => qt.ID_Qcm == q.Id && qt.ID_Lang != IdLang)).FirstOrDefault();
                        return new QcmDTO()
                        {
                            Id = q.Id,
                            Name = qcmTrad != null ? qcmTrad.QcmName : (defaultTrad != null ? defaultTrad.QcmName : string.Empty),
                            NbQuestions = q.NbQuestions,
                            NoteMinimal = q.NoteMinimal,
                            QcmTrad = qcmTrad,
                            DefaultTrad = defaultTrad,

                        };
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
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public GetQcmResponse Get(GetQcmRequest request)
        {
            var response = new GetQcmResponse()
            {
                Qcm = new QcmDTO()
                {
                    Id = 0,
                    Name = string.Empty,
                    NbQuestions = 0,
                    NoteMinimal = 0,
                    Inactif = true,
                    IsUpdatable = true,                    
                    Questions = new List<QuestionDTO>()
                }
            };

            try
            {
                var qcm = _qcmRepository.FindBy(request.Id);
                if (qcm != null)
                {
                    response = new GetQcmResponse()
                    {
                        Qcm = new QcmDTO()
                        {
                            Id = qcm.Id,
                            NbQuestions = qcm.NbQuestions,
                            NoteMinimal = qcm.NoteMinimal,
                            Inactif = qcm.Inactif.Value == 1,
                            QcmTrad = _qcmlangRepository.FindBy(new Specification<QcmLang>(q => q.ID_Qcm == request.Id && q.ID_Lang == request.IdLang)).FirstOrDefault(),
                            DefaultTrad = _qcmlangRepository.FindBy(new Specification<QcmLang>(q => q.ID_Qcm == request.Id && q.ID_Lang != request.IdLang)).FirstOrDefault(),
                            IsUpdatable = !_docRepository.FindBy(new Specification<Document>(doc => doc.IdQcm == request.Id)).Any(),
                            IsRemovable = !_docRepository.FindBy(new Specification<Document>(doc => doc.IdQcm == request.Id)).Any() && !_userQcmRepository.FindBy(new Specification<UserQcm>(uQcm => uQcm.ID_Qcm == request.Id)).Any(),
                            Questions = _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == request.Id)).Any() ?
                             _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == request.Id)).OrderBy("inactif, OrdreQuestion")
                             .Select(q => {
                                 var trad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == q.Id && ql.ID_Lang == request.IdLang)).FirstOrDefault();
                                 var defaultTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == q.Id && ql.ID_Lang != request.IdLang)).FirstOrDefault();
                                 return new QuestionDTO()
                                     {
                                         Question = q,
                                         QuestionTrad = trad,
                                         DefaultTrad = defaultTrad,
                                         IsTrad = trad == null,
                                         IsMultipleReponse = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && r.IsRight)).Count() > 1,
                                         Reponses = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == q.Id && (!r.inactif.HasValue || !r.inactif.Value))).OrderBy(r => r.OrdreReponse)
                                         .Select(rep =>
                                         {
                                             var rTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang == request.IdLang)).FirstOrDefault();
                                             var rDefaultTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang != request.IdLang)).FirstOrDefault();
                                             return new ReponseDTO
                                             {
                                                 Reponse = rep,
                                                 ReponseTrad = rTrad,
                                                 DefaultTrad = rDefaultTrad,
                                                 IdLang = request.IdLang,
                                                 IsTrad = trad == null
                                             };
                                         })
                                         .ToList()
                                     };
                                  }
                                )
                             .ToList() : new List<QuestionDTO>()

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
                    MethodName = "Get",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public GetQuestionResponse GetQuestion(GetQuestionRequest request)
        {
            var response = new GetQuestionResponse()
            {
                Question = new QuestionDTO()
                {
                    Question = new Question()
                    {

                    },
                    QuestionTrad = new QuestionLang(),
                    DefaultTrad = new QuestionLang(),
                    IsMultipleReponse = false,
                    Reponses = new List<ReponseDTO>()
                  
                }
            };
            try
            {
                var question = _questionRepository.FindBy(request.Id);
                if (question != null)
                {
                    var qDefaultTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == request.Id && ql.ID_Lang != request.IdLang)).FirstOrDefault();
                    response = new GetQuestionResponse()
                    {
                        Question = new QuestionDTO()
                        {
                            Question = question,
                            QuestionTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == request.Id && ql.ID_Lang == request.IdLang)).FirstOrDefault(),
                            DefaultTrad = qDefaultTrad,
                            IdLang = request.IdLang,
                            IsTrad = _questionlangRepository.FindBy(new Specification<QuestionLang>(ql => ql.ID_Question == request.Id && ql.ID_Lang == request.IdLang)).FirstOrDefault() != null,
                            IsMultipleReponse = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == request.Id && r.IsRight)).Count() > 1,
                            Reponses = _reponseRepository.FindBy(new Specification<Reponse>(rep => rep.ID_Question == request.Id && (!rep.inactif.HasValue || !rep.inactif.Value))).OrderBy("OrdreReponse")
                             .Select(rep =>
                             {
                                 var rTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang == request.IdLang)).FirstOrDefault();
                                 var rDefaultTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang != request.IdLang)).FirstOrDefault();
                                 return new ReponseDTO
                                 {
                                     Reponse = rep,
                                     ReponseTrad = rTrad,
                                     DefaultTrad = rDefaultTrad,
                                     IdLang = request.IdLang
                                 };
                             }).ToList()
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
                    MethodName = "GetQuestion",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public SaveQcmResponse Save(SaveQcmRequest request)
        {
            var ActionsBO = new List<ActionBO>();
            var response = new SaveQcmResponse();
            try
            {
                var result = _qcmRepository.FindBy(request.Qcm.Id);
                var testDoublon = _qcmlangRepository.FindBy(new Specification<QcmLang>(q => q.QcmName.StartsWith(request.Qcm.QcmTrad.QcmName) && q.Qcm.Id != request.Qcm.Id)).ToList();
                if (testDoublon.Count > 0)
                {
                    request.Qcm.Name += testDoublon.Count < 2 ? " - Copie" : " - Copie (" + testDoublon.Count + ")";
                    request.Qcm.QcmTrad.QcmName += testDoublon.Count < 2 ? " - Copie" : " - Copie (" + testDoublon.Count + ")";
                }
                Debug.WriteLine(testDoublon.Count);
                Debug.WriteLine(request.Qcm.QcmTrad.QcmName);
                var qcmToSave = new Qcm(request.Qcm.Id)
                {
                    Id = request.Qcm.Id,
                    NoteMinimal = request.Qcm.NoteMinimal,
                    NbQuestions = request.Qcm.NbQuestions == 0 ?
                            (request.Qcm.Questions != null ? request.Qcm.Questions.Count :
                            _questionRepository.Count(new Specification<Question>(q => q.Id_Qcm == request.Qcm.Id))) :
                            request.Qcm.NbQuestions,
                    QcmLang = new List<QcmLang>(){
                        request.Qcm.QcmTrad
                    },
                    DateCreation = DateTime.Now,
                    Inactif = request.Qcm.Inactif ? 1 : 0

                };
                if (result == null)
                {
                    _qcmRepository.Save(qcmToSave);
                }
                else
                {
                    _qcmRepository.Update(qcmToSave);
                }

                ActionsBO.Add(new ActionBO()
                {
                    Action = request.Qcm.Id == 0 ? Actions.Create : Actions.Update,
                    ID_Object = qcmToSave.Id,
                    ID_User = request.UserId,
                    ObjectCode = ObjectCode.QUIZZ,
                    LinkObjects = string.Empty,
                    DateAction = DateTime.Now

                });

                _histoActionBO = new HistoActionBO(base.uow, ActionsBO);
                _histoActionBO.SaveHisto();
                return new SaveQcmResponse() {
                    Id = qcmToSave.Id
                };

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Save",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public SaveQuestionResponse SaveQuestion(SaveQuestionRequest request)
        {
            var ActionsBO = new List<ActionBO>();
            var response = new SaveQuestionResponse();
            try
            {

                var questionToSave = new Question(request.Question.Question.Id);
                var questionTrad = new QuestionLang()
                {
                    ID_Lang = request.Question.IdLang,
                    ID_Question = request.Question.Question.Id,
                    TexteJustification = request.Question.QuestionTrad.TexteJustification,
                    TexteQuestion = request.Question.QuestionTrad.TexteQuestion,
                    Illustration = request.Question.QuestionTrad.Illustration
                };


                questionToSave.Id = request.Question.Question.Id;
                questionToSave.Id_Qcm = request.Question.Question.Id_Qcm;
                questionToSave.OrdreQuestion = request.Question.Question.OrdreQuestion == 0 ? GetQuestionDefaultOrder(request.Question.Question.Id_Qcm) : request.Question.Question.OrdreQuestion;
                questionToSave.inactif = request.Question.Question.inactif;
                questionToSave.QuestionLang.Add(questionTrad);
                questionToSave.Reponse = request.Question.Reponses
                    .Select(rep =>
                    {
                        var reponseTrad = new List<ReponseLang>()
                        {
                            new ReponseLang()
                            {
                                ID_Reponse = rep.Reponse.Id,
                                ID_Lang = request.Question.IdLang,
                                Texte = rep.ReponseTrad.Texte,                                
                            }
                        };
                        return new Reponse(rep.Reponse.Id)
                        {
                            Id = rep.Reponse.Id,
                            IsRight = rep.Reponse.IsRight,
                            ID_Question = rep.Reponse.ID_Question,
                            inactif = rep.Reponse.inactif,
                            OrdreReponse = rep.Reponse.OrdreReponse,
                            ReponseLang = reponseTrad
                        };
                    }).ToList();



                if (request.Question.Question.Id == 0)
                {
                    _questionRepository.Save(questionToSave);
                }
                else
                {
                    _questionRepository.Update(questionToSave, request.Question.IdLang);
                }

                ActionsBO.Add(new ActionBO()
                {
                    Action = Actions.Update,
                    ID_Object = questionToSave.Id_Qcm,
                    ID_User = request.UserId,
                    ObjectCode = ObjectCode.QUIZZ,
                    LinkObjects = string.Empty,
                    DateAction = DateTime.Now

                });
                _histoActionBO = new HistoActionBO(base.uow, ActionsBO);
                _histoActionBO.SaveHisto();
                return new SaveQuestionResponse()
                {
                    Id = questionToSave.Id
                };

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "SaveQuestion",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        private int GetQuestionDefaultOrder(int idQcm)
        {
            var defaultOrder = 10;
            var quest = _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == idQcm && q.inactif == 0)).Count();
            return quest > 0 ? defaultOrder * (quest + 1) : defaultOrder;
        }

        public OrderQuestionResponse OrderQuestion(OrderQuestionRequest request)
        {
            var response = new OrderQuestionResponse();
            try
            {
                var questions = new List<Question>();

                var questionToMove = _questionRepository.FindBy(request.Id);
                var questionToSubstitute = request.Direction == "UP" ? _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == questionToMove.Id_Qcm))
                    .OrderBy(string.Format("OrdreQuestion DESC")).Where(q => q.OrdreQuestion < questionToMove.OrdreQuestion)
                    .FirstOrDefault() :
                    _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == questionToMove.Id_Qcm))
                    .OrderBy(string.Format("OrdreQuestion")).Where(q => q.OrdreQuestion > questionToMove.OrdreQuestion)
                    .FirstOrDefault();

                if (request.Direction == "UP")
                {
                    questionToMove.OrdreQuestion -= 10;
                    questionToSubstitute.OrdreQuestion += 10;
                }
                else
                {
                    questionToMove.OrdreQuestion += 10;
                    questionToSubstitute.OrdreQuestion -= 10;
                }

                questions.Add(new Question(questionToMove.Id)
                {
                    Id = questionToMove.Id,
                    Id_Qcm = questionToMove.Id_Qcm,
                    inactif = questionToMove.inactif,
                    OrdreQuestion = questionToMove.OrdreQuestion,
                });



                questions.Add(new Question(questionToSubstitute.Id)
                {
                    Id = questionToSubstitute.Id,
                    Id_Qcm = questionToSubstitute.Id_Qcm,
                    inactif = questionToSubstitute.inactif,
                    OrdreQuestion = questionToSubstitute.OrdreQuestion,
                });

                foreach (var question in questions)
                {
                    _questionRepository.Update(question);
                }

                response.Question = new QuestionDTO()
                {
                    Question = questionToMove,
                    IsMultipleReponse = _reponseRepository.FindBy(new Specification<Reponse>(r => r.ID_Question == questionToMove.Id && r.IsRight)).Count() > 1,
                    Reponses = _reponseRepository.FindBy(new Specification<Reponse>(rep => rep.ID_Question == questionToMove.Id))
                    .Select(rep =>
                    {
                        var rTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == rep.Id && rl.ID_Lang == request.IdLang)).FirstOrDefault();
                        return new ReponseDTO
                        {
                            Reponse = rep,
                            ReponseTrad = rTrad,
                            IdLang = request.IdLang
                        };
                    }).ToList()
                };

   
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "OrdreQuestion",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }

        }

        public DeleteQcmResponse Delete(DeleteQcmRequest request)
        {
            var response = new DeleteQcmResponse();
            var ActionsBO = new List<ActionBO>();
            try
            {
                var qcm = _qcmRepository.FindBy(request.Id);
                if (qcm != null)
                {

                    var qcmToDelete = new Qcm(request.Id)
                    {
                        Id = request.Id,
                        DateCreation = qcm.DateCreation,
                        NbQuestions = qcm.NbQuestions,
                        NoteMinimal = qcm.NoteMinimal,
                        Inactif = 1
                    };
                    _qcmRepository.Update(qcmToDelete);

                    ActionsBO.Add(new ActionBO()
                    {
                        Action = Actions.Delete,
                        ID_Object = qcmToDelete.Id,
                        ID_User = request.UserId,
                        ObjectCode = ObjectCode.QUIZZ,
                        LinkObjects = string.Empty,
                        DateAction = DateTime.Now

                    });

                    _histoActionBO = new HistoActionBO(base.uow, ActionsBO);
                    _histoActionBO.SaveHisto();
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
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        /*public DeleteQuestionResponse DeleteQuestion(DeleteQuestionRequest request)
        {
            var response = new DeleteQuestionResponse();
            try
            {
                var questionToDelete = _questionRepository.FindBy(request.Id);
                if (questionToDelete != null)
                {
                   
                    var allActiveQuestions = _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == questionToDelete.Id_Qcm)).OrderBy("OrdreQuestion").ToList();
                    var ordreQuestion = 10;
                    foreach (var quest in allActiveQuestions)
                    {
                        var questionToOrder = new Question(questionToDelete.Id)
                        {
                            Id = quest.Id,
                            Id_Qcm = quest.Id_Qcm,
                            inactif = quest.Id == questionToDelete.Id ? 1 : quest.inactif,
                            Reponse = _reponseRepository.FindBy(new Specification<Reponse>(rep => rep.ID_Question == quest.Id)).ToList()
                        };

                       
                        if (questionToOrder.Id != questionToDelete.Id && questionToOrder.inactif == 0)
                        {
                            questionToOrder.OrdreQuestion = ordreQuestion;
                            ordreQuestion += 10;
                        }
                        else
                        {
                            questionToOrder.OrdreQuestion = 0;
                        }

                        if (request.IsDeleted && quest.Id == questionToDelete.Id)
                        {
                            _questionRepository.Remove(questionToDelete);
                        }
                        else
                        { 
                            _questionRepository.Update(questionToOrder);
                        }
                    }
                }

                return new DeleteQuestionResponse() {
                    Question = new QuestionDTO()
                    {
                        Question = questionToDelete
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteQuestion",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }*/
        public DeleteQuestionResponse DeleteQuestion(DeleteQuestionRequest request)
        {
            var response = new DeleteQuestionResponse();
            try
            {
                var questionToDelete = _questionRepository.FindBy(request.Id);
                if (questionToDelete != null)
                {
                    if (questionToDelete.inactif == 1)
                    {
                        //1 - Reponse update set ID_Question = null and inactif = 1 where ID_Question = ID
                        var allResponses = _reponseRepository.FindBy(new Specification<Reponse>(q => q.ID_Question == questionToDelete.Id)).ToList();
                        foreach (var res in allResponses)
                        {
                            var newRes = new Reponse()
                            {
                                Id = res.Id,
                                ID_Question = null,
                                inactif = true,
                            };
                            _reponseRepository.Update(newRes, questionToDelete.Id);
                        }
                        //2 - QuestionLang delete where ID_Question = ID
                        var langs = _langRepository.FindAll().ToList();
                        foreach (var lang in langs)
                        {
                            _questionlangRepository.Delete(questionToDelete.Id, lang.ID);
                        }
                        //3 - Question delete where ID_Question = ID
                        _questionRepository.Remove(questionToDelete);
                    }
                    else
                    {
                        var allActiveQuestions = _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == questionToDelete.Id_Qcm)).OrderBy("OrdreQuestion").ToList();
                        var ordreQuestion = 10;
                        foreach (var quest in allActiveQuestions)
                        {
                            var questionToOrder = new Question(questionToDelete.Id)
                            {
                                Id = quest.Id,
                                Id_Qcm = quest.Id_Qcm,
                                inactif = quest.Id == questionToDelete.Id ? 1 : quest.inactif,
                                Reponse = _reponseRepository.FindBy(new Specification<Reponse>(rep => rep.ID_Question == quest.Id)).ToList()
                            };


                            if (questionToOrder.Id != questionToDelete.Id && questionToOrder.inactif == 0)
                            {
                                questionToOrder.OrdreQuestion = ordreQuestion;
                                ordreQuestion += 10;
                            }
                            else
                            {
                                questionToOrder.OrdreQuestion = 0;
                            }
                            _questionRepository.Update(questionToOrder);
                        }
                    }
                }

                return new DeleteQuestionResponse() {
                    Question = new QuestionDTO()
                    {
                        Question = questionToDelete
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteQuestion",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        private void ReorderAllQuestion(int idQcm)
        {
            try
            {
                var allQuestions = _questionRepository.FindBy(new Specification<Question>(q => q.Id_Qcm == idQcm)).OrderBy("OrdreQuestion");
                var ordreQuestion = 10;
                foreach(var question in allQuestions)
                {
                   if (question.inactif == 0)
                    {
                        question.OrdreQuestion = ordreQuestion;                        
                        ordreQuestion += 10;
                    }
                    else
                    {
                        question.OrdreQuestion = 0;
                    }

                    _questionRepository.Update(question);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "ReorderAllQuestion",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public DeleteReponseResponse DeleteReponse(DeleteReponseRequest request)
        {
            var response = new DeleteReponseResponse();
            try
            {
                var reponseToDelete = _reponseRepository.FindBy(request.Id);
                if (reponseToDelete != null)
                {
                    
                    _reponseRepository.Update(new Reponse(request.Id) {
                        Id = reponseToDelete.Id,
                        ID_Question = reponseToDelete.ID_Question,
                        IsRight = reponseToDelete.IsRight,
                        OrdreReponse = reponseToDelete.OrdreReponse,
                        inactif = true,
                    });
                }

                return new DeleteReponseResponse()
                {
                   
                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteQuestion",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public GetReponseResponse GetReponse(GetReponseRequest request)
        {
            var response = new GetReponseResponse();
            try
            {
                var reponse = _reponseRepository.FindBy(request.Id);
                if (reponse != null)
                {
                    response = new GetReponseResponse()
                    {
                        Reponse = reponse,
                        ReponseTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == request.Id && rl.ID_Lang == request.IdLang)).FirstOrDefault(),
                        DefaultTrad = _reponselangRepository.FindBy(new Specification<ReponseLang>(rl => rl.ID_Reponse == request.Id && rl.ID_Lang != request.IdLang)).FirstOrDefault()
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
                    MethodName = "GetReponse",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public bool DeleteIllustration(int id, int lanId)
        {
            try
            {
                _questionlangRepository.Delete(id, lanId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteIllustration",
                    ServiceName = "QcmService",
                }, ex);
                throw ex;
            }
        }
    }
}
