namespace Imps.Services.HA
{
    partial class ProjectInstaller
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this._processInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this._serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // _processInstaller
            // 
            this._processInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this._processInstaller.Password = null;
            this._processInstaller.Username = null;
            // 
            // _serviceInstaller
            // 
            this._serviceInstaller.ServiceName = System.IO.Path.GetFileNameWithoutExtension(this.GetType().Assembly.CodeBase);
            this._serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                this._processInstaller,
                this._serviceInstaller
            });

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller _processInstaller;
        private System.ServiceProcess.ServiceInstaller _serviceInstaller;
    }
}