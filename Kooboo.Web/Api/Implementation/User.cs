//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Api.ApiResponse;
using System;
using System.Collections.Generic;
using Kooboo.Api;
using System.IO;
using Kooboo.Lib.Helper;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Api.Implementation
{
    public class UserApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "User";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public MetaResponse Login(string UserName, string Password, ApiCall apiCall)
        {
            var user = Kooboo.Data.GlobalDb.Users.Validate(UserName, Password);

            if (user != null)
            {
                string remember = apiCall.GetValue("remember");

                string returnUrl = apiCall.GetValue("returnurl");
                if (returnUrl != null)
                {
                    returnUrl = System.Web.HttpUtility.UrlDecode(returnUrl);
                }
                bool isRemember = false;
                if (!string.IsNullOrEmpty(remember))
                {
                    bool.TryParse(remember, out isRemember);
                }

                int days = isRemember ? 60 : 0;
                var response = new MetaResponse();

                response.Success = true;
                string redirct = Kooboo.Web.Service.UserService.GetLoginRedirectUrl(apiCall.Context, user, apiCall.Context.Request.Url, returnUrl);

                if (isRemember)
                {
                    redirct = Lib.Helper.UrlHelper.AppendQueryString(redirct, "remember", "yes");
                }

                response.Model = redirct;
                // resposne redirect url. for online and local version...  
                return response;
            }
            var noresponse = new MetaResponse();
            noresponse.Success = false;
            noresponse.Messages.Add(Data.Language.Hardcoded.GetValue("User name or password not valid", apiCall.Context));
            return noresponse;
        }

        public virtual string GetRegisterRedirectUrl(User user, string currentrequesturl)
        {

            var newuser = Kooboo.Data.GlobalDb.Users.Validate(user.UserName, user.Password);  // this is to ensure that local has the user detail info like username, password, orgname, etc...  
            return Lib.Helper.UrlHelper.Combine(currentrequesturl, "/_admin/sites");

        }

        public virtual MetaResponse Register(string UserName, string Password, string email, ApiCall apiCall)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Username or password not provided", apiCall.Context));
            }
            var currentuser = Kooboo.Data.GlobalDb.Users.Get(UserName);
            if (currentuser != null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("user exists", apiCall.Context));
            }
            var user = new User();
            user.UserName = UserName;
            user.Password = Password;
            user.EmailAddress = email;
            string acceptlang = apiCall.Context.Request.Headers["Accept-Language"];
            user.Language = Kooboo.Data.Language.LanguageSetting.GetByAcceptLangHeader(acceptlang);

            var ok = GlobalDb.Users.Register(user);

            if (ok)
            {
                var response = new MetaResponse();
                response.AppendCookie(DataConstants.UserApiSessionKey, user.Id.ToString(), 10);
                response.Success = true;

                response.Model = GetRegisterRedirectUrl(user, apiCall.Context.Request.Url);
                return response;
            }
            else
            {
                throw new Exception("User registration failed");
            }
        }


        [Kooboo.Attributes.RequireModel(typeof(User))]
        public bool UpdateProfile(ApiCall call)
        {
            var newuser = call.Context.Request.Model as User;
            var user = call.Context.User;
            user.UserName = newuser.UserName;
            user.Language = newuser.Language;
            user.EmailAddress = newuser.EmailAddress;
            if (GlobalDb.Users.AddOrUpdate(user))
            {
                call.Context.User = user;
                return true;
            }
            return false;
        }

        public User GetUser(ApiCall call)
        {
            var user = GlobalDb.Users.Profile(call.Context.User.Id);

            if (user != null)
            {
                var org = GlobalDb.Organization.Get(user.CurrentOrgId);
                if (org != null)
                {
                    if (org.AdminUser == user.Id)
                    {
                        user.IsAdmin = true;
                    }

                }
                else
                {
                    user.IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
                }
            }

            return user;
        }


        public MetaResponse ChangePassword(string UserName, string OldPassword, string NewPassword, ApiCall call)
        {
            bool isSuccess = GlobalDb.Users.ChangePassword(UserName, OldPassword, NewPassword);
            MetaResponse response = new MetaResponse();
            response.Success = isSuccess;
            return response;
        }

        public bool CheckUser(string username, ApiCall call)
        {
            var user = Kooboo.Data.GlobalDb.Users.Get(username);
            return user != null;
        }

        public Dictionary<string, string> Culture(ApiCall call)
        {
            return Kooboo.Data.Language.LanguageSetting.CmsLangs;
        }


        public virtual string ForgotPassword(string Email, ApiCall call)
        {
            string serverulr = GlobalDb.Users.GetServerUrl(Email);

            string url = serverulr + "/_api/User/ForgotPassword";
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Email", Email);
            return Lib.Helper.HttpHelper.Get<string>(url, para);

        }

        public string ResetPassword(Guid token, string password, ApiCall call)
        {
            if (GlobalDb.Users.ResetPassword(token, password))
            {
                return Hardcoded.GetValue("Your password has been reset", call.Context);
            }
            else
            {
                return Hardcoded.GetValue("Password reset failed", call.Context);
            }

        }
        public string GetLanguage(ApiCall call)
        {
            return call.Context.User.Language ?? "en";
        }

        public MetaResponse Logout(ApiCall apiCall)
        {
            var response = new MetaResponse();
            response.DeleteCookie(DataConstants.UserApiSessionKey);
            response.Success = true;
            return response;
        }


        public bool IsUniqueName(string name, ApiCall apiCall)
        {
            var user = Kooboo.Data.GlobalDb.Users.Get(name);
            if (user != null)
            {
                return false;
            }
            return true;
        }

        public bool IsUniqueEmail(string email, ApiCall call)
        {
            return Kooboo.Data.GlobalDb.Users.GetByEmail(email) == null;
        }

    }
}
