﻿using LegendsViewer.Controls.HTML.Utilities;
using System.Collections.Generic;
using System.Linq;
using LegendsViewer.Legends.Events;
using LegendsViewer.Legends.Parser;
using System;
using LegendsViewer.Legends.Enums;

namespace LegendsViewer.Legends
{
    public class WrittenContent : WorldObject
    {
        public string Name { get; set; } // legends_plus.xml
        public int PageStart { get; set; } // legends_plus.xml
        public int PageEnd { get; set; } // legends_plus.xml
        public WrittenContentType Type { get; set; } // legends_plus.xml
        public HistoricalFigure Author { get; set; } // legends_plus.xml
        public List<string> Styles { get; set; } // legends_plus.xml
        public List<Reference> References { get; set; } // legends_plus.xml
        public int PageCount { get { return PageEnd - PageStart + 1; } set { } }

        public static string Icon = "<i class=\"fa fa-fw fa-book\"></i>";

        public static List<string> Filters;
        public override List<WorldEvent> FilteredEvents
        {
            get { return Events.Where(dwarfEvent => !Filters.Contains(dwarfEvent.Type)).ToList(); }
        }

        public WrittenContent(List<Property> properties, World world)
            : base(properties, world)
        {
            Name = "Untitled";
            Styles = new List<string>();
            References = new List<Reference>();

            foreach (Property property in properties)
            {
                switch (property.Name)
                {
                    case "title": Name = Formatting.InitCaps(property.Value); break;
                    case "page_start": PageStart = Convert.ToInt32(property.Value); break;
                    case "page_end": PageEnd = Convert.ToInt32(property.Value); break;
                    case "reference":
                        property.Known = true;
                        if (property.SubProperties != null)
                            References.Add(new Reference(property.SubProperties, world)); break;
                    case "type":
                        switch (property.Value)
                        {
                            case "Autobiography": Type = WrittenContentType.Autobiography; break;
                            case "Biography": Type = WrittenContentType.Biography; break;
                            case "Chronicle": Type = WrittenContentType.Chronicle; break;
                            case "Dialog": Type = WrittenContentType.Dialog; break;
                            case "Essay": Type = WrittenContentType.Essay; break;
                            case "Guide": Type = WrittenContentType.Guide; break;
                            case "Letter": Type = WrittenContentType.Letter; break;
                            case "Manual": Type = WrittenContentType.Manual; break;
                            case "Novel": Type = WrittenContentType.Novel; break;
                            case "Play": Type = WrittenContentType.Play; break;
                            case "Poem": Type = WrittenContentType.Poem; break;
                            case "ShortStory": Type = WrittenContentType.ShortStory; break;
                            case "MusicalComposition": Type = WrittenContentType.MusicalComposition; break;
                            case "Choreography": Type = WrittenContentType.Choreography; break;
                            case "CulturalHistory": Type = WrittenContentType.CulturalHistory; break;
                            case "StarChart": Type = WrittenContentType.StarChart; break;
                            case "ComparativeBiography": Type = WrittenContentType.ComparativeBiography; break;
                            case "CulturalComparison": Type = WrittenContentType.CulturalComparison; break;
                            case "Atlas": Type = WrittenContentType.Atlas; break;
                            case "TreatiseOnTechnologicalEvolution": Type = WrittenContentType.TreatiseOnTechnologicalEvolution; break;
                            case "AlternateHistory": Type = WrittenContentType.AlternateHistory; break;
                            case "StarCatalogue": Type = WrittenContentType.StarCatalogue; break;
                            case "Dictionary": Type = WrittenContentType.Dictionary; break;
                            case "Genealogy": Type = WrittenContentType.Genealogy; break;
                            case "Encyclopedia": Type = WrittenContentType.Encyclopedia; break;
                            case "BiographicalDictionary": Type = WrittenContentType.BiographicalDictionary; break;
                            default:
                                Type = WrittenContentType.Unknown;
                                //world.ParsingErrors.Report("|==> WrittenContentType: " + property.Value);
                                int typeID;
                                if (!int.TryParse(property.Value.Replace("unknown ", ""), out typeID))
                                {
                                    property.Known = false;
                                }
                                break;
                        }
                        break;
                    case "author": Author = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                    case "style": Styles.Add(string.Intern(property.Value)); break;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override string ToLink(bool link = true, DwarfObject pov = null)
        {
            if (link)
            {
                string type = null;
                if (Type != WrittenContentType.Unknown)
                {
                    type = Type.GetDescription();
                }
                string title = "Written Content";
                title += string.IsNullOrWhiteSpace(type) ? "" : ", " + type;
                title += "&#13";
                title += "Events: " + Events.Count;

                string linkedString = "";
                if (pov != this)
                {
                    linkedString = Icon + "<a href = \"writtencontent#" + ID + "\" title=\"" + title + "\">" + Name + "</a>";
                }
                else
                {
                    linkedString = Icon + "<a title=\"" + title + "\">" + HTMLStyleUtil.CurrentDwarfObject(Name) + "</a>";
                }
                return linkedString;
            }
            else
            {
                return Name;
            }
        }
        public override string GetBookmark()
        {
            throw new NotImplementedException();
        }
    }
}
