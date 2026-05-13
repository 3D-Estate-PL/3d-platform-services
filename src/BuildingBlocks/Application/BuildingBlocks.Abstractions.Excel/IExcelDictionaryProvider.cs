using System.Dynamic;

namespace BuildingBlocks.Abstractions.Excel;

public interface IExcelDictionaryProvider
{
    public Task<List<ExpandoObject>> Import(string dictionaryName,SheetParameters sheetParameters);

    public Task ExportAsync(
        string spreadSheetId,
        List<object> headerNames,
        List<IList<object>> rows);
}