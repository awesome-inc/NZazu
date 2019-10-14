using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace NZazu.Fields
{
    internal static class PropertyExtensions
    {
        public static bool CanSetProperty<T>(this T instance, string propertyName)
        {
            object obj = instance;
            var type = instance.GetType();
            var propName = propertyName;

            var p = propName.IndexOf('.');
            while (p != -1) // handle child-property
            {
                var parent = propName.Substring(0, p);
                // test if a corresponding property exists for the given type.
                var propInfo = type.GetProperty(parent, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null)
                    throw new ArgumentException($"Property '{propName}' does not exist.");

                propName = propName.Substring(p + 1);
                obj = propInfo.GetValue(obj, null);
                type = obj.GetType();
                p = propName.IndexOf('.');
            }

            var propertyInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            return propertyInfo != null && HasPublicGetterAndSetter(propertyInfo);
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
                        throw new ArgumentException($"Property '{propertyName}' does not exist.");

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
                        throw new ArgumentException($"Property '{propName}' does not exist.");

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
                return converter.CanConvertFrom(typeof(string))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ? converter.ConvertFrom(null, CultureInfo.InvariantCulture, propValue)
                    : Convert.ChangeType(propValue, propInfo.PropertyType);
            }
            catch (Exception ex)
            {
                var message =
                    $"Could not convert the value \"{propValue}\" for property type \"{propInfo?.PropertyType.Name}\" of prop \"{propInfo?.Name}\"";
                throw new ArgumentException(message, ex);
            }
        }
    }
}