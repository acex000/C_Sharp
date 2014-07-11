
#if DEBUG
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Reflection;

namespace FileWatcherBizFi
{
    /// <summary>
    /// Summary description for ServiceController.
    /// </summary>
    public class ServiceControllerForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private ServiceBase _service;
        private MethodInfo _startMethod;
        private MethodInfo _stopMethod;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="ServiceControllerForm"/> class.
        /// </summary>
        /// <param name="service"></param>
        public ServiceControllerForm(ServiceBase service)
        {
            _service = service;
            InitializeComponent();
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


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(10, 9);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(115, 27);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(144, 9);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(115, 27);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // ServiceControllerForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(288, 64);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Name = "ServiceControllerForm";
            this.Text = "Service Controller";
            this.Load += new System.EventHandler(this.ServiceControllerForm_Load);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Handles the onload event of the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceControllerForm_Load(object sender, System.EventArgs e)
        {
            Type svcType = _service.GetType();
            _startMethod = svcType.GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _stopMethod = svcType.GetMethod("Stop", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (_startMethod == null)
            {
                MessageBox.Show("Start method not defined on target service. Cannot run service as application.");
                Application.Exit();
            }

            else if (_stopMethod == null)
            {
                MessageBox.Show("Stop method not defined on target service. Cannot run service as application.");
                Application.Exit();
            }
        }


        /// <summary>
        /// Handles the Start button's click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                _startMethod.Invoke(_service, null);
                startButton.Enabled = false;
                stopButton.Enabled = true;
            }
            catch (Exception ex)
            {
                startButton.Enabled = false;
                stopButton.Enabled = false;
            }

        }


        /// <summary>
        /// Handles the Stop button's click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, System.EventArgs e)
        {
            _stopMethod.Invoke(_service, null);
            startButton.Enabled = true;
            stopButton.Enabled = false;
            Application.Exit();
        }
    }
}
#endif

