namespace Common.Spreadsheets
{
    using System.Collections.Generic;
    using System.IO;

    using Common.Spreadsheets.Mappings;

    public interface ISpreadsheetWriter
    {
        byte[] Write<T>(IEnumerable<T> items, ISpreadsheetMapping spreadsheetMapping, Stream templateStream);

        void SaveFile<T>(
            IEnumerable<T> items,
            ISpreadsheetMapping spreadsheetMapping,
            Stream templateStream,
            FileInfo destinationFile);
    }
}