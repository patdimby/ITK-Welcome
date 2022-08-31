using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class ADController : CommunController
    {
        private readonly IADService _adService;

        public ADController()
        {
            _adService = new ADService(new FileLogger());
        }

        // GET: Admin/Config
        public ActionResult Index()
        {
            string nameGrid = "domainsGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);
            GetAllADRequest adRequest = new GetAllADRequest() { GridRequest = request };
            var ADs = _adService.GetAll(adRequest).ADs;
            var total = ADs.Count;
            var adViewModels = AutoMapperConfigAdmin.Mapper.Map<List<ADViewModel>>(ADs);
            var grid = new GridBO<ADViewModel>(request, adViewModels, total, request.Limit);
            return View(grid);
        }

        [HandleError(View = "~/Areas/Admin/Views/Shared/AjaxError.cshtml")]
        public ActionResult AjaxDomainsGrid()
        {
            string nameGrid = "domainsGrid";
            GridMvcRequest request = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            base.SetGridRequestSession(request, "Index");
            GetAllADRequest adRequest = new GetAllADRequest() { GridRequest = request };
            var ADs = _adService.GetAll(adRequest).ADs;
            var total = ADs.Count;
            var adViewModels = AutoMapperConfigAdmin.Mapper.Map<List<ADViewModel>>(ADs);
            var grid = new GridBO<ADViewModel>(request, adViewModels, total, request.Limit);
            return Json(new { Html = grid.ToJson("_DomainsGrid", this), total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id = 0)
        {
            var response = _adService.Get(new GetADRequest()
            {
                Id = Id,
                PwdKey = ConfigurationManager.AppSettings["keyPwd"],
                //PwdIV = "qf6bYB7dJxer+CQjoVhAdQ==" // ConfigurationManager.AppSettings["ivPwd"]
                PwdIV = ConfigurationManager.AppSettings["ivPwd"]
            });

            var adViewModel = new ADViewModel();

            if (response.AD != null)
            {
                adViewModel = AutoMapperConfigAdmin.Mapper.Map<ADViewModel>(response.AD);
                adViewModel.ConfirmPassword = adViewModel.Password;
            }

            return View(adViewModel);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(ADViewModel model)
        {
            var request = new SaveADRequest()
            {
                fromBatch = false,
                AD = new Domain.AD()
                {
                    ID = model.ID,
                    Name = model.Name,
                    Address = model.Address,
                    Domain = model.Domain,
                    Username = model.Username,
                    Password = model.Password,
                    ToBeSynchronized = model.ToBeSynchronized
                }
                
            };

            request.PwdKey = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M="; // ConfigurationManager.AppSettings["keyPwd"];
            request.PwdIV = "qf6bYB7dJxer+CQjoVhAdQ=="; //ConfigurationManager.AppSettings["ivPwd"];

            var response = _adService.Save(request);

            return RedirectToAction("Index", "AD");
        }

        public ActionResult ConfirmDelete(int Id)
        {
            var response = _adService.Get(new GetADRequest()
            {
                Id = Id,
                //PwdKey = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M=", //ConfigurationManager.AppSettings["keyPwd"],
                PwdKey = ConfigurationManager.AppSettings["keyPwd"],
                //PwdIV = "qf6bYB7dJxer+CQjoVhAdQ==" //ConfigurationManager.AppSettings["ivPwd"]
                PwdIV = ConfigurationManager.AppSettings["ivPwd"]
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = response.AD.ID,
                CanDelete = true,
                ControllerName = "AD",
                ActionName = "Delete",
                EntityName = Resources.Resource.the_domain,
                Name = response.AD.Name
            });
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Delete(int id)
        {
            var reponse = _adService.Delete(new DeleteADRequest()
            {
                Id = id
            });

            if (Request.IsAjaxRequest())
                return Json(new { Id = id }, JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
    }
}