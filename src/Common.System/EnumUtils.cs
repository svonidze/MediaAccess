namespace Common.System
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Xml.Serialization;

    public static class EnumUtils
    {
        public static bool TryParseToEnum<T>(this string value, out T result)
            where T : struct, IConvertible
        {
            return Enum.TryParse(value, true, out result);
        }

        public static T StringToEnum<T>(this string value)
            where T : struct, IConvertible
        {
            if (TryParseToEnum(value, out T result))
            {
                return result;
            }

            var query = from T enumValue in Enum.GetValues(typeof(T))
                let stringEnumValue = EnumToString(enumValue)
                where stringEnumValue.Equals(value, StringComparison.InvariantCultureIgnoreCase)
                select enumValue;
            
            foreach (var enumValue in query) return enumValue;

            throw new ArgumentException($"String value '{value}' is not element of enumeration '{nameof(T)}'");
        }

        public static T XmlEnumStringToEnum<T>(this string value)
            where T : struct
        {
            var enumValues = EnumToList<T>();
            var type = typeof(T);
            foreach (var enumValue in enumValues)
            {
                var info = type.GetField(enumValue.ToString());
                if (!info.IsDefined(typeof(XmlEnumAttribute), false))
                {
                    throw new ArgumentException(
                        $"XmlEnum Attribute is not defined for value '{enumValue}' in enumeration '{type.Name}'");
                }

                var attributes = info.GetCustomAttributes(typeof(XmlEnumAttribute), false);
                var attr = (XmlEnumAttribute)attributes.First();
                if (attr.Name == value)
                {
                    return enumValue;
                }
            }

            throw new ArgumentException($"XmlEnum value '{value}' is not found in enumeration '{type.Name}'");
        }

        public static string EnumToString<T>(T @enum)
            where T : struct, IConvertible
        {
            var type = @enum.GetType();
            var info = type.GetField(@enum.ToString());

            if (info.IsDefined(typeof(XmlEnumAttribute), false))
            {
                var attributes = info.GetCustomAttributes(typeof(XmlEnumAttribute), false);
                var attr = (XmlEnumAttribute)attributes.First();
                return attr.Name;
            }

            return @enum.ToString();
        }

        public static IEnumerable<T> EnumToList<T>()
            where T : struct
        {
            var type = typeof(T);

            if (type.BaseType != typeof(Enum))
            {
                throw new ArgumentException($"{type.FullName} is not {typeof(Enum).FullName}");
            }

            return Enum.GetValues(type).Cast<T>();
        }

        public static TTo ConvertEnum<TFrom, TTo>(this TFrom from)
            where TTo : struct
            where TFrom : struct
        {
            try
            {
                return (TTo)Enum.Parse(typeof(TTo), from.ToString());
            }
            catch (Exception)
            {
                throw new ArgumentException(
                    $"Cannot convert incompatible enums: from {@from.GetType()} to {typeof(TTo)}");
            }
        }

        public static TTo? ConvertEnum<TFrom, TTo>(TFrom? from)
            where TTo : struct
            where TFrom : struct
        {
            if (from.HasValue)
            {
                return ConvertEnum<TFrom, TTo>(from.Value);
            }

            return null;
        }

        public static List<T> StringToListOfEnums<T>(string separatedString, string[] separators)
            where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(separatedString))
            {
                return new List<T>();
            }

            var mappings = GetFromStringMappings<T>();

            var nonExistingItems = separatedString.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !mappings.ContainsKey(x)).ToList();

            if (nonExistingItems.Any())
            {
                throw new KeyNotFoundException($"'{string.Join(", ", nonExistingItems)}' not found in {nameof(T)}");
            }

            return separatedString.Split(separators, StringSplitOptions.RemoveEmptyEntries).Where(mappings.ContainsKey)
                .Select(x => mappings[x]).ToList();
        }

        public static string ListOfEnumsToString<T>(List<T> collection, string separator)
            where T : struct, IConvertible
        {
            if (!collection.Any())
            {
                return null;
            }

            var mappings = GetToStringMappings<T>();

            return string.Join(separator, collection.Select(x => mappings[x]).ToArray());
        }

        public static T ListOfEnumsToMask<T>(List<T> listOfEnums)
            where T : struct
        {
            var result = (int)(object)default(T);

            listOfEnums.ForEach(v => result |= (int)(object)v);
            return (T)(object)result;
        }

        public static List<T> MaskToListOfEnums<T>(T mask)
            where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Where(v => ((int)(object)mask & (int)(object)v) == (int)(object)v).Except(
                    new List<T>
                        {
                            default(T)
                        }).ToList();
        }

        private static Dictionary<string, T> GetFromStringMappings<T>()
            where T : struct, IConvertible
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(EnumToString, i => i);
        }

        private static Dictionary<T, string> GetToStringMappings<T>()
            where T : struct, IConvertible
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(i => i, EnumToString);
        }
    }
}