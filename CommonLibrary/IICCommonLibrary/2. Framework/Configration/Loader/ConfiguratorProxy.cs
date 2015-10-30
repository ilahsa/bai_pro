using System;
using System.Web;
using System.Web.Configuration;

using System.Data;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

using Imps.Services.CommonV4.Configuration;

//
// TODO
// 报错信息 
namespace Imps.Services.CommonV4
{
	public class ConfiguratorProxy: IConfigurator
	{
		private IConfigurator _imp;
		private object _syncRoot = new object();
		private List<ConfigUpdater> _updaters = new List<ConfigUpdater>();
		private IICIndex<ConfigUpdater> _index = new IICIndex<ConfigUpdater>("p_path", "p_key", "p_type");

		public ConfiguratorProxy(IConfigurator imp)
		{
			_imp = imp;
		}

		public T GetConfigField<T>(string key, Action<T> onUpdate)
		{
			ConfigFieldUpdater<T> updater = new ConfigFieldUpdater<T>(key, onUpdate);
			lock (_syncRoot) {
				_updaters.Add(updater);
				_index.BuildIndex(_updaters);
			}

			return _imp.GetConfigField<T>(key, onUpdate);
		}

		public T GetConfigItem<T>(string path, string key, Action<T> onUpdate) where T: IICConfigItem
		{
			ConfigItemUpdater<T> updater = new ConfigItemUpdater<T>(path, key, onUpdate);
			lock (_syncRoot) {
				_updaters.Add(updater);
				_index.BuildIndex(_updaters);
			}

			return _imp.GetConfigItem<T>(path, key, onUpdate);
		}

		public IICConfigItemCollection<K, V> GetConfigItems<K, V>(string path, Action<IICConfigItemCollection<K, V>> onUpdate) where V: IICConfigItem
		{
			ConfigItemCollectinoUpdater<K, V> updater = new ConfigItemCollectinoUpdater<K, V>(path, onUpdate);
			lock (_syncRoot) {
				_updaters.Add(updater);
				_index.BuildIndex(_updaters);
			}

			return _imp.GetConfigItems<K, V>(path, onUpdate);
		}

		public T GetConfigSecion<T>(string sectionName, Action<T> onUpdate) where T: IICConfigSection
		{
			ConfigSectionUpdater<T> updater = new ConfigSectionUpdater<T>(sectionName, onUpdate);
			lock (_syncRoot) {
				_updaters.Add(updater);
				_index.BuildIndex(_updaters);
			}

			return _imp.GetConfigSecion<T>(sectionName, onUpdate);
		}

		public string GetConfigText(string path, Action<string> onUpdate)
		{
			ConfigTextUpdater updater = new ConfigTextUpdater(path, onUpdate);
			lock (_syncRoot) {
				_updaters.Add(updater);
				_index.BuildIndex(_updaters);
			}
			return _imp.GetConfigText(path, onUpdate);
		}

		public IICCodeTable<K, V> GetCodeTable<K, V>(string tableName, Action<IICCodeTable<K, V>> onUpdate) where V: IICCodeTableItem
		{
			ConfigTableUpdater<K, V> updater = new ConfigTableUpdater<K, V>(tableName, onUpdate);
			lock (_syncRoot) {
				_updaters.Add(updater);
				_index.BuildIndex(_updaters);
			}

			IICCodeTable<K, V> table = _imp.GetCodeTable<K, V>(tableName, onUpdate);
			updater.Version = table.Version;
			return table;
		}

		public void DoUpdate(string path, string key, IICConfigType configType)
		{
			key = key ?? "";
			IList<ConfigUpdater> updaters = _index.Find(path, key, configType);
			if (updaters.Count == 0) {
				throw new Exception(string.Format("ConfigUpdater not found {0}:{1}.{2}", configType, path, key));
			}
			foreach (ConfigUpdater updater in updaters) {
				try {
					updater.OnUpdate();
				} catch (Exception ex) {
					SystemLog.Error(LogEventID.ConfigFailed, ex, "Config UpdateFailed - {0}", updater.ToString());
					throw;
				}
			}
		}
	}
}
