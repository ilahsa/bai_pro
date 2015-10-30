using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Samples
{
    public class ConfigSample1
    {
        public void Sample()
        {
            int port = IICConfigurationManager.Configurator.GetConfigField<int>("LocalSettings.SipcPort", OnPortUpdate);
            // start service on port
        }

        public void OnPortUpdate(int newValue)
        {
            // update localConfig
        }
    }

    [IICConfigItem("Database")]
    public class IICDbConfigItem : IICConfigItem
    {
        [IICConfigField("DbType")]
        public IICDbType DbType;

        [IICConfigField("DbName")]
        public string DbName;

        [IICConfigField("ConnectionString")]
        public string ConnectionString;
    }

    public class ConfigSample2
    {
        public void Sample()
        {
            IICDbConfigItem configItem = IICConfigurationManager.Configurator.GetConfigItem<IICDbConfigItem>("Databases", "IICUPDB.1", OnDbConfigUpdate);
            _db = DatabaseManager.GetDatabase(configItem.DbType, configItem.ConnectionString);
            // start service on port
        }

        public void OnDbConfigUpdate(IICDbConfigItem newConfig)
        {
            _db = DatabaseManager.GetDatabase(newConfig.DbType, newConfig.ConnectionString);
        }

        private Database _db;
    }   
}
