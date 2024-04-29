using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SharpDevLib.Extensions.Excel.Encryption;
using SharpDevLib.Extensions.Excel.Styles;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpDevLib.Extensions.Excel;

internal class ExcelService : IExcelService
{
    #region Encrypt/Decrypt
    public void Encrypt(string filePath, string password, string decryptedFilePath)
    {
        using var encryptedStream = Encrypt(filePath, password);
        encryptedStream.ToArray().SaveFile(decryptedFilePath);
    }

    private MemoryStream Encrypt(string filePath, string password)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        return Encrypt(fileStream, password);
    }

    public MemoryStream Encrypt(Stream stream, string password)
    {
        using var inputStream = new MemoryStream(); ;
        stream.CopyTo(inputStream);

        using var encryptedStream = new EncryptedPackageHandler().EncryptPackage(inputStream.ToArray(), new ExcelEncryption { Password = password, Version = EncryptionVersion.Standard, IsEncrypted = true });
        return encryptedStream ?? throw new Exception($"encrypt stream is empty");
    }

    public void Decrypt(string filePath, string password, string encryptedFilePath)
    {
        using var stream = Decrypt(filePath, password);
        stream.ToArray().SaveFile(encryptedFilePath);
    }

    private MemoryStream Decrypt(string filePath, string password)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        return Decrypt(fileStream, password);
    }

    public MemoryStream Decrypt(Stream stream, string password)
    {
        using var inputStream = new MemoryStream(); ;
        stream.CopyTo(inputStream);
        using var originStream = new EncryptedPackageHandler().DecryptPackage(inputStream, new ExcelEncryption { Password = password, IsEncrypted = true });
        var zipPackage = new ZipPackage(originStream);
        var outStream = new MemoryStream();
        zipPackage.Save(outStream);
        return outStream;
    }
    #endregion

    #region Read
    public DataTable[] Read(string filePath, params ExcelReadOptions[] options)
    {
        using var document = SpreadsheetDocument.Open(filePath, false);
        return Read(document, options).ToArray();
    }

    public DataTable[] Read(string filePath, string password, params ExcelReadOptions[] options)
    {
        if (password.IsNull()) return Read(filePath, options);
        using var stream = Decrypt(filePath, password);
        return Read(stream, options).ToArray();
    }

    public DataTable[] Read(Stream stream, params ExcelReadOptions[] options)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var document = SpreadsheetDocument.Open(stream, false);
        return Read(document, options).ToArray();
    }

    public DataTable[] Read(Stream stream, string password, params ExcelReadOptions[] options)
    {
        if (password.IsNull()) return Read(stream, options);
        using var decryptedStream = Decrypt(stream, password);
        return Read(decryptedStream, options).ToArray();
    }

    private static IEnumerable<DataTable> Read(SpreadsheetDocument document, params ExcelReadOptions[] options)
    {
        var workbookPart = document.WorkbookPart ?? throw new Exception($"load workbook failed");
        var worksheetParts = workbookPart.WorksheetParts.ToList();
        var sharedStringItems = workbookPart.GetPartsOfType<SharedStringTablePart>()?.FirstOrDefault()?.SharedStringTable?.Elements<SharedStringItem>()?.ToList() ?? new List<SharedStringItem>();
        for (int i = 0; i < options.Length; i++)
        {
            var option = options[i];
            var sheetIndex = (int)(option.SheetIndex ?? (uint)i);
            if (sheetIndex < 0 || sheetIndex >= worksheetParts.Count) throw new Exception($"sheet index({sheetIndex}) out of range(0-{worksheetParts.Count})");
            var workSheetPart = worksheetParts.ElementAt(sheetIndex);
            var sheetData = workSheetPart.Worksheet.GetFirstChild<SheetData>() ?? throw new Exception("load sheet data failed");
            var table = new DataTable();
            var rows = sheetData.Elements<Row>().ToList();
            if (rows.Count <= 0) yield return table;
            var columnReferences = SetTableHead(rows, table, sharedStringItems);
            SetTableData(columnReferences, rows.Skip(1).ToList(), table, sharedStringItems);
            yield return table;
        }
    }

    private static List<string> SetTableHead(List<Row> rows, DataTable table, List<SharedStringItem> sharedStringItems)
    {
        table.Columns.Add(new DataColumn("ExcelId"));
        var headRow = rows.First();
        var columnReferences = new List<string>();
        headRow.Elements<Cell>().ToList().ForEach(x =>
        {
            table.Columns.Add(new DataColumn(GetCellValue(x, sharedStringItems)));
            columnReferences.Add(AnalysisCellReference(x.CellReference?.ToString() ?? string.Empty).Item1);
        });
        return columnReferences;
    }

    private static void SetTableData(List<string> columnReferences, List<Row> rows, DataTable table, List<SharedStringItem> sharedStringItems)
    {
        rows.ForEach(row =>
        {
            var tableRow = table.NewRow();
            tableRow["ExcelId"] = Guid.NewGuid();
            var cells = row.Elements<Cell>().Select(x => new { ColumnReference = columnReferences.FirstOrDefault(y => y == AnalysisCellReference(x.CellReference?.ToString() ?? string.Empty).Item1), Cell = x }).ToList();
            for (int i = 0; i < columnReferences.Count; i++)
            {
                var cell = cells.FirstOrDefault(x => x.ColumnReference == columnReferences[i]);
                var columnName = table.Columns[i + 1].ColumnName;
                if (cell is null) tableRow[columnName] = null;
                else tableRow[columnName] = GetCellValue(cell.Cell, sharedStringItems);
            }
            table.Rows.Add(tableRow);
        });
    }

    private static string GetCellValue(Cell cell, List<SharedStringItem> sharedStringItems)
    {
        var text = cell.CellValue!.Text!;
        var dataType = cell.DataType;
        if (dataType is null) return text;
        else if (dataType == CellValues.SharedString)
        {
            var sharedItem = sharedStringItems.ElementAt(int.Parse(text));
            return sharedItem.Text?.Text ?? string.Empty;
        }
        else if (dataType == CellValues.String || dataType == CellValues.Number)
        {
            return text;
        }
        else
        {
            throw new Exception($"read cell with format:{dataType} not surppoted");
        }
    }
    #endregion

    #region Write
    public byte[] Write(params ExcelWriteOptions[] options)
    {
        using var stream = new MemoryStream();
        using var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        Write(document, options);
        document.Dispose();//required
        return stream.ToArray();
    }

    public byte[] Write(string password, params ExcelWriteOptions[] options)
    {
        if (password.IsNull()) return Write(options);
        var bytes = Write(options);
        using var inputStream = new MemoryStream(bytes);
        using var encryptedStream = Encrypt(inputStream, password);
        return encryptedStream.ToArray();
    }

    private static void Write(SpreadsheetDocument document, params ExcelWriteOptions[] options)
    {
        var workbookPart = CreateWorkbook(document);
        var styleFactory = new CellStyleFactory();
        for (uint i = 0; i < options.Length; i++)
        {
            var option = options[i];
            EnsureTableHasExcelIdColumn(option.Table);
            InitialStyles(workbookPart, option, styleFactory);

            var worksheetPart = CreateWorksheet(workbookPart, option, i + 1);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;
            SetTitleRemark(worksheetPart, option);
            SetHeaders(sheetData, option);
            SetData(sheetData, option);
            var cells = GetAllCells(worksheetPart, option);
            SetComments(worksheetPart, option, cells, styleFactory);
            SetStyles(worksheetPart, option, cells, styleFactory);
            RemoveUnusedColumns(cells);
        }
    }

    private static void EnsureTableHasExcelIdColumn(DataTable table)
    {
        if (table.Columns.Contains("ExcelId")) return;
        table.Columns.Add(new DataColumn("ExcelId", typeof(Guid)));
        foreach (DataRow row in table.Rows)
        {
            row["ExcelId"] = Guid.NewGuid();
        }
    }

    private static WorkbookPart CreateWorkbook(SpreadsheetDocument document)
    {
        var workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();
        return workbookPart;
    }

    private static WorksheetPart CreateWorksheet(WorkbookPart workbookPart, ExcelWriteOptions option, uint sheetIndex)
    {
        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet();

        var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
        if (sheets.IsNull())
        {
            sheets = new Sheets();
            workbookPart.Workbook.AppendChild(sheets);
        }
        sheets!.AppendChild(new Sheet
        {
            Id = workbookPart.GetIdOfPart(worksheetPart),
            SheetId = sheetIndex,
            Name = option.SheetName ?? $"sheet{sheetIndex}"
        });
        worksheetPart.Worksheet.Append(new SheetData());
        return worksheetPart;
    }

    private static void SetHeaders(SheetData sheetData, ExcelWriteOptions option)
    {
        var row = new Row() { RowIndex = FileRemarkRowSpan(option) + 1 };
        for (int i = 0; i < option.Table.Columns.Count; i++)
        {
            row.Append(new Cell()
            {
                CellReference = GetCellReference((int)row.RowIndex.Value, i),
                DataType = CellValues.String,
                CellValue = new CellValue(option.Table.Columns[i].ColumnName),
            });
        }
        sheetData.Append(row);
    }

    private static void SetData(SheetData sheetData, ExcelWriteOptions option)
    {
        for (int i = 0; i < option.Table.Rows.Count; i++)
        {
            var row = new Row() { RowIndex = (uint)(i + FileRemarkRowSpan(option) + 2) };
            for (int j = 0; j < option.Table.Columns.Count; j++)
            {
                var dataType = option.Table.Columns[j].DataType;
                var cell = option.Table.Rows[i][j];
                row.Append(new Cell()
                {
                    CellReference = GetCellReference((int)row.RowIndex.Value, j),
                    DataType = dataType.IsNumeric() ? CellValues.Number : CellValues.String,
                    CellValue = new CellValue(cell?.ToString() ?? string.Empty),
                });
            }
            sheetData.Append(row);
        }
    }

    private static uint FileRemarkRowSpan(ExcelWriteOptions options) => options.FileRemark.Content.IsEmpty() ? 0 : (options.FileRemark?.RowSpan ?? 1);

    private static void SetComments(WorksheetPart worksheetPart, ExcelWriteOptions option, List<CellModel> cells, CellStyleFactory styleFactory)
    {
        if (option.Remarks.IsEmpty()) return;
        var workbookPart = worksheetPart.GetParentParts().First() as WorkbookPart;
        var worksheetCommentsPart = worksheetPart.WorksheetCommentsPart;
        var commentList = worksheetCommentsPart?.Comments?.CommentList;
        if (worksheetCommentsPart.IsNull())
        {
            worksheetCommentsPart = worksheetPart.AddNewPart<WorksheetCommentsPart>();
            worksheetCommentsPart.Comments = new Comments();
            var authors = new Authors(option.Remarks.Select(x => x.Author).Distinct().Select(x => new Author(x)));
            worksheetCommentsPart.Comments.Append(authors);
            commentList = new CommentList();
            worksheetCommentsPart.Comments.Append(commentList);
        }

        var shapeTemplate = "<v:shape type=\"#_x0000_t202\" style='position:absolute;margin-left:78pt;margin-top:1.2pt;width:100.8pt;height:56.4pt;z-index:1;visibility:hidden' fillcolor=\"infoBackground [80]\" strokecolor=\"none [81]\" o:insetmode=\"auto\"><v:fill color2=\"infoBackground [80]\"/><v:shadow color=\"none [81]\" obscured=\"t\"/><v:path o:connecttype=\"none\"/><v:textbox style='mso-direction-alt:auto'></v:textbox><x:ClientData ObjectType=\"Note\"><x:MoveWithCells/><x:SizeWithCells/><x:Anchor>1, 12, 0, 1, 3, 18, 4, 3</x:Anchor><x:AutoFill>False</x:AutoFill><x:Row>{0}</x:Row><x:Column>{1}</x:Column></x:ClientData></v:shape>";
        var shapes = string.Empty;
        option.Remarks.GroupBy(x => new { x.ExcelId, x.ColumnName }).Select(x => new ExcelRemark(x.Key.ExcelId, x.Key.ColumnName, string.Join(";", x.Select(y => y.Remark)), x.First().Author)).ToList().ForEach(x =>
        {
            var cell = cells.FirstOrDefault(y => y.ExcelId == x.ExcelId && y.ColumnName == x.ColumnName);
            if (cell.IsNull()) return;
            var styleIndex = (uint)styleFactory.Create<WarningStyle>(workbookPart!);
            cell!.Cell.StyleIndex = styleIndex;

            var authors = worksheetCommentsPart!.Comments.Authors?.Elements<Author>().ToList() ?? new List<Author>();
            var author = authors.FirstOrDefault(y => y.InnerText == x.Author);
            var authorId = author.IsNull() ? 0 : authors.IndexOf(author!);
            commentList!.Append(new Comment(new CommentText(new Run(new Text(x.Remark)))) { Reference = cell.CellReference, AuthorId = (uint)authorId });
            shapes += string.Format(shapeTemplate, cell.RowIndex - 1, cell.ColumnIndex);
        });

        var vmlDrawingPart = worksheetPart.AddNewPart<VmlDrawingPart>();
        var vmlStyleText = $"<xml xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"><o:shapelayout v:ext=\"edit\"><o:idmap v:ext=\"edit\" data=\"1\"/></o:shapelayout><v:shapetype id=\"_x0000_t202\" coordsize=\"21600,21600\" o:spt=\"202\" path=\"m,l,21600r21600,l21600,xe\"><v:stroke joinstyle=\"miter\"/><v:path gradientshapeok=\"t\" o:connecttype=\"rect\"/></v:shapetype>{shapes}</xml>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(vmlStyleText));
        vmlDrawingPart.FeedData(stream);
        worksheetPart.Worksheet.Append(new LegacyDrawing() { Id = worksheetPart.GetIdOfPart(vmlDrawingPart) });
    }

    private static void RemoveUnusedColumns(List<CellModel> cells)
    {
        cells.Where(x => x.ColumnName == "ExcelId").ToList().ForEach(x => x.Cell.Remove());
    }

    private static void InitialStyles(WorkbookPart workbookPart, ExcelWriteOptions option, CellStyleFactory styleFactory)
    {
        if (option.ExcelStyles.ClearStyle) return;
        styleFactory.Create<DefaultStyle>(workbookPart!);
        styleFactory.Create<WarningStyle>(workbookPart);
        styleFactory.Create<ErrorStyle>(workbookPart);
        styleFactory.Create<HeadStyle>(workbookPart);
        styleFactory.Create<FileRemarkStyle>(workbookPart);
        if (option.ExcelStyles.Head.NotNull()) styleFactory.Create(workbookPart, option.ExcelStyles.Head!);
        option.ExcelStyles.Rows?.Select(x => x.StyleType).Distinct().ToList().ForEach(x => styleFactory.Create(workbookPart, x));
        option.ExcelStyles.Cols?.Select(x => x.StyleType).Distinct().ToList().ForEach(x => styleFactory.Create(workbookPart, x));
        option.ExcelStyles.Cells?.Select(x => x.StyleType).Distinct().ToList().ForEach(x => styleFactory.Create(workbookPart, x));
    }

    private static void SetStyles(WorksheetPart worksheetPart, ExcelWriteOptions option, List<CellModel> cells, CellStyleFactory styleFactory)
    {
        if (option.ExcelStyles.ClearStyle) return;
        AutoColumnWidth(worksheetPart, option, cells);
        var workbookPart = worksheetPart.GetParentParts().First() as WorkbookPart;

        if (option.ExcelStyles.Head.NotNull())
        {
            var styleIndex = (uint)styleFactory.Create(workbookPart!, option.ExcelStyles.Head!);
            cells.Where(y => y.IsHead).ToList().ForEach(y => y.Cell.StyleIndex = styleIndex);
        }
        option.ExcelStyles.Rows?.ForEach(x =>
        {
            var styleIndex = (uint)styleFactory.Create(workbookPart!, x.StyleType);
            cells.Where(y => y.ExcelId == x.ExcelId && !y.IsHead).ToList().ForEach(y => y.Cell.StyleIndex = styleIndex);
        });
        option.ExcelStyles.Cols?.ForEach(x =>
        {
            var styleIndex = (uint)styleFactory.Create(workbookPart!, x.StyleType);
            cells.Where(y => y.ColumnName == x.ColumnName && !y.IsHead).ToList().ForEach(y => y.Cell.StyleIndex = styleIndex);
        });
        option.ExcelStyles.Cells?.ForEach(x =>
        {
            var styleIndex = (uint)styleFactory.Create(workbookPart!, x.StyleType);
            cells.Where(y => y.ExcelId == x.ExcelId && y.ColumnName == x.ColumnName).ToList().ForEach(y => y.Cell.StyleIndex = styleIndex);
        });
        if (option.FileRemark.Content.NotNull()) worksheetPart.Worksheet.GetFirstChild<SheetData>()!.Elements<Row>().First().Elements<Cell>().First().StyleIndex = (uint)styleFactory.Create(workbookPart!, typeof(FileRemarkStyle));
    }

    private static void AutoColumnWidth(WorksheetPart worksheetPart, ExcelWriteOptions option, List<CellModel> cells)
    {
        if (!option.ExcelStyles.AutoCellWidth) return;
        var columns = new Columns();
        var columnCount = cells.Where(x => x.IsHead).Count();
        for (uint i = 0; i < columnCount - 1; i++)
        {
            var maxCharCount = (cells.Count <= 0 || !cells.Any(x => x.ColumnIndex == i)) ? 50 : cells.Where(x => x.ColumnIndex == i).Max(x => x.Cell.CellValue?.Text?.Length ?? 0);
            columns.AppendChild(new Column { Min = i + 1, Max = i + 1, Width = maxCharCount + 2, CustomWidth = true });
        }
        worksheetPart.Worksheet.InsertAt(columns, 0);
    }

    private static List<CellModel> GetAllCells(WorksheetPart worksheetPart, ExcelWriteOptions option)
    {
        var result = new List<CellModel>();
        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
        if (sheetData.IsNull()) return result;

        var rows = sheetData!.Elements<Row>();
        for (uint i = FileRemarkRowSpan(option); i < rows.Count(); i++)
        {
            var row = rows.ElementAt((int)i);
            var excelId = Guid.Empty;
            if (i > ((int)FileRemarkRowSpan(option)))
            {
                excelId = option.Table.Rows[(int)i - 1 - (int)FileRemarkRowSpan(option)]["ExcelId"].ToString()!.ToGuid();
            }

            var cells = row.Elements<Cell>();
            for (int j = 0; j < cells.Count(); j++)
            {
                var cell = cells.ElementAt(j);
                var columnName = option.Table.Columns[j].ColumnName;
                var reference = GetCellReference((int)(row.RowIndex!.Value), j);
                var model = new CellModel(excelId, row.RowIndex, (uint)j, columnName, reference, cell, excelId == Guid.Empty);
                result.Add(model);
            }
        }
        return result;
    }

    private static string GetCellReference(int rowIndex, int colIndex)
    {
        var prefixCount = colIndex / 26;
        var prefix = prefixCount > 0 ? ((char)(prefixCount + 65)).ToString() : "";
        var nameCount = colIndex % 26;
        var name = ((char)(nameCount + 65)).ToString();
        return $"{prefix}{name}{rowIndex}";
    }

    private static void SetTitleRemark(WorksheetPart worksheetPart, ExcelWriteOptions options)
    {
        if (options.FileRemark.IsNull() || options.FileRemark.Content.IsNull()) return;
        var colSpan = (uint)options.Table.Columns.Count - 1;
        if (options.FileRemark!.ColSpan.NotNull() && options.FileRemark.ColSpan > 0) colSpan = options.FileRemark!.ColSpan.Value;
        var rowSpan = options.FileRemark.RowSpan ?? 1;
        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

        for (uint i = 0; i < rowSpan; i++)
        {
            var row = new Row() { RowIndex = i + 1 };
            for (uint j = 0; j < colSpan; j++)
            {
                var cell = new Cell { CellReference = GetCellReference((int)row.RowIndex.Value, (int)j), DataType = CellValues.String, CellValue = new CellValue((i == 0 && j == 0) ? options.FileRemark.Content! : string.Empty) };
                row.Append(cell);
            }
            sheetData.Append(row);
        }

        var mergeCells = new MergeCells();
        worksheetPart.Worksheet.InsertAfter(mergeCells, sheetData);
        var mergeCell = new MergeCell() { Reference = new StringValue($"{GetCellReference(1, 0)}:{GetCellReference((int)rowSpan, colSpan < 0 ? 0 : (int)colSpan - 1)}") };
        mergeCells.Append(mergeCell);
    }

    //private void AddWatermark(WorksheetPart worksheetPart,string fileName= @"C:\Users\AdminUser\Pictures\1.jpg")
    //{
    //    var drawingPart = worksheetPart.AddNewPart<DrawingsPart>();
    //    var imagePart = drawingPart.AddImagePart(ImagePartType.Jpeg, worksheetPart.GetIdOfPart(drawingPart));
    //    using var fileStream = new FileStream(fileName, FileMode.Open);
    //    imagePart.FeedData(fileStream);
    //    var nvdp = new NonVisualDrawingProperties
    //    {
    //        Id = 1025,
    //        Name = "Picture 1",
    //        Description = "Chart"
    //    };
    //    var picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks
    //    {
    //        NoChangeAspect = true,
    //        NoChangeArrowheads = true
    //    };
    //    var nvpdp = new NonVisualPictureDrawingProperties
    //    {
    //        PictureLocks = picLocks
    //    };
    //    var nvpp = new NonVisualPictureProperties
    //    {
    //        NonVisualDrawingProperties = nvdp,
    //        NonVisualPictureDrawingProperties = nvpdp
    //    };

    //    var stretch = new DocumentFormat.OpenXml.Drawing.Stretch
    //    {
    //        FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle()
    //    };

    //    var blipFill = new BlipFill();
    //    var blip = new DocumentFormat.OpenXml.Drawing.Blip
    //    {
    //        Embed = drawingPart.GetIdOfPart(imagePart),
    //        CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print
    //    };
    //    blipFill.Blip = blip;
    //    blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
    //    blipFill.Append(stretch);

    //    var t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
    //    var offset = new DocumentFormat.OpenXml.Drawing.Offset
    //    {
    //        X = 0,
    //        Y = 0
    //    };
    //    t2d.Offset = offset;
    //    //http://en.wikipedia.org/wiki/English_Metric_Unit#DrawingML
    //    //http://stackoverflow.com/questions/1341930/pixel-to-centimeter
    //    //http://stackoverflow.com/questions/139655/how-to-convert-pixels-to-points-px-to-pt-in-net-c
    //    var extents = new DocumentFormat.OpenXml.Drawing.Extents();
    //    t2d.Extents = extents;
    //    var sp = new ShapeProperties
    //    {
    //        BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto,
    //        Transform2D = t2d
    //    };
    //    var prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry
    //    {
    //        Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle,
    //        AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList()
    //    };
    //    sp.Append(prstGeom);
    //    sp.Append(new DocumentFormat.OpenXml.Drawing.NoFill());

    //    DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
    //    picture.NonVisualPictureProperties = nvpp;
    //    picture.BlipFill = blipFill;
    //    picture.ShapeProperties = sp;

    //    DocumentFormat.OpenXml.Drawing.Spreadsheet.Position pos = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Position();
    //    pos.X = 0;
    //    pos.Y = 10;
    //    var ext = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Extent
    //    {
    //        Cx = extents.Cx,
    //        Cy = extents.Cy
    //    };
    //    var anchor = new DocumentFormat.OpenXml.Drawing.Spreadsheet.AbsoluteAnchor
    //    {
    //        Position = pos,
    //        Extent = ext
    //    };
    //    anchor.Append(picture);
    //    anchor.Append(new DocumentFormat.OpenXml.Drawing.Spreadsheet.ClientData());
    //    var wsd = new DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing();
    //    wsd.Append(anchor);
    //    Drawing drawing = new Drawing();
    //    drawing.Id = drawingPart.GetIdOfPart(imagePart);

    //    var fv = new FileVersion
    //    {
    //        ApplicationName = "Microsoft Office Excel"
    //    };

    //    //worksheetPart.Worksheet.Append(drawing);
    //    //(worksheetPart.GetParentParts().First() as WorkbookPart)!.Workbook.Append(fv);
    //}
    #endregion

    #region Common
    private const string _columnExpression = "[A-Za-z]+";
    private const string _rowExpression = "[0-9]+";
    private static Tuple<string, int> AnalysisCellReference(string cellReference)
    {
        var match = Regex.Match(cellReference, _columnExpression);
        if (!match.Success) throw new Exception($"{cellReference} is not a valid CellReference");
        var column = match.Value;
        var row = int.Parse(Regex.Match(cellReference, _rowExpression).Value);
        return new Tuple<string, int>(column, row);
    }
    #endregion
}