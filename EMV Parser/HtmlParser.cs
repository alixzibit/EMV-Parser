using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NLog;

namespace EMV_Parser
{
    public class HtmlParser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _filePath;

        public HtmlParser(string filePath)
        {
            _filePath = filePath;
        }

        public List<TagData> Parse()
        {
            var tagDataList = new List<TagData>();
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.Load(_filePath);

                var tableRows = htmlDoc.DocumentNode.Descendants("tr");
                foreach (var row in tableRows)
                {
                    var columns = row.Descendants("td").ToList();
                    if (columns.Count == 6)
                    {
                        var elementName = columns[0].InnerText.Trim(); // Changed index from 1 to 0
                        var tagName = columns[1].InnerText.Trim(); // Changed index from 0 to 1
                        var data = columns[5].InnerHtml.Trim();
                        var category = Regex.Match(columns[2].InnerHtml, @"(?<=<span>)(.*?)(?=<\/span>)").Value.Replace("&amp;", "&").Trim();

                        // Extracting the hexadecimal values before the binary bitmap values
                        var pattern = @"^((?:[\da-fA-F]{2}\s)+)[^\[]*";
                        var regex = new Regex(pattern);
                        var match = regex.Match(data);
                        if (match.Success)
                        {
                            data = match.Groups[1].Value.TrimEnd().Replace(" ", "");
                        }
                        else
                        {
                            data = data.Replace(" ", "");
                        }

                        var tagData = new TagData(elementName, tagName, category, data);
                        tagDataList.Add(tagData);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception occurred while parsing HTML file {FilePath}. Exception details: {Message}", _filePath, ex.Message);
            }

            return tagDataList;
        }
    }
}
