namespace Common.Spreadsheets
{
    using System.Collections.Generic;

    using Common.Spreadsheets.Model;

    public interface ISpreadsheetReader: IDisposable
    {
        IEnumerable<Sheet> Read();

        IEnumerable<T> Read<T>();
    }
}
