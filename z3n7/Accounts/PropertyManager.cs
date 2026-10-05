using System;
using System.Collections.Generic;
using System.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Utilities
{
    /// <summary>Copies simple properties of objects to and from database rows (by reflection).</summary>
    public static class PropertyManager
    {
        #region get
        /// <summary>
        /// Names of public readable properties of simple types: primitives, <c>string</c>, <c>decimal</c>,
        /// <c>DateTime</c>, enums.
        /// </summary>
        /// <param name="type">Type to inspect.</param>
        /// <param name="requireSetter">Only properties that also have a public setter.</param>
        public static List<string> GetTypeProperties(Type type, bool requireSetter = false)
        {
            var listColumnsToAdd = new List<string>();

            foreach (var prop in type.GetProperties())
            {
                bool hasGet = prop.CanRead && prop.GetMethod?.IsPublic == true;
                bool hasSet = prop.CanWrite && prop.SetMethod?.IsPublic == true;
        
                bool isAccessible = requireSetter ? (hasGet && hasSet) : hasGet;
                if (!isAccessible) continue;

                var propType = prop.PropertyType;
                bool isSimple = propType.IsPrimitive || 
                                propType == typeof(string) || 
                                propType == typeof(decimal) || 
                                propType == typeof(DateTime) ||
                                propType.IsEnum;

                if (isSimple) listColumnsToAdd.Add(prop.Name);
            }
            return listColumnsToAdd;
        }
        /// <summary>Same as <c>GetTypeProperties(obj.GetType())</c>.</summary>
        public static List<string> GetTypeProperties(object obj)
        {
            return GetTypeProperties(obj.GetType());
        }
        /// <summary>
        /// Reads property values as text, with single quotes doubled. Properties that fail to read are skipped.
        /// </summary>
        /// <param name="obj">Source object.</param>
        /// <param name="propertyList">Properties to read; default <c>GetTypeProperties</c>.</param>
        /// <param name="tableToUpd">
        /// When set, also writes the values to the current account's row of this table (<c>DicToDb</c>).
        /// </param>
        /// <returns>Property → value.</returns>
        public static Dictionary<string, string> GetValuesByProperty(this IZennoPosterProjectModel project, object obj, List<string> propertyList = null, string tableToUpd = null)
        {
            var type = obj.GetType();
    
            if (propertyList == null || propertyList.Count == 0) 
                propertyList = GetTypeProperties(type);
            
            var data = new Dictionary<string, string>();
    
            foreach (var column in propertyList)
            {
                try
                {
                    var prop = type.GetProperty(column);
                    var value = prop.GetValue(obj, null); 
                    string valueStr = value != null ? value.ToString() : string.Empty;
                    data.Add(column, valueStr);
                }
                catch
                {
                    //project.SendWarningToLog($"Error on field '{column}': {ex.Message}");
                }
            }
    
            // DicToDb escapes quotes itself and renames an "id" key, so it gets its own raw copy.
            if (!string.IsNullOrEmpty(tableToUpd)) project.DicToDb(new Dictionary<string, string>(data), tableToUpd);
            return data.ToDictionary(kv => kv.Key, kv => kv.Value.Replace("'", "''"));
        }
        #endregion
        
        #region set
        
        /// <summary>
        /// Sets the object's writable properties from a database row, converting text to the property type.
        /// Empty values and failed conversions are skipped; other errors are logged as warnings.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <param name="table">Table.</param>
        /// <param name="propertyList">Properties to set; default <c>GetTypeProperties</c>.</param>
        /// <param name="key">Column matched against <c>id</c>.</param>
        /// <param name="id">Row; default is the current account (<c>acc0</c>).</param>
        /// <param name="where">Raw SQL condition instead of <c>key</c>/<c>id</c>.</param>
        public static void SetValuesFromDb(this IZennoPosterProjectModel project, object obj, string table = "profile", List<string> propertyList = null, string key = "id", object id = null, string where = "")
        {
            var type = obj.GetType();

            if (propertyList == null)
                propertyList = GetTypeProperties(type);

            string columnsToGet = string.Join(", ", propertyList);
            var dbData = project.DbGetColumns(columnsToGet, table, key: key, id: id, where: where);

            foreach (var column in propertyList)
            {
                try
                {
                    var prop = type.GetProperty(column);
                    
                    if (prop == null || !prop.CanWrite || prop.SetMethod?.IsPublic != true)
                        continue;

                    string valueStr = dbData.ContainsKey(column) ? dbData[column] : null;

                    if (string.IsNullOrEmpty(valueStr))
                        continue;

                    object value = ConvertToPropertyType(valueStr, prop.PropertyType);

                    if (value != null)
                        prop.SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    project.SendWarningToLog($"Error setting field '{column}': {ex.Message}");
                }
            }
        }
        private static object ConvertToPropertyType(string value, Type targetType)
{
    try
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(string))
            return value;

        if (underlyingType == typeof(int))
            return int.Parse(value);

        if (underlyingType == typeof(long))
            return long.Parse(value);

        if (underlyingType == typeof(bool))
            return bool.Parse(value);

        if (underlyingType == typeof(decimal))
            return decimal.Parse(value);

        if (underlyingType == typeof(double))
            return double.Parse(value);

        if (underlyingType == typeof(DateTime))
            return DateTime.Parse(value);

        if (underlyingType.IsEnum)
            return Enum.Parse(underlyingType, value);

        return Convert.ChangeType(value, underlyingType);
    }
    catch
    {
        return null;
    }
}
        
        #endregion
        
    }
}