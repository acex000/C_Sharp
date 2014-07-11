
using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.IO;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;
using System.Security;
using System.Text.RegularExpressions;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;
using System.Net;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using DirectoryContentCopier;

namespace FileWatcherBizFi
{

    public class CheckForFile : System.ServiceProcess.ServiceBase
    {
        
        private int FIRE_TIMER = 1000;
        private Timer _processTimer = null;
        private System.ComponentModel.Container components = null;
        private string _BlobInLoc = string.Empty;
        public string _ConnectionStringBlobFile = string.Empty;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="DBLoaderService"/> class.
        /// </summary>
        public CheckForFile()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "CheckForUploadFilesBizFi";
        }


        private void Start()
        {

            OnStart(null);
        }


        /// <summary>
        /// Stops the service running in application mode.
        /// </summary>
        private void Stop()
        {
            OnStop();
        }



        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {

            if (_processTimer != null)
            {
                _processTimer.Dispose();
                _processTimer = null;
            }

            base.OnStop();
        }


        protected override void OnStart(string[] args)
        {

            try
            {
                base.OnStart(args);

                
                _processTimer =
                    new Timer(new TimerCallback(ProcessThread), null, 1000, FIRE_TIMER);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry(
                    "  Service failed to start. " + e.Message,
                    EventLogEntryType.Error);
                throw e;
            }

        }
        void ProcessThread(object obj)
        {
            _processTimer.Change(Timeout.Infinite, Timeout.Infinite);
            DateTime currentTime = DateTime.Now;
            executeProcess();
            System.GC.Collect();
            _processTimer.Change(300, 300);
        }
        private void executeProcess()
        {
            DirectoryContentCopier.DirectoryContentCopier copier = new DirectoryContentCopier.DirectoryContentCopier();
            string sourcePath = ConfigurationManager.AppSettings["sourcePath"];
            string destPath = ConfigurationManager.AppSettings["destPath"];
            DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
            DirectoryInfo destDir = new DirectoryInfo(destPath);
            DirectoryInfo movedFilesBackup = new DirectoryInfo(sourceDir.Parent.FullName + @"\movedFilesBackup\");
            if (!sourceDir.Exists)
            {
                EventLog.WriteEntry("SourcePath Not Exists!", EventLogEntryType.Error);
                ServiceController controller = new ServiceController(this.ServiceName);
                controller.Stop();
            }
            if (!destDir.Exists)
            {
                EventLog.WriteEntry("DestPath Not Exists!", EventLogEntryType.Error);
                ServiceController controller = new ServiceController(this.ServiceName);
                controller.Stop();
            }
            if (!movedFilesBackup.Exists)
            {
                Directory.CreateDirectory(movedFilesBackup.FullName);
            }
            
            try
            {
                copier.CopyAllContent(sourcePath, movedFilesBackup.FullName);
                copier.MoveAllContent(sourcePath, destPath);  
            }
            catch (Exception e)
            {

                EventLog.WriteEntry(e.Message, EventLogEntryType.Error, 3);
                ServiceController controller = new ServiceController(this.ServiceName);
                controller.Stop();
            }

            System.GC.Collect();

        }


       
 
    }
}


