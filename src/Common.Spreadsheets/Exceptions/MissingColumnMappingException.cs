namespace Common.Spreadsheets.Exceptions
{
    using System;

    [Serializable]
    public class MissingColumnMappingException : Exception
    {
        public MissingColumnMappingException()
        {
        }

        public MissingColumnMappingException(string message)
            : base(message)
        {
        }

        public MissingColumnMappingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MissingColumnMappingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public MissingColumnMappingException(string spreadsheetName, string[] propertiesWithoutColumnMappings)
            : base(
                string.Format(
                    "Properties '{0}' of the '{1}' spreadsheet have no Column mapping", 
                    string.Join("; ", propertiesWithoutColumnMappings), 
                    spreadsheetName))
        {
            this.SpreadsheetName = spreadsheetName;
            this.PropertiesWithoutColumnMappings = propertiesWithoutColumnMappings;
        }

        public string SpreadsheetName { get; private set; }

        public string[] PropertiesWithoutColumnMappings { get; private set; }
    }
}