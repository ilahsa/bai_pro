using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	//
	// keyName 规则
	//
	//	/Section
	public interface IConfigurator
	{
		/// <summary>
		///		Local:		<AppSettings/>
		///		Server:		HaConfigTable: Key->Value
		/// </summary>
		T GetConfigField<T>(string key, Action<T> onUpdate);

		/// <summary>
		///		Local:		Section->Item
		///		Server:		HaConfigTable: Key->Value
		/// </summary>
		T GetConfigItem<T>(string path, string key, Action<T> onUpdate) where T: IICConfigItem;
		
		/// <summary>
		///		Local:		Section->Items/Item
		///		Server:		HaConfigTable: Section->Items
		/// </summary>
		IICConfigItemCollection<K, V> GetConfigItems<K, V>(string path, Action<IICConfigItemCollection<K, V>> onUpdate) where V: IICConfigItem;

		/// <summary>
		///		Local:		<ConfigSections/>
		///		Server:		HaConfigTable: Section
		/// </summary>
		T GetConfigSecion<T>(string sectionName, Action<T> onUpdate) where T: IICConfigSection;
		
		/// <summary>
		///		获取一段配置文本 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="onUpdate"></param>
		/// <returns></returns>
		string GetConfigText(string path, Action<string> onUpdate);

        /// <summary>
        ///     GetConfiguration
        /// </summary>
		IICCodeTable<K, V> GetCodeTable<K, V>(string tableName, Action<IICCodeTable<K, V>> onUpdate) 
			where V : IICCodeTableItem;
	}
}