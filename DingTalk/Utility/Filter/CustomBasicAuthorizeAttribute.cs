using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
using System.Reflection;
using Ruanmou.SOA.Web.Utility.Filters;

namespace DingTalk.Utility.Filters
{
    public class CustomBasicAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// action前会先来这里完成权限校验
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //actionContext.Request.Headers["Authorization"]
            if (actionContext.ActionDescriptor.GetCustomAttributes<CustomAllowAnonymousAttribute>().FirstOrDefault() != null)
            {
                return;//继续
            }
            else if (actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<CustomAllowAnonymousAttribute>().FirstOrDefault() != null)
            {
                return;//继续
            }
            else
            {
                var authorization = actionContext.Request.Headers.Authorization;
                if (authorization == null)
                {
                    this.HandlerUnAuthorization();
                }
                else if (this.ValidateTicket(authorization.Parameter))
                {
                    return;//继续
                }
                else
                {
                    this.HandlerUnAuthorization();
                }
            }
        }

        private void HandlerUnAuthorization()
        {
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
        }
        private bool ValidateTicket(string encryptTicket)
        {
            ////解密Ticket
            //if (string.IsNullOrWhiteSpace(encryptTicket))
            //    return false;
            try
            {
                var strTicket = FormsAuthentication.Decrypt(encryptTicket).UserData;
                //FormsAuthentication.Decrypt(encryptTicket).
                return string.Equals(strTicket, string.Format("{0}&{1}", "Admin", "123456"));//应该分拆后去数据库验证
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}