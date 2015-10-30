using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public class ConfigurationException: ApplicationException
	{
		public ConfigurationException()
		{
		}

		protected ConfigurationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ConfigurationException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}

	[Serializable]
	public class ConfigurationNotFoundException: ConfigurationException
	{
		public ConfigurationNotFoundException()
		{
		}

		protected ConfigurationNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ConfigurationNotFoundException(string path)
			: base(string.Format("ConfigurationNotFound {0}", path), null)
		{
			_type = IICConfigType.Field;
			_path = path;
		}

		public ConfigurationNotFoundException(IICConfigType type, string path)
			: base(string.Format("ConfigurationNotFound<{0}>: {1}", type, path), null)
		{
			_type = type;
			_path = path;
		}

		public ConfigurationNotFoundException(IICConfigType type, string path, string key)
			: base(string.Format("ConfigurationNotFound<{0}>: {1}.{2}", type, path, key), null)
		{
			_type = type;
			_path = path;
		}

		public ConfigurationNotFoundException(string path, string key, string field)
			: base(string.Format("ConfigurationNotFound: {0}.{1}#{2}", path, key, field), null)
		{
			_path = path;
			_key = key;
			_field = field;
		}

		private IICConfigType _type = IICConfigType.Item;
		private string _path;
		private string _key;
		private string _field;
	}

	[Serializable]
	public class ConfigurationFailedException: ConfigurationException
	{
		public ConfigurationFailedException()
		{
		}

		protected ConfigurationFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ConfigurationFailedException(IICConfigType type, string path, Exception e)
			: base(string.Format("ConfigFailed<{0}>, {1}", type, path), e)
		{
		}

		public ConfigurationFailedException(IICConfigType type, string path, string key, string field, Exception e)
			: base(string.Format("ConfigFailed<{0}> {1}.{2}#{3}", type, path, key, field), e)
		{
		}
	}
}
