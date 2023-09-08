using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace EMV_Parser
{
    //experimental parser for using a word file for parsing
    public class WordParser
    {
        private readonly string _filePath;

        public WordParser(string filePath)
        {
            _filePath = filePath;
        }

        public List<Dictionary<string, string>> Parse()
        {
            List<Dictionary<string, string>> tableDataList = new List<Dictionary<string, string>>();

            if (File.Exists(_filePath))
            {
                using WordprocessingDocument doc = WordprocessingDocument.Open(_filePath, false);
                var tables = doc.MainDocumentPart.Document.Body.Descendants<Table>();
                foreach (var table in tables)
                {
                    foreach (var row in table.Descendants<TableRow>())
                    {
                        Dictionary<string, string> tableData = new Dictionary<string, string>();
                        var cells = row.Descendants<TableCell>();
                        if (cells.Count() == 3)
                        {
                            tableData.Add("Tag", cells.ElementAt(0).InnerText.Trim());
                            tableData.Add("Element Name", cells.ElementAt(1).InnerText.Trim());
                            tableData.Add("Data", cells.ElementAt(2).InnerText.Trim());
                            tableDataList.Add(tableData);
                        }
                    }
                }
            }

            return tableDataList;
        }
    }
}
