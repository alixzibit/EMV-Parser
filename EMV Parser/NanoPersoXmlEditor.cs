using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EMV_Parser
{
    public class NanoPersoXmlEditor
    {
        private readonly string _nanoPersoXmlFilePath;

        public NanoPersoXmlEditor(string nanoPersoXmlFilePath)
        {
            _nanoPersoXmlFilePath = nanoPersoXmlFilePath;
        }

        public void Edit(Dictionary<string, string> tagValueMap)
        {
            XDocument doc = XDocument.Load(_nanoPersoXmlFilePath);
            var nodes = doc.Descendants("NODE");
            foreach (var node in nodes)
            {
                if (node.Attribute("TYPE")?.Value == "EMV2000")
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    string tag = node.Attribute("TAG")?.Value;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    string value = node.Attribute("VALUE")?.Value;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (!string.IsNullOrWhiteSpace(tag) && !string.IsNullOrWhiteSpace(value))
                    {
                        if (tagValueMap.ContainsKey(tag))
                        {
                            node.Attribute("VALUE")?.SetValue(tagValueMap[tag]);
                        }
                    }
                }
            }

            doc.Save(_nanoPersoXmlFilePath);
        }
    }
}
