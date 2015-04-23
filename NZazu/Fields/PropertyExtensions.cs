using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace NZazu.Fields
{
    [ExcludeFromCodeCoverage]
    static class PropertyExtensions
    {
        public static bool CanSetProperty<T>(this T instance, string propertyName)
        {
            object obj = instance;
            var type = instance.GetType();
            var propName = propertyName;

            int p = propName.IndexOf('.');
            while (p != -1) // handle child-property
            {
                var parent = propName.Substring(0, p);
                // test if a corresponding property exists for the given type.
                var propInfo = type.GetProperty(parent, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null)
                    throw new ArgumentNullException(String.Format("Property \"{0}\" does not exist.", propName));

                propName = propName.Substring(p + 1);
                obj = propInfo.GetValue(obj, null);
                type = obj.GetType();
                p = propName.IndexOf('.');
            }

            var propertyInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            return (propertyInfo != null) && HasPublicGetterAndSetter(propertyInfo);
        }

        public static void SetProperty(this object item, string propertyName, string propValue)
        {
            while (true)
            {
                var type = item.GetType();
                var propName = propertyName;
                var p = propertyName.IndexOf('.');
                if (p != -1) // handle child-property
                {
                    propName = propertyName.Substring(0, p);

                    // test if a corresponding property exists for the given type.
                    var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                        throw new ArgumentNullException(String.Format("Property \"{0}\" does not exist.", propName));

                    var childPropName = propertyName.Substring(p + 1);
                    var childItem = propInfo.GetValue(item, null);

                    // traverse down to child
                    item = childItem;
                    propertyName = childPropName;
                    continue;
                }
                else
                {
                    // test if a corresponding property exists for the given type.
                    var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                        throw new ArgumentNullException(String.Format("Property \"{0}\" does not exist.", propName));

                    // deserialize using TypeConverters
                    var outVal = GetConvertedValue(propValue, propInfo);

                    // if value could be converted from string, set it on the property.
                    if (outVal != null)
                        propInfo.SetValue(item, outVal, null);
                }
                break;
            }
        }

        private static bool HasPublicGetterAndSetter(PropertyInfo p)
        {
            // cf. http://stackoverflow.com/questions/3762456/c-sharp-how-to-check-if-property-setter-is-public
            return p.GetGetMethod(true).IsPublic && p.CanWrite && p.GetSetMethod(true).IsPublic;
        }

        private static object GetConvertedValue(string propValue, PropertyInfo propInfo)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                if (converter.CanConvertFrom(typeof(string)))
                    return converter.ConvertFrom(null, CultureInfo.InvariantCulture, propValue);
                return Convert.ChangeType(propValue, propInfo.PropertyType);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}