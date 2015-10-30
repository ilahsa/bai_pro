using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	/// <summary>
	///		实现类, 获取更新, 并且执行OnUpdate方法
	/// </summary>
	public class ConfiguratorImp: IConfigurator
	{
		#region IConfigurator Members
		public T GetConfigField<T>(string key, Action<T> onUpdate)
		{
			if (IICConfigurationManager.Loader == null)
				throw new InvalidOperationException("No Loader Yet, You *MUST* call ServiceSettings.InitService() at First");

			IICConfigFieldBuffer buffer = IICConfigurationManager.Loader.LoadConfigField(key);
			try {
				T ret = ObjectHelper.ConvertTo<T>(buffer.Value);

				if (onUpdate != null) {
					onUpdate(ret);
				}

				return ret;
			} catch (Exception ex) {
				throw new ConfigurationFailedException(IICConfigType.Field, key, ex);
			}
		}

		public T GetConfigItem<T>(string path, string key, Action<T> onUpdate) where T: IICConfigItem
		{
			if (IICConfigurationManager.Loader == null)
				throw new InvalidOperationException("No Loader Yet, You *MUST* call ServiceSettings.InitService() at First");

			try {
				IList<IICConfigItemBuffer> buffers = IICConfigurationManager.Loader.LoadConfigItem(path, key);
				T ret;
				if (buffers == null || buffers.Count == 0) {
					ret = IICConfigItem.CreateDefault<T>();
				} else {
					IICIndex<IICConfigItemBuffer> index = new IICIndex<IICConfigItemBuffer>("Path", "Key", "Field");
					index.BuildIndex(buffers);
					ret = (T)LoadItem(path, key, typeof(T), index);
				}

				if (onUpdate != null) {
					onUpdate(ret);
				}

				return ret;
			} catch (Exception ex) {
				throw new ConfigurationFailedException(IICConfigType.Item, path + "." + key, ex);
			}
		}

		public IICConfigItemCollection<K, V> GetConfigItems<K, V>(string path, Action<IICConfigItemCollection<K, V>> onUpdate) where V: IICConfigItem
		{
			if (IICConfigurationManager.Loader == null)
				throw new InvalidOperationException("No Loader Yet, You *MUST* call ServiceSettings.InitService() at First");

			try {
				IList<IICConfigItemBuffer> buffers = IICConfigurationManager.Loader.LoadConfigItem(path, "");
				IICIndex<IICConfigItemBuffer> index = new IICIndex<IICConfigItemBuffer>("Path", "Key", "Field");
				if (buffers != null) {
					index.BuildIndex(buffers);
				}

				IICConfigItemCollection<K, V> ret = (IICConfigItemCollection<K, V>)LoadCollection(path, typeof(K), typeof(V), typeof(IICConfigItemCollection<K, V>), index);
				if (onUpdate != null) {
					onUpdate(ret);
				}
				return ret;
			} catch (Exception ex) {
				throw new ConfigurationFailedException(IICConfigType.ItemCollection, path, ex);
			}
		}

		public T GetConfigSecion<T>(string path, Action<T> onUpdate) where T: IICConfigSection
		{
			if (IICConfigurationManager.Loader == null)
				throw new InvalidOperationException("No Loader Yet, You *MUST* call ServiceSettings.InitService() at First");

			try {
				IList<IICConfigItemBuffer> buffers = IICConfigurationManager.Loader.LoadConfigSection(path);
				T ret;
				if (buffers == null || buffers.Count == 0) {
					ret = IICConfigSection.CreateDefault<T>();
				} else {
					IICIndex<IICConfigItemBuffer> index = new IICIndex<IICConfigItemBuffer>("Path", "Key", "Field");
					index.BuildIndex(buffers);

					ret = LoadSection<T>(path, index);
				}

				if (onUpdate != null) {
					onUpdate(ret);
				}

				return ret;
			} catch (Exception ex) {
				throw new ConfigurationFailedException(IICConfigType.Section, path, ex);
			}
		}

		public string GetConfigText(string path, Action<string> onUpdate)
		{
			if (IICConfigurationManager.Loader == null)
				throw new InvalidOperationException("No Loader Yet, You *MUST* call ServiceSettings.InitService() at First");

			try {
				string text = IICConfigurationManager.Loader.LoadConfigText(path);

				if (onUpdate != null) {
					onUpdate(text);
				}

				return text;
			} catch (Exception ex) {
				throw new ConfigurationFailedException(IICConfigType.Text, path, ex);
			}
		}

		public IICCodeTable<K, V> GetCodeTable<K, V>(string tableName, Action<IICCodeTable<K, V>> onUpdate) where V: IICCodeTableItem
		{
			if (IICConfigurationManager.Loader == null)
				throw new InvalidOperationException("No loader yet, You *MUST* call ServiceSettings.InitService() at First");

			try {
				IICConfigTableBuffer buffer = IICConfigurationManager.Loader.LoadConfigTable(tableName);
				if (buffer == null)
					throw new ConfigurationNotFoundException(IICConfigType.Table, tableName);

				IICCodeTable<K, V> table = new IICCodeTable<K, V>(buffer);
				table.RunAfterLoad();

				if (onUpdate != null) {
					onUpdate(table);
				}
				return table;
			} catch (Exception ex) {
				throw new ConfigurationFailedException(IICConfigType.Table, tableName, ex);
			}
		}
		#endregion

		#region Private Methods
		private T LoadSection<T>(string sectionName, IICIndex<IICConfigItemBuffer> index) where T: IICConfigSection
		{
			IICConfigSectionAttribute sectionAttr = AttributeHelper.GetAttribute<IICConfigSectionAttribute>(typeof(T));
			T section = Activator.CreateInstance<T>();

			foreach (FieldInfo field in typeof(T).GetFields()) {
				IICConfigFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<IICConfigFieldAttribute>(field);
				if (fieldAttr != null) {
					IICConfigItemBuffer item = index.TryFindOne(sectionAttr.SectionName, string.Empty, fieldAttr.FieldName);
					if (item != null) {
						ObjectHelper.SetValue(field, section, item.Value, field.FieldType);
					} else {
						ObjectHelper.SetValue(field, section, fieldAttr.DefaultValue);
					}
					continue;
				}

				IICConfigItemAttribute itemAttr = AttributeHelper.TryGetAttribute<IICConfigItemAttribute>(field);
				if (itemAttr != null) {
					object item = LoadItem(sectionName + "." + itemAttr.ItemName, string.Empty, field.FieldType, index);
					field.SetValue(section, item);
					continue;
				}

				IICConfigItemCollectionAttribute colletionAttr = AttributeHelper.TryGetAttribute<IICConfigItemCollectionAttribute>(field);
				if (colletionAttr != null) {

					Type[] types = field.FieldType.GetGenericArguments();
					Type keyType = types[0];
					Type itemType = types[1];
					IConfigCollection collection = LoadCollection(sectionName + "." + colletionAttr.ItemName, keyType, itemType, field.FieldType, index);
					field.SetValue(section, collection);
					continue;
				}
			}
			return section;
		}


		private IConfigCollection LoadCollection(string path, Type keyType, Type itemType, Type fieldType, IICIndex<IICConfigItemBuffer> index)
		{
			IList<IICConfigItemBuffer> items = index.Find(path);

			string lastKey = null;
			List<string> keys = new List<string>();
			foreach (IICConfigItemBuffer buffer in items) {
				if (string.IsNullOrEmpty(lastKey) || buffer.Key != lastKey) {
					if (!string.IsNullOrEmpty(buffer.Key)) {
						keys.Add(buffer.Key);
						lastKey = buffer.Key;
					}
				}
			}

			IConfigCollection collection = (IConfigCollection)Activator.CreateInstance(fieldType);
			foreach (string key in keys) {
				object itemObj = LoadItem(path, key, itemType, index);
				object keyObj = ObjectHelper.ConvertTo(key, keyType);
				collection.Add(keyObj, itemObj);
			}
			return collection;
		}

		private object LoadItem(string path, string key, Type itemType, IICIndex<IICConfigItemBuffer> index)
		{
			IICConfigItemAttribute itemAttr = AttributeHelper.GetAttribute<IICConfigItemAttribute>(itemType);
			object ret = Activator.CreateInstance(itemType);
			foreach (FieldInfo field in itemType.GetFields()) {
				IICConfigFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<IICConfigFieldAttribute>(field);
				if (fieldAttr != null) {
					IICConfigItemBuffer buffer = index.TryFindOne(path, key, fieldAttr.FieldName);
					if (buffer != null) {
						ObjectHelper.SetValue(field, ret, buffer.Value, field.FieldType);
					} else if (fieldAttr.DefaultValue != null) {
						ObjectHelper.SetValue(field, ret, fieldAttr.DefaultValue);
					} else {
						throw new ConfigurationNotFoundException(path, key, field.Name);
					}
				}
			}
			return ret;
		}
		#endregion
	}
}
