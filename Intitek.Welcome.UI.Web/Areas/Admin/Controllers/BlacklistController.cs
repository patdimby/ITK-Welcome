using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using Intitek.Welcome.UI.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class BlacklistController : CommunController
    {
        private readonly IBlacklistService _blacklistService;
        public BlacklistController()
        {
            _blacklistService = new BlacklistService(new FileLogger());
        }
        public ActionResult Index()
        {
            string nameGrid = "blackListGrid";
            string nameGrid2 = "cityEntityBLGrid";
            GridMvcRequest initrequest = GridBORequest.GetRequestGrid(Request, nameGrid, null);
            GridMvcRequest request = base.GetGridRequestSession(initrequest);

            GridMvcRequest initrequest2 = GridBORequest.GetRequestGrid(Request, nameGrid2, null);
            GridMvcRequest request2 = base.GetGridRequestSession(initrequest2);
            var response = _blacklistService.GetAll();
            var blackListVMs = AutoMapperConfigAdmin.Mapper.Map<List<BlackListViewModel>>(response.BlackLists);
            var cityEntityBlackListVMs = AutoMapperConfigAdmin.Mapper.Map<List<CityEntityBlacklistViewModel>>(response.CityEntityBlacklists);
            var grid = new GridBO<BlackListViewModel>(request, blackListVMs, blackListVMs.Count, -1);
            var gridCityEntityBlackLVMs = new GridBO<CityEntityBlacklistViewModel>(request2, cityEntityBlackListVMs, cityEntityBlackListVMs.Count, -1);
            return View(new BlackListViewModels() {GridBlackList = grid, GridCityEntityBlacklist=gridCityEntityBlackLVMs });
        }
        public ActionResult ConfirmDelete(string Id)
        {
            var response = _blacklistService.GetBlackList(new GetBlackListRequest()
            {
                Path = Id
            });

            return PartialView("_ConfirmDelete", new ConfirmDeleteViewModel()
            {
                ID = response.BlackList.Path,
                Name = response.BlackList.Path,
                CanDelete = true,
                ControllerName = "Blacklist"
            });
        }
        public ActionResult Delete(string Id)
        {
            var reponse = _blacklistService.DeleteBlackList(new DeleteBlackListRequest()
            {
                Id = Id,
            });
            return RedirectToAction("Index", "Blacklist");
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult BlackListExist(BlackListViewModel model)
        {
            var isNameEx = false;
            if (!string.IsNullOrWhiteSpace(model.Path))
            {
                var request = new SaveBlackListRequest()
                {
                    BlackList = new Domain.BlackList()
                    {
                        Path = model.Path
                    },
                    Id = model.ID
                };
                isNameEx = _blacklistService.IsBlackListExist(request);
            }

            return Json(new { valid = !isNameEx }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(string Id="")
        {
            var response = _blacklistService.GetBlackList(new GetBlackListRequest()
            {
                Path = Id
            });
            var blackViewModel = new BlackListViewModel()
            {
                ID = response.BlackList.Path,
                Path = response.BlackList.Path
            };
            return View(blackViewModel);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit(BlackListViewModel model)
        {
            var request = new SaveBlackListRequest()
            {
                BlackList = new Domain.BlackList()
                {
                    Path = model.Path,
                    DateCre = DateTime.Now
                },
                Id = model.ID,
            };
            var isNameEx = _blacklistService.IsBlackListExist(request);
            if (!isNameEx)
            {
                var response = _blacklistService.SaveBlackList(request);
            }
            return RedirectToAction("Index", "Blacklist");
        }
    }
}