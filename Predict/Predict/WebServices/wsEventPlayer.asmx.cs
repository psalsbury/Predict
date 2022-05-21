using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Microsoft.AspNet.Identity;
using Predict.Models;

namespace Predict.WebServices
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class wsEventPlayer : System.Web.Services.WebService
    {

        [WebMethod(Description = "update Event Player Session", EnableSession = true)]
        public void UpdateEventPlayerSession()
        {
            var context = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();

            HttpSessionStateBase mySession = new HttpSessionStateWrapper(HttpContext.Current.Session);
            Predict.Helper.SessionHelper.UpdateEventPlayersSessionVariable(context, mySession, userId, true);
        }

    }
}
