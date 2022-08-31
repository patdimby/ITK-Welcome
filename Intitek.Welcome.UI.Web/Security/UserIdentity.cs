using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Front;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Security.Claims;
using System.Web;
using Intitek.Welcome.UI.ViewModels;
using Newtonsoft.Json;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace Intitek.Welcome.UI.Web.Security
{
    public static class UserTypes
    {
        public const string BACKOFFICE = "BACKOFFICE";
        public const string FRONTOFFICE = "FRONTOFFICE";
        public const string SPYOFFICE = "SPYOFFICE";
    }
    public class UserIdentity
    {
        private IUserService _userService;
        private IPrincipal Principal;
        public UserIdentity(IPrincipal Principal)
        {

            this.Principal = Principal;
        }

        public UserIdentity()
        {
        }
        /// <summary>
        /// Mise à jour des informations de l'utilisateur connecté
        /// </summary>
        /// <returns>false si user==null</returns>
        public bool UpdateUser()
        {
            if (_userService == null)
            {
                _userService = new UserService(new FileLogger());
            }

            var claimIdentity = this.Principal.Identity as ClaimsIdentity;
            //var user = _userService.GetIntitekUserByLogin(LoginName);
            var user = _userService.GetIntitekUserByEmail(LoginName, true);
            if (user == null)
            {
                return false;
            }
            var userVm = AutoMapperConfig.Mapper.Map<UserViewModel>(user.User);
            userVm.CompanyLogo = user.Logo; // "LogoIntitekGroup.png";
            userVm.NomPrenom = user.User.FullName; //string.IsNullOrEmpty(LongName) ? this.UserViewModel.Username : LongName;
            userVm.FirstName = user.User.FirstName;
            userVm.EntityName = user.User.EntityName;
            userVm.AgencyName = user.User.AgencyName;
            userVm.CompanyLogo = user.Logo;
            userVm.Status = user.User.Status.Value;
            userVm.ID_AD = user.User.ID_AD;

            
            var userDataClaim = claimIdentity.FindFirst(ClaimTypes.UserData);
            claimIdentity.RemoveClaim(userDataClaim);
            claimIdentity.AddClaim(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userVm)));
            var roleDataClaims = claimIdentity.FindAll(ClaimTypes.Role);
            foreach (var roleClaim in roleDataClaims)
            {
                claimIdentity.RemoveClaim(roleClaim);
            }
            if (userVm.Status == (int)Status.BACKOFFICE)
            {
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, UserTypes.BACKOFFICE));
            }
            if (userVm.Status == (int)Status.FRONTOFFICE)
            {
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, UserTypes.FRONTOFFICE));
            }
            if (userVm.Status == (int)Status.SPYOFFICE)
            {
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, UserTypes.SPYOFFICE));
            }
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            //authenticationManager.User = new ClaimsPrincipal(claimIdentity);
            AuthenticateResult authenticateResult = authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie).Result;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(claimIdentity), authenticateResult.Properties);
            return true;
        }
        public string LoginName
        {
            get
            {
                var claimIdentity = this.Principal.Identity as ClaimsIdentity;
                string login = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                return login;
            }           
        }
        public bool IsAuthorizeFrontOffice()
        {
            return this.Principal.Identity.IsAuthenticated && this.Principal.IsInRole(UserTypes.FRONTOFFICE);
        }
        public bool IsAuthorizeBackOffice()
        {
            return this.Principal.Identity.IsAuthenticated && this.Principal.IsInRole(UserTypes.BACKOFFICE);
        }
        public bool IsAuthorizeSpyOffice()
        {
            return this.Principal.Identity.IsAuthenticated && this.Principal.IsInRole(UserTypes.SPYOFFICE);
        }
    }
}