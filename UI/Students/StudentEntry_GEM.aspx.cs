using System;
using System.Web.UI;

namespace SMS_Gem.UI.Students
{
    public partial class StudentEntry_GEM : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // All CRUD logic is handled via Web API (/api/StudentApi/...)
            // Ensure ViewState is disabled on the page for optimal performance
            this.EnableViewState = false;
        }
    }
}