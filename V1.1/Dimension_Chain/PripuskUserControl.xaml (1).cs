using System.Windows.Controls;

namespace Dimension_Chain
{
    /// <summary>
    /// Логика взаимодействия для PripuskUserControl.xaml
    /// </summary>
    public partial class PripuskUserControl : UserControl
    {
        UI_PripuskDimension UI_D;                                         // размер, к которому в данный момент привязан юзерконтрол

        public PripuskUserControl()
        {
            InitializeComponent();
        }

        public void SetUI_PD(UI_PripuskDimension UI_PD)
        {
            UI_D = UI_PD;
            textBoxMin.Text = UI_D.min.ToString();
            textBoxMax.Text = UI_D.max.ToString();
        }
        public void ReNull()
        {
            UI_D = null;
            textBoxMin.Text = "0";
            textBoxMax.Text = "0";
        }

        private void textBoxMin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI_D != null)
            {
                Methods.ParseTextBox(textBoxMin, ref UI_D.min);
                UI_D.lblPripusk.Content = UI_D.min.ToString() + "..." + UI_D.max.ToString();
                UI_D.lblNominal.UpdateLayout();     // для обновления визуализированного lblNominal
                UI_D.SetOtherLabels();
                UI_D.PUC_Apdated();          // вызываем событие
            }
        }

        private void textBoxMax_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI_D != null)
            {
                Methods.ParseTextBox(textBoxMax, ref UI_D.max);
                UI_D.lblPripusk.Content = UI_D.min.ToString() + "..." + UI_D.max.ToString();
                UI_D.lblNominal.UpdateLayout();     // для обновления визуализированного lblNominal
                UI_D.SetOtherLabels();
                UI_D.PUC_Apdated();          // вызываем событие
            }
        }
    }
}
