using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using ClosedXML;
using NLog;
using System.Text;

namespace EMV_Parser
{
    public class ExcelParser
    {
        private string filePath;

        private string ReadMultiLineCellValue(ExcelWorksheet worksheet, int row, int column)
        {
            StringBuilder sb = new StringBuilder();
            object value = worksheet.Cells[row, column].Value;

            if (value != null)
            {
                sb.Append(value.ToString());

                for (int i = row + 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    object nextValue = worksheet.Cells[i, column].Value;

                    if (nextValue == null)
                    {
                        break;
                    }

                    sb.AppendLine();
                    sb.Append(nextValue.ToString());
                }
            }

            return sb.ToString();
        }

        public ExcelParser(string filePath)
        {
            this.filePath = filePath;
        }

        public List<TagData> Parse()
        {
            List<TagData> tagDataList = new List<TagData>();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Find the row index that contains the "Name", "Tag", "Value" headers
                int headerRowIndex = -1;
                for (int row = 1; row <= rowCount; row++)
                {
                    bool nameFound = false;
                    bool tagFound = false;
                    bool valueFound = false;

                    for (int col = 1; col <= colCount; col++)
                    {
                        string cellValue = worksheet.Cells[row, col].Value?.ToString();

                        if (cellValue != null)
                        {
                            if (cellValue.Equals("Name", StringComparison.OrdinalIgnoreCase))
                                nameFound = true;
                            else if (cellValue.Equals("Tag", StringComparison.OrdinalIgnoreCase))
                                tagFound = true;
                            else if (cellValue.Equals("Value", StringComparison.OrdinalIgnoreCase))
                                valueFound = true;
                        }
                    }

                    if (nameFound && tagFound && valueFound)
                    {
                        headerRowIndex = row;
                        break;
                    }
                }


                // If we couldn't find the headers, return an empty list
                if (headerRowIndex == -1)
                {
                    return tagDataList;
                }

                string currentCategory = string.Empty;

                // Read the data rows below the header
                for (int row = 1; row < headerRowIndex; row++)
                {
                    currentCategory = worksheet.Cells[row, 1].Value?.ToString();
                }

                for (int row = headerRowIndex + 1; row <= rowCount; row++)
                {
                    string name = ReadMultiLineCellValue(worksheet, row, 1);
                    string tag = ReadMultiLineCellValue(worksheet, row, 2);
                    string value = ReadMultiLineCellValue(worksheet, row, 3);

                    if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(tag) || !string.IsNullOrWhiteSpace(value))
                    {
                        tagDataList.Add(new TagData
                        {
                            ElementName = name,
                            Tag = tag,
                            Value = value,
                            Category = currentCategory
                        });
                    }
                }
            }

            return tagDataList;
        }
    }
}