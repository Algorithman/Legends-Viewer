﻿using System.Collections.Generic;
using System.Xml;

namespace LegendsViewer.Legends.Parser
{
    class XMLPlusParser : XMLParser
    {
        private bool inMiddleOfSection = false;
        private List<Property> CurrentItem;

        public XMLPlusParser(World world, string xmlFile) : base(xmlFile)
        {
            World = world;
            Parse();
        }

        new public void Parse()
        {
            if (XML.ReadState == ReadState.Closed)
                return;

            while (!XML.EOF && CurrentItem == null)
            {

                if (!inMiddleOfSection)
                    CurrentSection = GetSectionType(XML.Name);
                if (CurrentSection == Section.Junk)
                {
                    XML.Read();
                }
                else if (CurrentSection == Section.Unknown)
                    SkipSection();
                else
                    ParseSection();
            }

            if (XML.EOF)
            {
                XML.Close();
            }
        }

        private new void ParseSection()
        {

            while (XML.NodeType == XmlNodeType.EndElement || XML.NodeType == XmlNodeType.None)
            {
                if (XML.NodeType == XmlNodeType.None)
                    return;
                XML.ReadEndElement();
            }

            if (!inMiddleOfSection)
            {
                XML.ReadStartElement();
                inMiddleOfSection = true;
            }

            CurrentItem = ParseItem();

            if (XML.NodeType == XmlNodeType.EndElement)
            {
                XML.ReadEndElement();
                inMiddleOfSection = false;
            }
        }

        public void AddNewProperties(List<Property> existingProperties, Section xmlParserSection)
        {
            if (CurrentItem == null)
                return;

            if (xmlParserSection > CurrentSection)
            {
                while (xmlParserSection > CurrentSection)
                {
                    AddItemToWorld(CurrentItem);
                    CurrentItem = null;
                    Parse();
                }
            }

            if (xmlParserSection < CurrentSection)
            {
                return;
            }

            if (CurrentItem != null)
            {
                Property id = existingProperties.Find(property => property.Name == "id");
                Property currentId = CurrentItem.Find(property => property.Name == "id");
                while (currentId != null && currentId.ValueAsInt() < 0)
                {
                    CurrentItem = ParseItem();
                    if (CurrentItem != null) currentId = CurrentItem.Find(property => property.Name == "id");
                }
                if (id != null && currentId != null && id.ValueAsInt().Equals(currentId.ValueAsInt()))
                {
                    foreach (var property in CurrentItem)
                    {
                        if (CurrentSection == Section.Entities && (property.Name == "entity_link" || property.Name == "child" || property.Name == "entity_position" || property.Name == "entity_position_assignment" || property.Name == "occasion"))
                        {
                            existingProperties.Add(property);
                            continue;
                        }
                        if (CurrentSection == Section.Artifacts && property.Name == "writing")
                        {
                            existingProperties.Add(property);
                            continue;
                        }
                        if (CurrentSection == Section.WrittenContent && property.Name == "style")
                        {
                            existingProperties.Add(property);
                            continue;
                        }
                        Property matchingProperty = existingProperties.Find(p => p.Name == property.Name);
                        if (CurrentSection == Section.Events && matchingProperty != null && 
                            (matchingProperty.Name == "type" || matchingProperty.Name == "state" || matchingProperty.Name == "slayer_race"))
                        {
                            continue;
                        }

                        if (matchingProperty != null)
                        {
                            matchingProperty.Value = property.Value;
                            matchingProperty.Known = false;
                            if (matchingProperty.SubProperties == null)
                            {
                                matchingProperty.SubProperties = property.SubProperties;
                            }
                            else
                            {
                                matchingProperty.SubProperties.AddRange(property.SubProperties);
                            }
                        }
                        else
                        {
                            existingProperties.Add(property);
                        }
                    }
                    CurrentItem = null;
                    Parse();
                }
            }
        }
    }
}