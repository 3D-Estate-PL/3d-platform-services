namespace BuildingBlocks.Abstractions.Excel;

public record SheetParameters
{
    public int RangeColumnStart { get; set; }
    public int RangeColumnEnd { get; set; }
    public int RangeRowStart { get; set; }
    public int RangeRowEnd { get; set; }

    public int ColumnsCount => RangeColumnEnd - RangeColumnStart + 1;
    
    public required string SpreadSheetId { get; init; }
}