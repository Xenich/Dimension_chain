using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dimension_Chain
{
    /// <summary>
    /// Логика взаимодействия для DimensionUserControl.xaml
    /// </summary>
    [Serializable]
    partial class TechUserControl : UserControl         
    {
        UI_TechDimension UI_D;                                         // размер, к которому в данный момент привязан юзерконтрол

        public TechUserControl()
        {
            InitializeComponent();
        }

        public void SetUI_TD(UI_TechDimension UI_TD)
        {
            UI_D = UI_TD;
            textBoxNominal.Text = UI_TD.nominal.ToString();
            textBoxUp.Text = UI_TD.up.ToString();
            textBoxDown.Text = UI_TD.down.ToString();
        }

        public void ReNull()
        {
            UI_D = null;
            textBoxNominal.Text = "20";
            textBoxUp.Text = "0";
            textBoxDown.Text = "0";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double up;
            double down;
            Double.TryParse(textBoxUp.Text, out up);
            down = -1 * up;
            textBoxDown.Text = down.ToString();
        }

        private void textBoxNominal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI_D != null)
            {
                Methods.ParseTextBox(textBoxNominal, UI_D.lblNominal, ref UI_D.nominal);
                UI_D.lblNominal.UpdateLayout();     // для обновления визуализированного lblNominal
                UI_D.SetUpDownLbls();
                UI_D.TUC_Apdated();
            }
        }

        private void textBoxUp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI_D != null)
            {
                Methods.ParseTextBox(textBoxUp, UI_D.lblUp, ref UI_D.up);
                UI_D.SetUpDownLbls();
                UI_D.TUC_Apdated();
            }
        }

        private void textBoxDown_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI_D != null)
            {
                Methods.ParseTextBox(textBoxDown, UI_D.lblDown, ref UI_D.down);
                UI_D.SetUpDownLbls();
                UI_D.TUC_Apdated();
            }
        }
    }
}
