using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace BuildingBlocks.Infrastructure.GoogleSheet;



public class GoogleDictionariesProvider : IExcelDictionaryProvider
{
    private readonly GoogleSheetsClient _googleSheetsClient;

    public GoogleDictionariesProvider(GoogleSheetsClient googleSheetsClient)
    {
        _googleSheetsClient = googleSheetsClient;
    }

    private string GetColumnName(int index)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var value = "";

        if (index >= letters.Length)
            value += letters[index / letters.Length - 1];

        value += letters[index % letters.Length - 1];
        return value;
    }
    
    public async Task<List<ExpandoObject>> Import(string dictionaryName,
        SheetParameters sheetParameters)
    {
        var client = _googleSheetsClient.Get();
        var range = $"{dictionaryName}!{GetColumnName(sheetParameters.RangeColumnStart)}{sheetParameters.RangeRowStart}:{GetColumnName(sheetParameters.RangeColumnEnd)}{sheetParameters.RangeRowEnd}";
        
        
        var request = client.Spreadsheets.Values.Get(sheetParameters.SpreadSheetId, range);
        var response = await request.ExecuteAsync();

        var columnNames = new List<string>();
        var returnValues = new List<ExpandoObject>();

        var rowCounter = 0;
        foreach (var row in response.Values)
        {
            if (rowCounter == 0)
            {
                for (var i = 0; i < row.Count; i++)
                {
                    columnNames.Add(row[i].ToString());
                }

                rowCounter++;
                continue;
            }
            
            var expando = new ExpandoObject();
            var expandoDict = expando as IDictionary<String, object>;
            var columnCounter = 0;
            foreach (var columnName in columnNames)
            {
                if (columnCounter < row.Count)
                {
                    expandoDict.Add(columnName, row[columnCounter].ToString());
                }
                
                columnCounter++;
            }

            if (expando.Any())
            {
                returnValues.Add(expando);
            }

        }
        
        return returnValues;
    }

    public async Task ExportAsync(
        string spreadSheetId,
        List<object> headerNames,
        List<IList<object>> rows)
    {
        var sheetService = _googleSheetsClient.Get();
        await UpdateHeaders(spreadSheetId, headerNames);
        
        var batchUpdateValues = new List<ValueRange>();
        var index = 2;

        foreach (var rowColumns in rows)
        {
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> {rowColumns},
                Range = GetRange(index)
            };
            batchUpdateValues.Add(valueRange);
            index++;
        }

        var batchUpdate = new BatchUpdateValuesRequest {Data = batchUpdateValues};
        batchUpdate.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW.ToString();
        var update =
            sheetService.Spreadsheets.Values.BatchUpdate(batchUpdate, spreadSheetId);
        var response = await update.ExecuteAsync();
    }
    
    private async Task UpdateHeaders(string sheetId, List<object> propertyNames)
    {
        var sheetService = _googleSheetsClient.Get();

        var valueHeaderRange = new ValueRange {Values = new List<IList<object>> {propertyNames}};
        var updateHeader = sheetService.Spreadsheets.Values.Update(valueHeaderRange,
            sheetId, GetRange(1));
        updateHeader.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

        await updateHeader.ExecuteAsync();
    }
    
    
    private static string GetRange(int rowIndex)
    {
        return $"A{rowIndex}:S{rowIndex}";
    }
}