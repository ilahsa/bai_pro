using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	public enum IICConfigType
	{
		Field,
		Item,
		ItemCollection,
		Section,
		Table,
		Text,
	}

	abstract class ConfigUpdater
	{
		protected string p_path;

		protected string p_key;

		protected IICConfigType p_type;

		protected DateTime p_Version;

		public string Path
		{
			get { return p_path; }
		}

		public string Key
		{
			get { return p_key; }
		}

		public IICConfigType Type
		{
			get { return p_type; }
		}

		public DateTime Version
		{
			set { p_Version = value; }
			get { return p_Version; }
		}

		public ConfigUpdater(string path, string key, IICConfigType type)
		{
			p_path = path;
			p_key = key;
			p_type = type;
		}

		public override string ToString()
		{
			return string.Format("ConfigUpdater<{0}> {1}.{2}", p_type, p_path, p_key);
		}

		public abstract void OnUpdate();
	}
}