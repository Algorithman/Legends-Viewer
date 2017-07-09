using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegendsViewer.Controls;
namespace LegendsViewer
{
    public class LegendsBookmark
    {
        public string WorldName { get; set; }
        public string WorldObjectDescriptor { get; set; }

        public ControlOption controlType { get; set; }

        public override string ToString()
        {
            return WorldName + " -> " + WorldObjectDescriptor;
        }
    }
}
