using System;
using System.Collections.Generic;
using LegendsViewer.Legends.Parser;

namespace LegendsViewer.Legends.Events
{
    public class ArtifactStored : WorldEvent
    {
        public Artifact Artifact { get; set; }
        public int UnitID { get; set; }
        public HistoricalFigure HistoricalFigure { get; set; }
        public Site Site { get; set; }

        public ArtifactStored(List<Property> properties, World world)
            : base(properties, world)
        {
            foreach (Property property in properties)
            {
                switch (property.Name)
                {
                    case "artifact_id": Artifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                    case "unit_id": UnitID = Convert.ToInt32(property.Value); break;
                    case "hist_figure_id": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                    case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                }
            }

            Artifact.AddEvent(this);
            HistoricalFigure.AddEvent(this);
            Site.AddEvent(this);
        }

        public override string Print(bool link = true, DwarfObject pov = null)
        {
            string eventString = GetYearTime() + Artifact.ToLink(link, pov) + " was stored in " + Site.ToLink(link, pov) + " by " + HistoricalFigure.ToLink(link, pov);
            eventString += PrintParentCollection(link, pov);
            eventString += ".";
            return eventString;
        }
    }
}