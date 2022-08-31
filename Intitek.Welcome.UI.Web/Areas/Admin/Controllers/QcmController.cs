using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using Intitek.Welcome.UI.Web.Infrastructure;
using Newtonsoft.Json;
using System.Linq;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class QcmController : CommunController
    {
        private readonly IQcmService _qcmService;
        private readonly ILangService _langService;
        private ILogger _logger;
        public QcmController()
        {
            this._logger = new FileLogger();
            _qcmService = new QcmService(this._logger);
            _langService = new LangService(new FileLogger());

        }
        // GET: Admin/Qcm
        public ActionResult Index()
        {
            var idLang = GetIdLang();
            string nameGrid = "qcmGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllQcmRequest docRequest = new GetAllQcmRequest() { GridRequest = request, IdLang =idLang };
            var total = _qcmService.GetAllCount(docRequest);
            var qcms = _qcmService.GetAll(docRequest).Qcms;
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<QcmViewModel>>(qcms);
            var grid = new GridBO<QcmViewModel>(request, viewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxQcmGrid()
        {
            var idLang = GetIdLang();
            string nameGrid = "qcmGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, "Name");
            base.SetGridRequestSession(request, "Index");
            GetAllQcmRequest docRequest = new GetAllQcmRequest() { GridRequest = request, IdLang = idLang };
            var total = _qcmService.GetAllCount(docRequest);
            var qcms = _qcmService.GetAll(docRequest).Qcms;
            var viewModels = AutoMapperConfigAdmin.Mapper.Map<List<QcmViewModel>>(qcms);
            var grid = new GridBO<QcmViewModel>(request, viewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_QcmGrid", this) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id = 0, int IdLang = 0, bool viewOnly = false)
        {

            var codeLang = GetCodeLang();
            var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;
            var langues = _langService.GetAll(new GetAllLangRequest()).Langues;
            var response = _qcmService.Get(new GetQcmRequest()
            {
                Id = Id,
                IdLang = IdLang == 0 ? lang.ID : IdLang,
            });


            return View(new QcmViewModel()
            {
                ID = Id,
                Name = response.Qcm.QcmTrad != null ? response.Qcm.QcmTrad.QcmName : string.Empty, // response.Qcm.DefaultTrad.QcmName,
                DefaultTradName = response.Qcm.DefaultTrad != null ? response.Qcm.DefaultTrad.QcmName : string.Empty,
                NbQuestions = response.Qcm.NbQuestions,
                NoteMinimal = response.Qcm.NoteMinimal,
                Inactif = response.Qcm.Inactif,
                IsUpdatable = !viewOnly,
                IsRemovable = response.Qcm.IsRemovable,
                Questions = response.Qcm.Questions,
                CodeLangue = codeLang.Substring(0,2)
            });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(QcmViewModel model)
        {
            var codeLang = GetCodeLang();
            var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;
            var reponse = new SaveQcmResponse();
            byte[] illusData = null;

            if (Request.Files!=null && Request.Files.Count > 0)
            {
                var fileToUpload = Request.Files["FileUpload"];
                if (fileToUpload != null)
                {
                    string extension = Path.GetExtension(fileToUpload.FileName);
                    if (extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string path = Path.Combine(Server.MapPath(QcmViewModel.FOLDER_SAVE_BADGE), model.Filename);
                        fileToUpload.SaveAs(path);
                    }
                }
                var fileToUploadPDF = Request.Files["FileUploadPDF"];
                if (fileToUploadPDF != null)
                {
                    string extensionPDF = Path.GetExtension(fileToUploadPDF.FileName);
                    if (extensionPDF.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string path = Path.Combine(Server.MapPath(QcmViewModel.FOLDER_TEMPLATE_DIPLOME), model.TemplateFilename);
                        fileToUploadPDF.SaveAs(path);
                    }
                }
                var illusToUpload = Request.Files["IllusUpload"];
                if (illusToUpload != null)
                {
                    if (illusToUpload.ContentLength > 0 && illusToUpload.ContentType.Contains("image"))
                    {
                        using (var binaryReader = new BinaryReader(illusToUpload.InputStream))
                        {
                            illusData = binaryReader.ReadBytes(illusToUpload.ContentLength);
                        }
                    }
                }
            }
            //Si updatable
            if (model.IsUpdatable)
            {
                var request = new SaveQcmRequest()
                {
                    Qcm = new QcmDTO()
                    {
                        Id = model.ID,
                        Name = model.Name,
                        NbQuestions = model.NbQuestions.HasValue ? model.NbQuestions.Value : 0,
                        NoteMinimal = model.NoteMinimal,
                        Questions = model.Questions,
                        QcmTrad = new Domain.QcmLang()
                        {
                            ID_Qcm = model.ID,
                            ID_Lang = lang.ID,
                            QcmName = model.Name
                        },
                        Inactif = model.Inactif 
                        
                    },
                    UserId = GetUserIdConnected(),
                    UserName = GetUserConnected().Username
                };
                reponse = _qcmService.Save(request);
            }
            

            return RedirectToAction("Edit", new { Id = reponse.Id });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult DeleteBadge(int Id, string CodeLangue)
        {
            QcmViewModel qcmVm = new QcmViewModel() { ID = Id, CodeLangue = CodeLangue };
            string templates = System.Web.HttpContext.Current.Server.MapPath(QcmViewModel.FOLDER_SAVE_BADGE);
            try
            {
                System.IO.File.Delete(templates + qcmVm.Filename);
            }
            catch(Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "DeleteBadge",
                    ServiceName = "QcmController",

                }, ex);
            }
            return RedirectToAction("Index");

        }
        
        public ActionResult GetQuestion(int Id = 0, int IdLang = 0)
        {
            var codeLang = GetCodeLang();
            var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;
            var langues = _langService.GetAll(new GetAllLangRequest()).Langues;
            var response = _qcmService.GetQuestion(new GetQuestionRequest()
            {
                Id = Id,
                IdLang = IdLang == 0 ? lang.ID : IdLang,
            });

            return PartialView("_QcmQuestionEdit", new QuestionViewModel()
            {
                Id = response.Question.Question.Id,
                Id_Lang = IdLang = IdLang == 0 ? lang.ID : IdLang,
                Id_Qcm = response.Question.Question.Id_Qcm,
                TexteJustification = response.Question.QuestionTrad == null ? string.Empty : response.Question.QuestionTrad.TexteJustification,
                DefaultTexteJustification = response.Question.DefaultTrad == null ? string.Empty : response.Question.DefaultTrad.TexteJustification,
                TexteQuestion = response.Question.QuestionTrad == null ? string.Empty : response.Question.QuestionTrad.TexteQuestion,
                DefaultTexteQuestion = response.Question.DefaultTrad == null ? string.Empty : response.Question.DefaultTrad.TexteQuestion,
                OrdreQuestion = response.Question.Question.OrdreQuestion,
                IsMultipleReponse = false,
                inactif = response.Question.Question.inactif,
                Reponses = response.Question.Reponses,
                Illustration = response.Question.QuestionTrad == null ? null : response.Question.QuestionTrad.Illustration
            });

        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult OrderQuestion(int Id, string Direction)
        {
            var question = _qcmService.GetQuestion(new GetQuestionRequest()
            {
                Id = Id
            });

            var response = _qcmService.OrderQuestion(new OrderQuestionRequest()
            {
                Id = Id,
                OrdreQuestion = question.Question.Question.OrdreQuestion,
                Direction = Direction
            });

            var data = response.Question.Question;

            return Json(new { success = true, data =  data.Id_Qcm }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SaveQuestion()
        {
            QuestionViewModel model = JsonConvert.DeserializeObject<QuestionViewModel>(Request.Form["model"]);
            byte[] illustration = null;
            var file = Request.Files.Count > 0 ? Request.Files[0] : null;
            if(file != null)
            {
                if (file.ContentLength > 0 && file.ContentType.Contains("image"))
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        illustration = binaryReader.ReadBytes(file.ContentLength);
                    }
                } else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            var codeLang = GetCodeLang();
            var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;
            var request = new SaveQuestionRequest()
            {

                Question = new QuestionDTO()
                {
                    IdLang = model.Id_Lang,
                    Question = new Domain.Question()
                    {
                        Id = model.Id,
                        Id_Qcm = model.Id_Qcm,

                        OrdreQuestion = model.OrdreQuestion,
                        inactif = model.inactif
                    },
                    QuestionTrad = new Domain.QuestionLang()
                    {
                        ID_Question = model.Id,
                        ID_Lang = lang.ID,
                        TexteQuestion = model.TexteQuestion,
                        TexteJustification = model.TexteJustification,
                        Illustration = illustration
                    },
                    Reponses = model.Reponses,
                },
                UserId = GetUserIdConnected()
                
            };

            if (model.inactif == 1)
            {
               var response =  _qcmService.DeleteQuestion(new DeleteQuestionRequest()
                {
                    Id = model.Id,
                    IsDeleted = false
                });
            }
            else {
                var response = _qcmService.SaveQuestion(request);
            }

            return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmDelete(int Id)
        {
            var response = _qcmService.Get(new GetQcmRequest()
            {
                Id = Id
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = response.Qcm.Id,
                ControllerName = "Qcm",
                CanDelete = response.Qcm.IsRemovable,
                EntityName = Resources.Resource.le_qcm,
                Name = response.Qcm.Name
            });
        }

        public ActionResult Delete(int Id)
        {
            var reponse = _qcmService.Delete(new DeleteQcmRequest()
            {
                Id = Id,
                UserId = GetUserIdConnected(),
                UserName = GetUserConnected().Username
               
            });

            return RedirectToAction("Index");
        }

        public ActionResult ConfirmQuestionDelete(int Id, bool IsDeleted = false)
        {
            var response = _qcmService.GetQuestion(new GetQuestionRequest()
            {
                Id = Id,
                
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = response.Question.Question.Id,
                CanDelete = true,
                ControllerName = "Qcm",
                ActionName = "DeleteQuestion",
                EntityName = Resources.Resource.la_question,
                IsDeleted = IsDeleted,
                Name = string.Format("{0}. {1}", response.Question.Question.OrdreQuestion, response.Question.QuestionTrad == null ? response.Question.DefaultTrad.TexteQuestion : response.Question.QuestionTrad.TexteQuestion)
            });
        }



        public ActionResult DeleteQuestion(int Id, bool IsDeleted = false)
        {
            var response = _qcmService.DeleteQuestion(new DeleteQuestionRequest()
            {
                Id = Id,
                IsDeleted = IsDeleted
            });

            
            return RedirectToAction("Edit", new {  Id = response.Question.Question.Id_Qcm });
        }

        public ActionResult ConfirmReponseDelete(int Id)
        {
            var response = _qcmService.GetReponse(new GetReponseRequest()
            {
                Id = Id
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = Id,
                CanDelete = true,
                ControllerName = "Qcm",
                ActionName = "DeleteReponse",
                EntityName = Resources.Resource.la_question,
                Name = response.ReponseTrad == null ? response.DefaultTrad.Texte : response.ReponseTrad.Texte
            });
        }

        public ActionResult DeleteReponse(int Id)
        {
            var response = _qcmService.DeleteReponse(new DeleteReponseRequest()
            {
                Id = Id,

            });
           return Json(new { success = true, data = Id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteIllustration(int Id, int LanId)
        {
            var result = _qcmService.DeleteIllustration(Id, LanId);
            return Json(new { success = result });
        }

        [HttpGet]
        public FileContentResult ExportExcel(int Id)
        {
            byte[] fileresult = null;
            try
            {
                var codeLang = GetCodeLang();
                var lang = _langService.Get(new GetLangRequest() { Code = codeLang }).Langue;
                var langues = _langService.GetAll(new GetAllLangRequest()).Langues;
                // GET QCM Current Lang
                var responseFR = _qcmService.Get(new GetQcmRequest()
                {
                    Id = Id,
                    IdLang = 1,
                });
                var qcmFR = responseFR.Qcm;
                // GET QCM Other Lang
                var responseEN = _qcmService.Get(new GetQcmRequest()
                {
                    Id = Id,
                    IdLang = 2,
                });
                var qcmEN = responseEN.Qcm;
                string filename = (lang.ID == 2 ? (qcmEN.QcmTrad != null ? qcmEN.QcmTrad?.QcmName : qcmFR.QcmTrad?.QcmName) : qcmFR.QcmTrad?.QcmName) + "_" + DateTime.Now.ToString("dd/MM/yyyy") + "_" + DateTime.Now.ToString("HH") +
                              DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss") + ".xlsx";
                string templatePath = Server.MapPath("~/Templates/Excel/template_export_import_qcm.xlsx");
                ExcelExportInfo excelExportInfo = new ExcelExportInfo
                {
                    TemplatePath = templatePath
                };
                fileresult = ExcelExportHelper.ExportExcel(qcmFR, qcmEN, excelExportInfo);
                return File(fileresult, ExcelExportHelper.ExcelContentType, filename);
            }
            catch(Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult UploadExcel()
        {
            var fileName = String.Empty;
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var fileToUpload = Request.Files[0];
                if (fileToUpload != null)
                {
                    fileName = fileToUpload.FileName;
                    if (System.IO.File.Exists(Path.Combine(Server.MapPath(QcmViewModel.FOLDER_SAVE_QCM), fileToUpload.FileName)))
                    {
                        fileName = $@"{Path.GetFileNameWithoutExtension(fileToUpload.FileName)}_{DateTime.Now:dd_MM_yyyy_hhmmss}{Path.GetExtension(fileToUpload.FileName)}";
                    }
                    string extension = Path.GetExtension(fileToUpload.FileName);
                    if (extension.Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string path = Path.Combine(Server.MapPath(QcmViewModel.FOLDER_SAVE_QCM), fileName);
                        Directory.CreateDirectory(Server.MapPath(QcmViewModel.FOLDER_SAVE_QCM));
                        fileToUpload.SaveAs(path);
                        return Json(new
                        {
                            Response = Json(new
                            {
                                status = "OK",
                                fileName,
                                fileUrl = path,
                                success = true
                            })
                        });
                    }
                }
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { status = "KO", success = false });
            
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult LoadQCMDirect(string fileName)
        {
            try
            {
                // locate file
                var filePath = Server.MapPath($"~/excel/qcm/{fileName}");

                // FR
                var qcmDTO = ExcelExportHelper.LoadQCMFromExcel(filePath, fileName, 1);
                var request = new SaveQcmRequest()
                {
                    Qcm = qcmDTO,
                    UserId = GetUserIdConnected(),
                    UserName = GetUserConnected().Username
                };
                var reponse = _qcmService.Save(request);
                this._logger.Info("Loading QCM FR success!");
                // EN
                var qcmDTOEN = ExcelExportHelper.LoadQCMFromExcel(filePath, fileName, 2);
                if(qcmDTOEN.QcmTrad != null)
                {
                    var requestEN = new SaveQcmRequest()
                    {
                        Qcm = new QcmDTO()
                        {
                            Id = reponse.Id,
                            NbQuestions = qcmDTOEN.NbQuestions > qcmDTO.NbQuestions ? qcmDTOEN.NbQuestions : qcmDTO.NbQuestions,
                            NoteMinimal = qcmDTOEN.NbQuestions > qcmDTO.NbQuestions ? qcmDTOEN.NoteMinimal : qcmDTO.NoteMinimal,
                            Questions = qcmDTOEN.Questions,
                            QcmTrad = new Domain.QcmLang()
                            {
                                ID_Qcm = reponse.Id,
                                ID_Lang = 2,
                                QcmName = qcmDTOEN.QcmTrad.QcmName
                            }
                        },
                        UserId = GetUserIdConnected(),
                        UserName = GetUserConnected().Username
                    };
                    reponse = _qcmService.Save(requestEN);
                }
                this._logger.Info("Loading QCM EN success!");

                // Save Questions
                for (int i = 0; i < qcmDTO.Questions.Count; i++)
                {
                    var requestQuestion = new SaveQuestionRequest()
                    {

                        Question = new QuestionDTO()
                        {
                            IdLang = qcmDTO.Questions[i].QuestionTrad.ID_Lang,
                            Question = new Domain.Question()
                            {
                                Id = 0,
                                Id_Qcm = reponse.Id,
                                inactif = 0
                            },
                            QuestionTrad = new Domain.QuestionLang()
                            {
                                ID_Question = 0,
                                ID_Lang = qcmDTO.Questions[i].QuestionTrad.ID_Lang,
                                TexteQuestion = qcmDTO.Questions[i].QuestionTrad.TexteQuestion,
                                TexteJustification = qcmDTO.Questions[i].QuestionTrad.TexteJustification
                            },
                            Reponses = qcmDTO.Questions[i].Reponses,
                        },
                        UserId = GetUserIdConnected()

                    };
                    var result = _qcmService.SaveQuestion(requestQuestion);

                    // EN
                    if (qcmDTOEN != null && i < qcmDTOEN.Questions?.Count)
                    {
                        var requestQuestionEN = new SaveQuestionRequest()
                        {

                            Question = new QuestionDTO()
                            {
                                IdLang = 2,
                                Question = new Domain.Question()
                                {
                                    Id = result.Id,
                                    Id_Qcm = reponse.Id
                                },
                                QuestionTrad = new Domain.QuestionLang()
                                {
                                    ID_Question = result.Id,
                                    ID_Lang = qcmDTOEN.Questions[i].QuestionTrad.ID_Lang,
                                    TexteQuestion = qcmDTOEN.Questions[i].QuestionTrad.TexteQuestion,
                                    TexteJustification = qcmDTOEN.Questions[i].QuestionTrad.TexteJustification
                                },
                                Reponses = qcmDTOEN.Questions[i].Reponses,
                            },
                            UserId = GetUserIdConnected()

                        };
                        
                        var laQuestion = _qcmService.GetQuestion(new GetQuestionRequest { Id = result.Id });
                        var qcmReponsesEN = qcmDTOEN.Questions[i].Reponses;
                        for (int j = 0; j < laQuestion.Question.Reponses.Count; j++)
                        {
                            if (j < qcmReponsesEN.Count)
                            {
                                qcmReponsesEN[j].ReponseTrad.ID_Reponse = laQuestion.Question.Reponses[j].Reponse.Id;
                                qcmReponsesEN[j].Reponse = laQuestion.Question.Reponses[j].Reponse;
                            }
                                
                        }
                        requestQuestionEN.Question.Reponses = qcmReponsesEN;
                        requestQuestionEN.Question.Question = laQuestion.Question.Question;
                        var resultEN = _qcmService.SaveQuestion(requestQuestionEN);
                    }
                }
                this._logger.Info("Chargement des " + qcmDTO.Questions.Count + " questions : succès!");
                // delete files after update database
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                this._logger.Info("Removing temporary file success!");
                return Json(new { success = true, status = "OK" });
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.Message);
                this._logger.Error(ex.StackTrace);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { status = "KO", success = false });
            }
        }
    }
}