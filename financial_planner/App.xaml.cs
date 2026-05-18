using System.Windows;
using financial_planner.View;

namespace financial_planner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var authWindow = new AuthorizationWindow();
            authWindow.Show();
        }
    }
}