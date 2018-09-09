using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;


namespace Dimension_Chain
{
    [Serializable]
    public partial class UI_PripuskDimension : UI_Dimension
    {
        public delegate void PripuskApdatedEventHandler(UI_Dimension dim);
        public event PripuskApdatedEventHandler PripuskApdatedEvent;              // событие при изменении припуска (желаемого пользователем максимума или минимума)

        public double max;     // желаемое максимальное значение припуска, введённое пользователем
        public double min;     // желаемое минимальное значение припуска, введённое пользователем

        public Label lblPripusk;
                
        MouseButtonEventHandler lblPripuskClick;          // делегат

        public UI_PripuskDimension(PripuskUserControl PUC) : base(PUC)
        {
                // идёт основной конструктор базового класса .......................................
            ConstructorPart();
            PUC.SetUI_PD(this);

            lblNominal.Content = "?";
            lblUp.Content = "?";
            lblDown.Content = "?";
            lblNominal.UpdateLayout();  // для обновления lblNominal
            SetUpDownLbls();
        }


        private void ConstructorPart()
        {
            tp = type.pripusk;
            NotAlarm();                 // задание цвета линий

            lblPripusk = new Label();
            lblPripusk.FontSize = 13;
            lblPripusk.Margin = new Thickness(1);
            lblPripusk.Padding = new Thickness(1);
            lblPripusk.BorderThickness = new Thickness(1);
            lblPripusk.BorderBrush = Brushes.Green;
            lblPripuskClick = new MouseButtonEventHandler(lblPripusk_MouseLeftButtonDown);
            canv.Children.Add(lblPripusk);
            lblPripusk.Content = min.ToString() + "..." + max.ToString();
        }
 
//---------------------------------------------------------------------------------------------------------------------------------------------------------

            // Расположение всех остальных лейблов помимо номинала и пределов, которые содержит размер - виртуальный метод, переопределён.
        public override void SetOtherLabels()
        {
            Canvas.SetTop(lblPripusk, mainLine.Y1 + 3);
            Canvas.SetLeft(lblPripusk, (mainLine.X1 + mainLine.X2) / 2 - lblPripusk.ActualWidth / 2);
        }

            // подписка на событие нажатия ЛКМ по метке lblPripusk - выбор размера - виртуальный метод, переопределён.
        public override void lblClickedOtherPodpiska()
        {
            lblPripusk.MouseLeftButtonDown += lblPripuskClick;
        }

        protected void lblPripusk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dimensionClick();          // вызываем событие - выбран размер, метод из базового класса
            e.Handled = true;           // запретить дальнейшую обработку события, чтоб оно не попало на канвас
        }

            // отписка от всех событий при удалении размера
        public override void OtpiskaWhenDelate()
        {
            (UC as PripuskUserControl).ReNull();

            if(lblPripuskClick.GetInvocationList().Length != 0)
                lblPripusk.MouseLeftButtonDown -= lblPripuskClick;
        }

            // Удаление с канваса всех остальных лейблов помимо номинала и пределов, которые содержит размер - виртуальный метод, переопределён.
        public override void RemoveOtherLabels()
        {
            canv.Children.Remove(lblPripusk);
        }

        public override void NotAlarm()
        {
            lblNominal.Background = Brushes.Transparent;
            mainLine.Stroke = Brushes.Green;
            firstLine.Stroke = Brushes.Green;
            secondLine.Stroke = Brushes.Green;
            firstArrowUp.Stroke = Brushes.Green;
            firstArrowDown.Stroke = Brushes.Green;
            secondArrowUp.Stroke = Brushes.Green;
            secondArrowDown.Stroke = Brushes.Green;
        }
        public void PUC_Apdated()
        {
            if (PripuskApdatedEvent != null)
                PripuskApdatedEvent(this);
        }
    }
}
