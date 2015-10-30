using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public abstract class IICConfigItem
	{
		public static T CreateDefault<T>() where T: IICConfigItem
		{
			return (T)CreateDefault(typeof(T));
		}

		public static IICConfigItem CreateDefault(Type type)
		{
			IICConfigItemAttribute itemAttr = AttributeHelper.GetAttribute<IICConfigItemAttribute>(type);
			if (itemAttr.IsRequired) {
				throw new ConfigurationNotFoundException(IICConfigType.Item, type.Name);
			} else {
				IICConfigItem ret = (IICConfigItem)Activator.CreateInstance(type);
				ret.SetDefaultValue();
				return ret;
			}
		}

		public virtual void SetDefaultValue()
		{
			Type itemType = this.GetType();

			foreach (FieldInfo field in itemType.GetFields()) {
				IICConfigFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<IICConfigFieldAttribute>(field);
				if (fieldAttr != null) {
					if (fieldAttr.DefaultValue != null) {
						try {
							field.SetValue(this, fieldAttr.DefaultValue);
						} catch (Exception ex) {
							throw new ConfigurationException(
								string.Format("<{0}.{1}: {2}> DefaultValue Convert Failed: {3}", 
									itemType.Name, 
									field.Name,
									field.FieldType.Name,
									fieldAttr.DefaultValue),
								ex);
						}
					}
				}
			}
		}
	}
}
