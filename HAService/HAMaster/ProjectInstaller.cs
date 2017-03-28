using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.Configuration;

namespace Imps.Services.HA
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            XmlDocument doc = new XmlDocument();
            doc.Load(this.GetType().Assembly.CodeBase + ".config");

            XmlNode node = doc.SelectSingleNode("/configuration/appSettings/add[@key=\"ServiceName\"]");
            this._serviceInstaller.ServiceName = node.Attributes["value"].Value;

            node = doc.SelectSingleNode("/configuration/appSettings/add[@key=\"DisplayName\"]");
            this._serviceInstaller.DisplayName = node.Attributes["value"].Value;

            node = doc.SelectSingleNode("/configuration/appSettings/add[@key=\"Description\"]");
            this._serviceInstaller.Description = node.Attributes["value"].Value;
        }
    }
}