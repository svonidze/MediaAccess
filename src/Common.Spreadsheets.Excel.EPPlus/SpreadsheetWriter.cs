namespace Common.Spreadsheets.Excel.EPPlus
{
    using Common.Collections;
    using Common.Spreadsheets.Enum;
    using Common.Spreadsheets.Exceptions;
    using Common.Spreadsheets.Mappings;

    using OfficeOpenXml;

    public class SpreadsheetWriter : ISpreadsheetWriter
    {
        private static ExcelPackage PackIntoExcelPackage<T>(
            Stream templateStream,
            IEnumerable<T> items,
            ISpreadsheetMapping spreadsheetMapping)
        {
            var package = new ExcelPackage(templateStream);
            var sheet = package.Workbook.Worksheets[spreadsheetMapping.SpreadsheetName];

            if (sheet == null)
            {
                throw new Exception(
                    $"Sheet with name '{spreadsheetMapping.SpreadsheetName}' is not found in the input bulk");
            }

            if (spreadsheetMapping.MappingMode == MappingMode.MappingByFirsRowKeys)
            {
                var lastColumnNumber = sheet.Cells.Max(c => c.End.Column);
                var firstRowCells = sheet.Cells[1, (int)SpreadsheetColumns.A + 1, 1, lastColumnNumber];
                AdjustColumnMappingAsPerKeyColumn(firstRowCells, spreadsheetMapping);
            }

            Write(
                items,
                sheet,
                spreadsheetMapping.ColumnMappings,
                startRowNumber: spreadsheetMapping.FirstRowsToSkip + 1);
            return package;
        }

        private static MemoryStream PackIntoExcelPackageStream<T>(
            Stream templateStream,
            IEnumerable<T> items,
            ISpreadsheetMapping spreadsheetMapping)
        {
            using var package = PackIntoExcelPackage(templateStream, items, spreadsheetMapping);
            var memoryStream = new MemoryStream();
            package.SaveAs(memoryStream);
            return memoryStream;
        }

        public byte[] Write<T>(IEnumerable<T> items, ISpreadsheetMapping spreadsheetMapping, Stream templateStream)
        {
            using var memoryStream = PackIntoExcelPackageStream(templateStream, items, spreadsheetMapping);
            return memoryStream.ToArray();
        }

        public void SaveFile<T>(
            IEnumerable<T> items,
            ISpreadsheetMapping spreadsheetMapping,
            Stream templateStream,
            FileInfo destinationFile)
        {
            using var excelPackage = PackIntoExcelPackage(templateStream, items, spreadsheetMapping);
            excelPackage.SaveAs(destinationFile);
        }

        private static void AdjustColumnMappingAsPerKeyColumn(
            IEnumerable<ExcelRangeBase> cells, 
            ISpreadsheetMapping spreadsheetMapping)
        {
            foreach (var cell in cells)
            {
                var cellValue = cell.Value.ToString();

                var columnMapping = spreadsheetMapping.ColumnMappings
                    .Where(c => c.Key == cellValue)
                    .AllowVerboseException()
                    .SingleOrDefault();

                if (columnMapping == default)
                {
                    continue;
                }

                columnMapping.Column = (SpreadsheetColumns)cell.Start.Column - 1;
            }
        }

        private static void Write<T>(
            IEnumerable<T> items,
            ExcelWorksheet sheet,
            List<IColumnMapping> columnMappings,
            int startRowNumber)
        {
            var propertiesWithoutColumnMappings =
                columnMappings.Where(m => !m.Column.HasValue).Select(m => m.PropertyName).ToArray();

            if (propertiesWithoutColumnMappings.Any())
            {
                throw new MissingColumnMappingException(typeof(T).FullName, propertiesWithoutColumnMappings);
            }

            foreach (var item in items)
            {
                foreach (var columnMapping in columnMappings)
                {
                    if (!columnMapping.Column.HasValue)
                    {
                        throw new Exception("Unreachable code. The previous handler should ensure that all properties have column mappings.");
                    }

                    sheet.Cells[startRowNumber, (int)columnMapping.Column + 1].Value = columnMapping.GetValue(item);
                }

                startRowNumber++;
            }
        }
    }
}