using System;
using System.Windows.Controls;
using System.Windows.Media;


namespace Dimension_Chain
{
    [Serializable]
    public partial class UI_TechDimension : UI_Dimension
    {
        public delegate void TechDimensionApdatedEventHandler(UI_TechDimension dim);
        public event TechDimensionApdatedEventHandler TechDimensionApdatedEvent;         // событие при изменении размера (номинала или допусков)

        public UI_TechDimension(TechUserControl TUC) : base(TUC)
        {
            // идёт основной конструктор базового класса ............
            ConstructorPart();
            nominal = 20;
            up = 0;
            down = 0;
            TUC.SetUI_TD(this);
            lblNominal.Content = nominal;
            lblUp.Content = up;
            lblDown.Content = down;
            lblNominal.UpdateLayout();  // для обновления lblNominal
            SetUpDownLbls();
        }

        private void ConstructorPart()
        {
            tp = type.tech;
            mainLine.Stroke = Brushes.Black;
            firstLine.Stroke = Brushes.Black;
            secondLine.Stroke = Brushes.Black;
            firstArrowUp.Stroke = Brushes.Black;
            firstArrowDown.Stroke = Brushes.Black;
            secondArrowUp.Stroke = Brushes.Black;
            secondArrowDown.Stroke = Brushes.Black;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
            // произошёл апдейт соответствующего юзерконтрола
        public void TUC_Apdated()
        {
            if (TechDimensionApdatedEvent != null)
                TechDimensionApdatedEvent(this);
        }

            // отписка от всех событий при удалении размера
        public override void OtpiskaWhenDelate()
        {
            (UC as TechUserControl).ReNull();
        }

            // Удаление с канваса всех остальных лейблов помимо номинала и пределов, которые содержит размер - виртуальный метод, переопределён.
        public override void RemoveOtherLabels()
        {}

            // отмена выбора размера с обнулением юзерконтрола, к нему привязанного
        public override void Unchoose()
        {
            (UC as TechUserControl).ReNull();
            UC = null;
        }
    }
}
