using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PViMS.Services.FileHandler
{
    public class ImportFollowUpSheet : IEnumerable<ImportFollowUpSheet.ImportFollowUpSheetRow>, IDisposable
    {
        //private readonly ImportEnrollmentRow _currentRow;
        private SpreadsheetDocument _spreadsheetDocument;
        private Sheet _sheet;
        private Worksheet _worksheet;
        private SheetData _rows;

        public ImportFollowUpSheet(Stream file)
        {
            _spreadsheetDocument = SpreadsheetDocument.Open(file, false);
            _sheet = _spreadsheetDocument.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
            _worksheet = (_spreadsheetDocument.WorkbookPart.GetPartById(_sheet.Id.Value) as WorksheetPart).Worksheet;
            _rows = (SheetData)_worksheet.ChildElements.GetItem(4);
        }

        public void Dispose()
        {
            if (_spreadsheetDocument != null)
            {
                _spreadsheetDocument.Dispose();
                _spreadsheetDocument = null;
            }
        }

        public IEnumerator<ImportFollowUpSheetRow> GetEnumerator()
        {
            return new ImportFollowUpSheetRowEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static class Columns
        {
            public const int Column1 = 1;
            public const int Form = 1;
            public const int TreatmentSite = 2;
            public const int ASMCode = 3;
            public const int LastName = 4;
            public const int FirstName = 5;
            public const int ASMCode1 = 6;
            public const int DateOfBirth = 7;
            public const int Age = 8;
            public const int Sex = 9;
            public const int Weight = 10;
            public const int Height = 11;
            public const int MedName1 = 16;
            public const int MedIndication1 = 17;
            public const int MedDose1 = 18;
            public const int MedStart1 = 20;
            public const int MedEnd1 = 21;
            public const int MedName2 = 23;
            public const int MedIndication2 = 24;
            public const int MedDose2 = 25;
            public const int MedStart2 = 27;
            public const int MedEnd2 = 28;
            public const int MedName3 = 30;
            public const int MedIndication3 = 31;
            public const int MedDose3 = 32;
            public const int MedStart3 = 34;
            public const int MedEnd3 = 35;
            public const int MedName4 = 37;
            public const int MedIndication4 = 38;
            public const int MedDose4 = 39;
            public const int MedStart4 = 41;
            public const int MedEnd4 = 42;
        }

        public class ImportFollowUpSheetRow
        {
            private readonly ImportFollowUpSheet _importFollowUpSheet;
            private readonly int _currentIndex;

            private string[] _columns = new[] { 
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", 
                "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
                "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
                "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ",
                "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ",
                "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ",
            };

            public ImportFollowUpSheetRow(ImportFollowUpSheet importFollowUpSheet, int currentIndex)
            {
                _importFollowUpSheet = importFollowUpSheet;
                _currentIndex = currentIndex;
            }

            public string Column1
            {
                get { return GetCellValue(_currentIndex, Columns.Column1); }
            }

            public string Form
            {
                get { return GetCellValue(_currentIndex, Columns.Form); }
            }

            public string TreatmentSite
            {
                get { return GetCellValue(_currentIndex, Columns.TreatmentSite); }
            }

            public string ASMCode
            {
                get { return GetCellValue(_currentIndex, Columns.ASMCode); }
            }

            public string LastName
            {
                get { return GetCellValue(_currentIndex, Columns.LastName); }
            }

            public string FirstName
            {
                get { return GetCellValue(_currentIndex, Columns.FirstName); }
            }

            public string ASMCode1
            {
                get { return GetCellValue(_currentIndex, Columns.ASMCode1); }
            }

            public DateTime? DateOfBirth
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.DateOfBirth); }
            }

            public string Age
            {
                get { return GetCellValue(_currentIndex, Columns.Age); }
            }

            public string Sex
            {
                get { return GetCellValue(_currentIndex, Columns.Sex); }
            }

            public string Weight
            {
                get { return GetCellValue(_currentIndex, Columns.Weight); }
            }

            public string Height
            {
                get { return GetCellValue(_currentIndex, Columns.Height); }
            }

            public string MedName1
            {
                get { return GetCellValue(_currentIndex, Columns.MedName1); }
            }

            public string MedIndication1
            {
                get { return GetCellValue(_currentIndex, Columns.MedIndication1); }
            }

            public string MedDose1
            {
                get { return GetCellValue(_currentIndex, Columns.MedDose1); }
            }

            public DateTime? MedStart1
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedStart1); }
            }

            public DateTime? MedEnd1
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedEnd1); }
            }

            public string MedName2
            {
                get { return GetCellValue(_currentIndex, Columns.MedName2); }
            }

            public string MedIndication2
            {
                get { return GetCellValue(_currentIndex, Columns.MedIndication2); }
            }

            public string MedDose2
            {
                get { return GetCellValue(_currentIndex, Columns.MedDose2); }
            }

            public DateTime? MedStart2
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedStart2); }
            }

            public DateTime? MedEnd2
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedEnd2); }
            }

            public string MedName3
            {
                get { return GetCellValue(_currentIndex, Columns.MedName3); }
            }

            public string MedIndication3
            {
                get { return GetCellValue(_currentIndex, Columns.MedIndication3); }
            }

            public string MedDose3
            {
                get { return GetCellValue(_currentIndex, Columns.MedDose3); }
            }

            public DateTime? MedStart3
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedStart3); }
            }

            public DateTime? MedEnd3
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedEnd3); }
            }

            public string MedName4
            {
                get { return GetCellValue(_currentIndex, Columns.MedName4); }
            }

            public string MedIndication4
            {
                get { return GetCellValue(_currentIndex, Columns.MedIndication4); }
            }

            public string MedDose4
            {
                get { return GetCellValue(_currentIndex, Columns.MedDose4); }
            }

            public DateTime? MedStart4
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedStart4); }
            }

            public DateTime? MedEnd4
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.MedEnd4); }
            }


            private string GetCellValue(int rowIndex, int columnindex)
            {
                var cell = LocateCell(rowIndex, columnindex);
                if(cell == null)
                {
                    return string.Empty;
                }

                string value = "";
                try
                {
                    value = cell.CellValue.InnerText;
                }
                catch (Exception ex)
                {
                    value = string.Empty;
                }
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return _importFollowUpSheet._spreadsheetDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                }
                return value;
            }

            private bool? GetCellValueAsBool(int rowIndex, int columnindex)
            {
                var value = GetCellValue(rowIndex, columnindex);


                if (String.IsNullOrWhiteSpace(value))
                {
                    return null;
                }
                if (value.ToLowerInvariant() == "yes")
                {
                    return true;
                }
                if (value.ToLowerInvariant() == "no")
                {
                    return false;
                }
                throw new FormatException(string.Format("Was expecting a value of '', 'yes' or 'no', but received '{0}'", value));
            }

            private DateTime? GetCellValueAsDate(int rowIndex, int columnindex)
            {
                var value = GetCellValue(rowIndex, columnindex);

                if (String.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                var dbl = Convert.ToDouble(value);
                DateTime tempDate = DateTime.FromOADate(dbl);

                return tempDate;
            }

            private string[] GetCellValueAsStringArray(int rowIndex, int columnindex)
            {
                var value = GetCellValue(rowIndex, columnindex);

                if (value != "N/A" && !String.IsNullOrWhiteSpace(value))
                {
                    if (value.IndexOf(",") > -1)
                    {
                        string[] seperator = new string[1] { "," };
                        return value.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        string[] retValues = new string[1];
                        retValues[0] = value;
                        return retValues;
                    }
                }

                return new string[0];
            }

            private Cell LocateCell(int rowIndex, int columnindex)
            {
                var row = (Row)_importFollowUpSheet._rows.ChildElements[rowIndex];
                var comparisonCellReference = GetExcelCellReference(rowIndex, columnindex);

                foreach (Cell child in row.ChildElements)
                {
                    if (child.CellReference == comparisonCellReference)
                    {
                        return child;
                    }
                }
                return null;
            }

            private string GetExcelCellReference(int row, int column)
            {
                string cellAddress = "";
                cellAddress = (column > 26 ? _columns[(column / 26) - 1] : "") + _columns[column % 26] + (row + 1);
                return cellAddress;
            }
        }

        public class ImportFollowUpSheetRowEnumerator : IEnumerator<ImportFollowUpSheetRow>
        {
            private readonly ImportFollowUpSheet _file;
            private int _currentIndex;
            private ImportFollowUpSheetRow _current;

            public ImportFollowUpSheetRowEnumerator(ImportFollowUpSheet file)
            {
                _file = file;
                Reset();
            }

            public bool MoveNext()
            {
                _currentIndex++;
                _current = new ImportFollowUpSheetRow(_file, _currentIndex);
                try
                {
                    if (String.IsNullOrWhiteSpace(_current.Column1))
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _currentIndex = 0;
                _current = null;
            }

            public ImportFollowUpSheetRow Current
            {
                get { return _current; }
            }

            object IEnumerator.Current
            {
                get { return ((IEnumerator<ImportFollowUpSheetRow>)this).Current; }
            }

            public void Dispose()
            {
            }
        }
    }

}
