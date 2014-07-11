  

using System;
using System.Windows.Forms;
using System.ServiceProcess;

namespace FileWatcherBizFi
{
    /// <summary>
    /// Contains the main entry point of the application.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            ServiceBase serviceToStart = null;
            serviceToStart = new CheckForFile();

            if (serviceToStart == null)
            {
                MessageBox.Show("Specified service could not be found. Cannot start service in application mode");
                return;
            }
            else
            {
                ServiceControllerForm form = new ServiceControllerForm(serviceToStart);
                Application.Run(form);
            }

#else
        ServiceBase.Run (new ServiceBase[] {new CheckForFile ()});
#endif
        }
    }
}



