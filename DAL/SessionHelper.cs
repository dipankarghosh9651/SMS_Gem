using System.Configuration;
using System.Web;

namespace SMS_Gem.DAL
{
    public static class SessionHelper
    {
        public static string CurrentBranch(HttpContext context)
        {
            if (context?.Session?["BranchCode"] != null)
            {
                return context.Session["BranchCode"].ToString();
            }
            return ConfigurationManager.AppSettings["Default.Branch"] ?? "CAP";
        }

        public static string CurrentUser(HttpContext context)
        {
            if (context?.Session?["UserId"] != null)
            {
                return context.Session["UserId"].ToString();
            }
            return "SystemAdmin";
        }
    }
}