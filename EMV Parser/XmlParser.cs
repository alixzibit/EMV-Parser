using EMV_Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
//MAIN PARSER IN USE FOR MCHIP OUTPUT FILE (NOTE MCHIP OUTPUT FILE NEEDS TO BE GENERATED FROM PROFILE CONVERTER FOR
//PERSO TOOL NOT THE OUTPUT FILE WE USE FOR UL)
public class XmlParser
{
    private readonly string _filePath;

    public XmlParser(string filePath)
    {
        _filePath = filePath;
    }

    public List<TagData> Parse()
    {
        List<TagData> tagDataList = new List<TagData>();

        if (File.Exists(_filePath))
        {
            XElement xmlData = XElement.Load(_filePath);

            // Query for the WORKSHEET elements, ignoring the namespace
            var worksheets = xmlData.Descendants().Where(e => e.Name.LocalName == "WORKSHEET").ToList();

            foreach (var worksheet in worksheets)
            {
                string category = worksheet.Attribute("NAME").Value.Trim();

                // Parse data from <ELEM> elements with the NAME, TAG, and VALUE attributes, ignoring the namespace
                var elemData = worksheet.Descendants().Where(e => e.Name.LocalName == "ELEM")
                    .Where(e => e.Attribute("NAME") != null && e.Attribute("TAG") != null && e.Attribute("VALUE") != null)
                    .Where(e => !string.IsNullOrEmpty(e.Attribute("VALUE").Value.Trim()) && !string.IsNullOrEmpty(e.Attribute("TAG").Value.Trim())) // Ignore rows with empty VALUE and TAG attributes
                    .Select(e => new TagData(
                        e.Attribute("NAME").Value.Trim(),
                        e.Attribute("TAG").Value.Trim(),
                        category, // Swapped the order of 'category' and 'value'
                        e.Attribute("VALUE").Value.Trim()
                    ));


                tagDataList.AddRange(elemData);
            }
        }

        return tagDataList;
    }
}
