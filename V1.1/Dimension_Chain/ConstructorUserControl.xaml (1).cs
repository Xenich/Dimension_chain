using System;
using System.Windows;
using System.Windows.Controls;


namespace Dimension_Chain
{
    /// <summary>
    /// Логика взаимодействия для ConstructorUserControl.xaml
    /// </summary>
    partial class ConstructorUserControl : UserControl
    {
        UI_ConstrDimension UI_D;                                         // размер, к которому в данный момент привязан юзерконтрол

        public ConstructorUserControl()
        {
            InitializeComponent();
        }

        public void SetUI_CD(UI_ConstrDimension UI_CD)
        {
            UI_D = UI_CD;
            textBoxNominal.Text = UI_CD.nominal.ToString();
            textBoxUp.Text = UI_CD.up.ToString();
            textBoxDown.Text = UI_CD.down.ToString();
        }

        public void ReNull()
        {
            UI_D = null;
            textBoxNominal.Text = "0";
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
                Methods.ParseTextBox(textBoxNominal, UI_D.lblNominalConstr, ref UI_D.nominal);
                UI_D.max = UI_D.nominal + UI_D.up;
                UI_D.min = UI_D.nominal + UI_D.down;

                UI_D.lblNominalConstr.Content = UI_D.nominal.ToString();

                UI_D.lblNominal.UpdateLayout();     // для обновления визуализированного lblNominal
                UI_D.SetOtherLabels();
                UI_D.CUC_Apdated();
            }
        }

        private void textBoxUp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI_D != null)
            {
                Methods.ParseTextBox(textBoxUp, UI_D.lblUpConstr, ref UI_D.up);
                UI_D.max = UI_D.nominal + UI_D.up;
                UI_D.lblUpConstr.Content = UI_D.lblUpConstr.Content = UI_D.up > 0 ? "+" + UI_D.up.ToString() : UI_D.up.ToString();
                UI_D.lblNominal.UpdateLayout();     // для обновления визуализированного lblNominal
                UI_D.SetOtherLabels();
                UI_D.CUC_Apdated();           // вызываем событие
            }
        }

        private void textBoxDown_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI_D != null)
            {
                Methods.ParseTextBox(textBoxDown, UI_D.lblDownConstr, ref UI_D.down);
                UI_D.min = UI_D.nominal + UI_D.down;
                UI_D.lblDownConstr.Content = UI_D.lblDownConstr.Content = UI_D.down > 0 ? "+" + UI_D.down.ToString() : UI_D.down.ToString();
                UI_D.lblNominal.UpdateLayout();     // для обновления визуализированного lblNominal
                UI_D.SetOtherLabels();
                UI_D.CUC_Apdated();          // вызываем событие
            }
        }
    }
}
