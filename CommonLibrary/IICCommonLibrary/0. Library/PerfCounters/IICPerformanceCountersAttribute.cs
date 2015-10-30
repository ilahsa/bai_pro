using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IICPerformanceCountersAttribute: Attribute
    {
        public IICPerformanceCountersAttribute(string catagoryName)
            : this(catagoryName, PerformanceCounterCategoryType.MultiInstance, catagoryName)
        {
        }

        public IICPerformanceCountersAttribute(string catagoryName, PerformanceCounterCategoryType catagoryType)
            : this(catagoryName, catagoryType, catagoryName)
        {
        }

        public IICPerformanceCountersAttribute(string categoryName, string categoryHelp)
            : this(categoryName, PerformanceCounterCategoryType.MultiInstance, categoryHelp)
        {
        }

        public IICPerformanceCountersAttribute(string categoryName, PerformanceCounterCategoryType categoryType, string categoryHelp)
        {
			CategoryName = categoryName;
			CategoryType = categoryType;
            CategoryHelp = categoryHelp;
        }

        public PerformanceCounterCategoryType CategoryType
        {
            get;
            set;
        }

        public string CategoryName
        {
            get; 
            set;
        }

        public string CategoryHelp
        {
            get;
            set;
        }
    }
}
