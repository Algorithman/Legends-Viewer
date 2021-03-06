using System;
using System.Collections.Generic;
using System.Linq;
using LegendsViewer.Legends.Parser;

namespace LegendsViewer.Legends.Events
{
    public class HFProfanedStructure : WorldEvent
    {
        public int Action { get; set; } // legends_plus.xml
        public HistoricalFigure HistoricalFigure { get; set; }
        public Site Site { get; set; }
        public int StructureID { get; set; }
        public Structure Structure { get; set; }

        public HFProfanedStructure(List<Property> properties, World world)
            : base(properties, world)
        {
            foreach (Property property in properties)
            {
                switch (property.Name)
                {
                    case "hist_fig_id": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                    case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                    case "structure_id": StructureID = Convert.ToInt32(property.Value); break;
                    case "structure": StructureID = Convert.ToInt32(property.Value); break;
                    case "histfig": if (HistoricalFigure == null) { HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else property.Known = true; break;
                    case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else property.Known = true; break;
                    case "action": Action = Convert.ToInt32(property.Value); break;
                }
            }
            if (Site != null)
            {
                Structure = Site.Structures.FirstOrDefault(structure => structure.ID == StructureID);
            }
            HistoricalFigure.AddEvent(this);
            Site.AddEvent(this);
            Structure.AddEvent(this);
        }

        public override string Print(bool link = true, DwarfObject pov = null)
        {
            string eventString = GetYearTime() + HistoricalFigure.ToLink(link, pov) + " profaned ";
            eventString += Structure != null ? Structure.ToLink(link, pov) : "UNKNOWN STRUCTURE";
            eventString += " in " + Site.ToLink(link, pov);
            eventString += PrintParentCollection(link, pov);
            eventString += ".";
            return eventString;
        }
    }
}