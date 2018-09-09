using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Dimension_Chain
{
    static class Methods
    {
        public static void ParseTextBox(TextBox tBox, Label lbl, ref double d)
        {
            if (Double.TryParse(tBox.Text, out d))
                lbl.Content = d;
            else
            {
                if (tBox.Text == "" || tBox.Text == "-" || tBox.Text == "+")
                {
                    lbl.Content = 0;
                    d = 0;
                }
                else
                    tBox.Text = lbl.Content.ToString();
            }
        }

        public static void ParseTextBox(TextBox tBox, ref double d)
        {
            double k = d;
            if (Double.TryParse(tBox.Text, out k))
                d = k;
            else
            {
                if (tBox.Text == "" || tBox.Text == "-" || tBox.Text == "+")
                    d = 0;
                else
                    tBox.Text = d.ToString();
            }
        }
    }
}
