using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public abstract class IICConfigSection
	{
		public static T CreateDefault<T>() where T: IICConfigSection
		{
			return (T)CreateDefault(typeof(T));
		}

		public static IICConfigSection CreateDefault(Type sectionType)
		{
			IICConfigSectionAttribute attr = AttributeHelper.GetAttribute<IICConfigSectionAttribute>(sectionType);
			if (attr.IsRequired) {
				return null;
			} else {
				IICConfigSection ret = (IICConfigSection)Activator.CreateInstance(sectionType);
				ret.SetDefaultValue();
				return ret;
			}
		}

		public virtual void SetDefaultValue()
		{
			Type sectionType = this.GetType();

			IICConfigSectionAttribute sectionAttr = AttributeHelper.GetAttribute<IICConfigSectionAttribute>(sectionType);

			foreach (FieldInfo field in sectionType.GetFields()) {
				IICConfigFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<IICConfigFieldAttribute>(field);
				if (fieldAttr != null && fieldAttr.DefaultValue != null) {
					ObjectHelper.SetValue(field, this, fieldAttr.DefaultValue);
					continue;
				}

				IICConfigItemAttribute itemAttr = AttributeHelper.TryGetAttribute<IICConfigItemAttribute>(field);
				if (itemAttr != null) {
					IICConfigItem item = (IICConfigItem)Activator.CreateInstance(field.FieldType);
					item.SetDefaultValue();
					field.SetValue(this, item);
					continue;
				}

				IICConfigItemCollectionAttribute colletionAttr = AttributeHelper.TryGetAttribute<IICConfigItemCollectionAttribute>(field);
				if (colletionAttr != null) {
					object collection = Activator.CreateInstance(field.FieldType);
					field.SetValue(this, collection);
					continue;
				}
			}
		}
	}
}
