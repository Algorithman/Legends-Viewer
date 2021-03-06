﻿using System.Drawing;

namespace LegendsViewer.Controls.HTML.Utilities
{
    public static class HTMLStyleUtil
    {
        public const string SYMBOL_POPULATION   = "<span class=\"legends_symbol_population\">&#9823;</span>";
        public const string SYMBOL_SITE         = "<span class=\"legends_symbol_site\">&#9978;</span>";
        public const string SYMBOL_DEAD         = "<span class=\"legends_symbol_dead\">&#10013;</span>";

        public static string CurrentDwarfObject(string name)
        {
            return "<span class=\"legends_current_dwarfobject\">" + name + "</span>";
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
    }
}
