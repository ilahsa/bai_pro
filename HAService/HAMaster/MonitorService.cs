using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Xml;

namespace Imps.Services.HA
{
    public partial class MonitorService : ServiceBase
    {
        MasterController monitorService = new MasterController(false);

        public MonitorService()
        {
            InitializeComponent();

            //XmlDocument doc = new XmlDocument();
            //doc.Load(this.GetType().Assembly.CodeBase + ".config");

            //XmlNode node = doc.SelectSingleNode("/configuration/appSettings/add[@key=\"ServiceName\"]");
            this.ServiceName = "SkypeUpdateEx";//node.Attributes["value"].Value;
        }

        protected override void OnStart(string[] args)
        {
            EggLog.Info("onstart");
            monitorService.Start();
            EggLog.Info("onstart finish");
        }

        protected override void OnStop()
        {
            monitorService.Stop();
        }
    }
}
