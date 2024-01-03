using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PViMS.Services.FileHandler
{
    public class ImportBaselineSheet : IEnumerable<ImportBaselineSheet.ImportBaselineSheetRow>, IDisposable
    {
        //private readonly ImportEnrollmentRow _currentRow;
        private SpreadsheetDocument _spreadsheetDocument;
        private Sheet _sheet;
        private Worksheet _worksheet;
        private SheetData _rows;

        public ImportBaselineSheet(Stream file)
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

        public IEnumerator<ImportBaselineSheetRow> GetEnumerator()
        {
            return new ImportBaselineSheetRowEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static class Columns
        {
            public const int Form = 0;
            public const int TreatmentSite = 1;
            public const int ASMCode = 2;
            public const int DateOfBirth = 3;
            public const int Age = 4;
            public const int Sex = 5;
            public const int Weight = 6;
            public const int Height = 7;
            public const int Pregnant = 8;
            public const int LMP = 9;
            public const int Gestation = 10;
            public const int Address = 11;
            public const int PhoneNumber = 12;
            public const int Alcohol = 14;
            public const int Smoking = 15;
            public const int OtherSubstance = 16;
            public const int BloodPressure = 18;
            public const int Diabetes = 19;
            public const int CardioVascularDisease = 21;
            public const int HepBCo = 23;
            public const int HepCCo = 25;
            public const int HepaticInsufficiency = 27;
            public const int RenalInsufficiency = 29;
            public const int Tuberculosis = 32;
            public const int MDR = 34;
            public const int MentalDisorder = 36;
            public const int ALTTestDate = 40;
            public const int ALTResult = 41;
            public const int ASTTestDate = 44;
            public const int ASTResult = 45;
            public const int CD4TestDate = 48;
            public const int CD4Result = 49;
            public const int GlycemiaTestDate = 56;
            public const int GlycemiaResult = 57;
            public const int TriglyceridesTestDate = 72;
            public const int TriglyceridesResult = 73;
        }

        public class ImportBaselineSheetRow
        {
            private readonly ImportBaselineSheet _importBaselineSheet;
            private readonly int _currentIndex;

            private string[] _columns = new[] { 
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", 
                "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
                "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
                "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ",
                "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ",
                "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ",
            };

            public ImportBaselineSheetRow(ImportBaselineSheet importBaselineSheet, int currentIndex)
            {
                _importBaselineSheet = importBaselineSheet;
                _currentIndex = currentIndex;
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

            public string Pregnant
            {
                get { return GetCellValue(_currentIndex, Columns.Pregnant); }
            }

            public DateTime? LMP
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.LMP); }
            }

            public string Gestation
            {
                get { return GetCellValue(_currentIndex, Columns.Gestation); }
            }

            public string Address
            {
                get { return GetCellValue(_currentIndex, Columns.Address); }
            }

            public string PhoneNumber
            {
                get { return GetCellValue(_currentIndex, Columns.PhoneNumber); }
            }

            public string Alcohol
            {
                get { return GetCellValue(_currentIndex, Columns.Alcohol); }
            }

            public string Smoking
            {
                get { return GetCellValue(_currentIndex, Columns.Smoking); }
            }

            public string OtherSubstance
            {
                get { return GetCellValue(_currentIndex, Columns.OtherSubstance); }
            }

            public string BloodPressure
            {
                get { return GetCellValue(_currentIndex, Columns.BloodPressure); }
            }

            public string Diabetes
            {
                get { return GetCellValue(_currentIndex, Columns.Diabetes); }
            }

            public string CardioVascularDisease
            {
                get { return GetCellValue(_currentIndex, Columns.CardioVascularDisease); }
            }

            public string HepBCo
            {
                get { return GetCellValue(_currentIndex, Columns.HepBCo); }
            }

            public string HepCCo
            {
                get { return GetCellValue(_currentIndex, Columns.HepCCo); }
            }

            public string HepaticInsufficiency
            {
                get { return GetCellValue(_currentIndex, Columns.HepaticInsufficiency); }
            }

            public string RenalInsufficiency
            {
                get { return GetCellValue(_currentIndex, Columns.RenalInsufficiency); }
            }

            public string Tuberculosis
            {
                get { return GetCellValue(_currentIndex, Columns.Tuberculosis); }
            }

            public string MDR
            {
                get { return GetCellValue(_currentIndex, Columns.MDR); }
            }

            public string MentalDisorder
            {
                get { return GetCellValue(_currentIndex, Columns.MentalDisorder); }
            }

            public DateTime? ALTTestDate
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.ALTTestDate); }
            }

            public string ALTResult
            {
                get { return GetCellValue(_currentIndex, Columns.ALTResult); }
            }

            public DateTime? ASTTestDate
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.ASTTestDate); }
            }

            public string ASTResult
            {
                get { return GetCellValue(_currentIndex, Columns.ASTResult); }
            }

            public DateTime? CD4TestDate
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.CD4TestDate); }
            }

            public string CD4Result
            {
                get { return GetCellValue(_currentIndex, Columns.CD4Result); }
            }

            public DateTime? GlycemiaTestDate
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.GlycemiaTestDate); }
            }

            public string GlycemiaResult
            {
                get { return GetCellValue(_currentIndex, Columns.GlycemiaResult); }
            }

            public DateTime? TriglyceridesTestDate
            {
                get { return GetCellValueAsDate(_currentIndex, Columns.TriglyceridesTestDate); }
            }

            public string TriglyceridesResult
            {
                get { return GetCellValue(_currentIndex, Columns.TriglyceridesResult); }
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
                    return _importBaselineSheet._spreadsheetDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
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
                var row = (Row)_importBaselineSheet._rows.ChildElements[rowIndex];
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

        public class ImportBaselineSheetRowEnumerator : IEnumerator<ImportBaselineSheetRow>
        {
            private readonly ImportBaselineSheet _file;
            private int _currentIndex;
            private ImportBaselineSheetRow _current;

            public ImportBaselineSheetRowEnumerator(ImportBaselineSheet file)
            {
                _file = file;
                Reset();
            }

            public bool MoveNext()
            {
                _currentIndex++;
                _current = new ImportBaselineSheetRow(_file, _currentIndex);
                try
                {
                    if (String.IsNullOrWhiteSpace(_current.Form))
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

            public ImportBaselineSheetRow Current
            {
                get { return _current; }
            }

            object IEnumerator.Current
            {
                get { return ((IEnumerator<ImportBaselineSheetRow>)this).Current; }
            }

            public void Dispose()
            {
            }
        }
    }

}
