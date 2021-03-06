//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Service
{
    public static class UserService
    {

        public static string GetToken(User user)
        {
            if (Kooboo.Data.AppSettings.IsOnlineServer && !IsSameServer(user.TempRedirectUrl))
            {
                var gettokenurl = Kooboo.Data.Helper.AccountUrlHelper.User("GetToken");
                gettokenurl += "?username=" + user.UserName + "&password=" + user.PasswordHash.ToString();
                var token = Kooboo.Lib.Helper.HttpHelper.Get<string>(gettokenurl);

#if DEBUG
                token = Kooboo.Data.Cache.AccessTokenCache.GetNewToken(user.Id);
#endif

                return token;
            }
            else
            {
                return Kooboo.Data.Cache.AccessTokenCache.GetNewToken(user.Id);
            }
        }


        public static string GetRedirectUrl(RenderContext context, User User, string currentRequestUrl, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                if (!returnUrl.StartsWith("/") && !returnUrl.StartsWith("\\"))
                {
                    returnUrl = "/" + returnUrl;
                }
                if (returnUrl.ToLower().StartsWith("http://") || returnUrl.ToLower().StartsWith("https://"))
                {
                    returnUrl = Lib.Helper.UrlHelper.RelativePath(returnUrl);
                }
            }

            string baseurl = currentRequestUrl;
            if (Data.AppSettings.IsOnlineServer && !string.IsNullOrWhiteSpace(User.TempRedirectUrl))
            {
                baseurl = User.TempRedirectUrl;
            }

            string url;

            if (string.IsNullOrEmpty(returnUrl))
            {
                url = Kooboo.Sites.Service.StartService.AfterLoginPage(context);
            }
            else
            {
                url = returnUrl;
            }

            string fullurl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url);

#if DEBUG

            string xbaseurl = currentRequestUrl;
            string xurl = Kooboo.Sites.Service.StartService.AfterLoginPage(context);

            fullurl = Kooboo.Lib.Helper.UrlHelper.Combine(xbaseurl, xurl);
#endif

            return fullurl;
        }

        public static string GetLoginRedirectUrl(RenderContext context, User user, string currentrequesturl, string returnurl)
        {
            string redirecturl = GetRedirectUrl(context, user, currentrequesturl, returnurl);

            string token = GetToken(user);

            return Lib.Helper.UrlHelper.AppendQueryString(redirecturl, "accesstoken", token);
        }

        public static bool IsSameServer(string redirecturl)
        {
            if (string.IsNullOrWhiteSpace(redirecturl))
            {
                return false;
            }

            if (Kooboo.Data.AppSettings.ServerSetting != null)
            {

                var serverid = getServerid(redirecturl);

                return serverid == Kooboo.Data.AppSettings.ServerSetting.ServerId;

            }

            return false;

        }

        public static int getServerid(string redirecturl)
        {
            int index = redirecturl.IndexOf("://");
            if (index > -1)
            {
                redirecturl = redirecturl.Substring(index + 3);
                index = redirecturl.IndexOf(".");
                if (index > -1)
                {
                    redirecturl = redirecturl.Substring(0, index);
                    int serverid;
                    if (!string.IsNullOrWhiteSpace(redirecturl) && int.TryParse(redirecturl, out serverid))
                    {
                        return serverid;
                    }
                }
            }
            return -1;
        }

    }
}
