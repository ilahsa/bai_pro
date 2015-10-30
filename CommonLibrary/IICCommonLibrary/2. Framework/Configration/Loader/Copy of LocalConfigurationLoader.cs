//using System;
//using System.IO;
//using System.Data;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Web.Configuration;
//using System.Configuration;
//using System.Text;
//using System.Web;
//using System.Xml;

//namespace Imps.Services.CommonV4.Configuration
//{
//    public class LocalConfigurationLoader: IConfigurationLoader
//    {
//        public LocalConfigurationLoader()
//        {
//            System.Configuration.Configuration config;
//            if (HttpRuntime.AppDomainId == null) {
//                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
//            } else {
//                config = WebConfigurationManager.OpenWebConfiguration(HttpRuntime.AppDomainAppVirtualPath);
//            }

//            foreach (KeyValueConfigurationElement elem in config.AppSettings.Settings) {
//                _appSettings.Add(elem.Key, elem.Value);
//            }

//            ConfigurationSection section = config.GetSection("LocalConfig");
//            if (section == null)
//                return;

//            string xml = section.SectionInformation.GetRawXml();
//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(xml);
//            foreach (XmlNode node in doc.FirstChild.ChildNodes) {
//                LoadSectionXml(node.OuterXml, node.Name);
//            }

//            _index.BuildIndex(_buffers);
//        }

//        public IICConfigFieldBuffer LoadConfigField(string key)
//        {
//            string value;
//            if (_appSettings.TryGetValue(key, out value)) {
//                IICConfigFieldBuffer buffer = new IICConfigFieldBuffer();
//                buffer.Key = key;
//                buffer.Value = value;
//                buffer.Version = DateTime.Now;
//                return buffer;
//            } else {
//                throw new ConfigurationNotFoundException(IICConfigType.Field, key);
//            }
//        }

//        public IList<IICConfigItemBuffer> LoadConfigSection(string path)
//        {
//            //
//            // TODO: a little Performance
//            List<IICConfigItemBuffer> ret = new List<IICConfigItemBuffer>();
//            foreach (IICConfigItemBuffer buffer in _buffers) {
//                if (buffer.Path.StartsWith(path))
//                    ret.Add(buffer);
//            }
//            return ret;
//        }

//        public IList<IICConfigItemBuffer> LoadConfigItem(string path, string key)
//        {
//            return _index.Find(path, key);
//        }

//        public string LoadConfigText(string path)
//        {
//            using (StreamReader reader = new StreamReader(path, Encoding.UTF8)) {
//                String text = reader.ReadToEnd();
//                return text;
//            }
//        }

//        public IICConfigTableBuffer LoadConfigTable(string tableName)
//        {
//            return LoadConfigTableFromDatabase(tableName);
//        }

//        public static IICConfigTableBuffer LoadConfigTableFromDatabase(string tableName)
//        {
//            Database hadb = DatabaseManager.GetDatabase("IMGCDB");
//            string[] parameters = { "@TableName" };
//            string loadParam;
//            string databaseName;
//            DateTime version;
//            using (DataReader reader = hadb.SpExecuteReader("USP_CenterLoadConfigTable", parameters, tableName)) {
//                if (!reader.Read()) {
//                    throw new ConfigurationNotFoundException(IICConfigType.Table, tableName);
//                }
//                databaseName = (string)reader["DatabaseName"];
//                loadParam = (string)reader["LoadParam"];
//                version = (DateTime)reader["Version"];
//            }

//            string[] parameters2 = { "@TableName" };
//            Database db = DatabaseManager.GetDatabase(databaseName);
//            DataTable table = db.SpExecuteTable("USP_LoadConfigTable", parameters2, loadParam);
//            IICConfigTableBuffer buffer = new IICConfigTableBuffer();
//            buffer.TableName = tableName;
//            buffer.Version = version;
//            int columnCount = table.Columns.Count;
//            buffer.ColumnNames = new string[columnCount];
//            for (int i = 0; i < columnCount; i++) {
//                buffer.ColumnNames[i] = table.Columns[i].ColumnName;
//            }

//            buffer.Rows = new List<RpcClass<string[]>>();
//            foreach (DataRow dataRow in table.Rows) {
//                string[] row = new string[columnCount];
//                for (int i = 0; i < columnCount; i++) {
//                    row[i] = dataRow[table.Columns[i]].ToString();
//                }
//                buffer.Rows.Add(new RpcClass<string[]>(row));
//            }
//            return buffer;
//        }

//        #region Private Methods
//        private void LoadSectionXml(string xml, string sectionName)
//        {
//            if (sectionName.ToLower() == "#comment")
//                return;

//            XmlDocument doc = new XmlDocument();
//            doc.LoadXml(xml);
			
//            XmlNode node = doc.FirstChild;
//            LoadInner(sectionName, node);
//        }

//        private void LoadInner(string rootPath, XmlNode node)
//        {
//            string key;
//            if (node.Name == "Item") {
//                key = node.Attributes[0].Value;
//            } else {
//                key = string.Empty;
//            }

//            if (node.Attributes != null) {
//                foreach (XmlAttribute attr in node.Attributes) {
//                    IICConfigItemBuffer buffer = new IICConfigItemBuffer();
//                    buffer.Path = rootPath;
//                    buffer.Field = attr.Name;
//                    buffer.Value = attr.Value;
//                    buffer.Key = key;

//                    _buffers.Add(buffer);
//                }
//            }

//            foreach (XmlNode childNode in node.ChildNodes) {
//                if (childNode.Name == "Item")
//                    LoadInner(rootPath, childNode);
//                else
//                    LoadInner(rootPath + "." + childNode.Name, childNode);
//            }
//        }
//        #endregion

//        private IICIndex<IICConfigItemBuffer> _index = new IICIndex<IICConfigItemBuffer>("Path", "Key");
//        private Dictionary<string, string> _appSettings = new Dictionary<string, string>();
//        private List<IICConfigItemBuffer> _buffers = new List<IICConfigItemBuffer>();
//    }
//}
