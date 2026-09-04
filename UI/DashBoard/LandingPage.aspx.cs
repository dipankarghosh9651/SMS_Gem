using System;
using System.Web.UI;

namespace SMS_Gem.UI.DashBoard
{
    public partial class LandingPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.EnableViewState = false;
        }
    }
}