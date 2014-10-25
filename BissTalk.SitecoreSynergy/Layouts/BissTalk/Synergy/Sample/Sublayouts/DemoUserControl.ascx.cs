using System;
using System.Linq;

namespace BissTalk.SitecoreSynergy.Layouts.BissTalk.Synergy.Sample.Sublayouts
{
    public partial class DemoUserControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            var item = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);
            var datasource = item.Children.Select(i => new {i.Name, Path = i.Paths.FullPath});
            RowRepeater.DataSource = datasource;
            RowRepeater.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            OutputStuff.Text += "<h1>HELLO WORLD!!!!</h1>";
        }
    }
}