using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Dimension_Chain
{
    [Serializable]
    public partial class UI_ConstrDimension : UI_Dimension
    {
        public delegate void ConstrApdatedEventHandler(UI_Dimension dim);
        public event ConstrApdatedEventHandler ConstrApdatedEvent;            // событие при изменении конструкторского размера (желаемого пользователем максимума или минимума)

        public double max;     // желаемое максимальное значение размера, введённое пользователем
        public double min;     // желаемое  минимальное значение размера, введённое пользователем

        public Label lblNominalConstr;          // з
        public Label lblUpConstr;
        public Label lblDownConstr;
        Rectangle rectangle;

        MouseButtonEventHandler rectangleClick;               // делегаты

        public UI_ConstrDimension(ConstructorUserControl CUC) : base(CUC)
        {
                // идёт основной конструктор базового класса..................................
            ConstructorPart();
            CUC.SetUI_CD(this);
            lblNominal.Content = "?";
            lblUp.Content = "?";
            lblDown.Content = "?";
            lblNominal.UpdateLayout();  // для обновления lblNominal
            SetUpDownLbls();
        }

        private void ConstructorPart()
        {
            this.tp = type.konstr;
            NotAlarm();                 // задание цвета линий
                //??
            lblNominal.Content = "?";
            lblUp.Content = "?";
            lblDown.Content = "?";
            rectangleClick = new MouseButtonEventHandler(rectangleClick_MouseLeftButtonDown);

            lblNominalConstr =  new Label();
            lblUpConstr = new Label();
            lblDownConstr = new Label();
            rectangle = new Rectangle();
            lblNominalConstr.FontSize = 15;
            lblNominalConstr.Margin = new Thickness(1);
            lblNominalConstr.Padding = new Thickness(1);
            lblUpConstr.FontSize = 11;
            lblUpConstr.Margin = new Thickness(1);
            lblUpConstr.Padding = new Thickness(1);
            lblDownConstr.FontSize = 11;
            lblDownConstr.Margin = new Thickness(1);
            lblDownConstr.Padding = new Thickness(1);
            rectangle.Stroke = Brushes.Blue;
            rectangle.StrokeThickness = 1;
            rectangle.Fill = Brushes.Transparent;
            lblNominalConstr.Content =  nominal.ToString();
            lblUpConstr.Content = up.ToString();
            lblDownConstr.Content = down.ToString();

            canv.Children.Add(lblNominalConstr);
            canv.Children.Add(lblUpConstr);
            canv.Children.Add(lblDownConstr);
            canv.Children.Add(rectangle);
        }

             // Расположение всех остальных лейблов помимо номинала и пределов, которые содержит размер - виртуальный метод, переопределён.
        public override void SetOtherLabels()
        {
            Canvas.SetTop(lblNominalConstr, mainLine.Y1 +2);
            Canvas.SetLeft(lblNominalConstr, (mainLine.X1 + mainLine.X2) / 2 - lblNominalConstr.ActualWidth / 2);
            Canvas.SetTop(lblUpConstr, Canvas.GetTop(lblNominalConstr) - lblUpConstr.ActualHeight / 4 + 2);
            Canvas.SetLeft(lblUpConstr, Canvas.GetLeft(lblNominalConstr) + lblNominalConstr.ActualWidth);
            Canvas.SetTop(lblDownConstr, Canvas.GetTop(lblNominalConstr) + lblNominalConstr.ActualHeight / 4 + 4);
            Canvas.SetLeft(lblDownConstr, Canvas.GetLeft(lblUpConstr));
            Canvas.SetTop(rectangle, mainLine.Y1 + 2);
            Canvas.SetLeft(rectangle, Canvas.GetLeft(lblNominalConstr) - 1);
            rectangle.Height = lblNominalConstr.ActualHeight + 5;
            rectangle.Width = lblNominalConstr.ActualWidth + Math.Max(lblUpConstr.ActualWidth, lblDownConstr.ActualWidth) + 4;
        }

            // подписка на событие нажатия ЛКМ по метке lblNominalConstr - выбор размера - виртуальный метод, переопределён.
        public override void lblClickedOtherPodpiska()
        {
            //lblNominalConstr.MouseLeftButtonDown += lblNominalConstrBut;
            rectangle.MouseLeftButtonDown += rectangleClick;
        }

        void rectangleClick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dimensionClick();           // вызываем событие - выбран размер, метод из базового класса
            e.Handled = true;           // запретить дальнейшую обработку события, чтоб оно не попало на канвас
        }

//-------------------------------------------------------------------------------------------------------------------------------------------------
        public override void OtpiskaWhenDelate()
        {
            (UC as ConstructorUserControl).ReNull();

            if (rectangleClick.GetInvocationList().Length != 0)
                lblNominalConstr.MouseLeftButtonDown -= rectangleClick;
        }

        // Удаление с канваса всех остальных лейблов помимо номинала и пределов, которые содержит размер - виртуальный метод, переопределён.
        public override void RemoveOtherLabels()
        {
            canv.Children.Remove(lblNominalConstr);
            canv.Children.Remove(lblUpConstr);
            canv.Children.Remove(lblDownConstr);
            canv.Children.Remove(rectangle);
        }

        public override void NotAlarm()
        {
            lblNominal.Background = Brushes.Transparent;
            mainLine.Stroke = Brushes.Blue;
            firstLine.Stroke = Brushes.Blue;
            secondLine.Stroke = Brushes.Blue;
            firstArrowUp.Stroke = Brushes.Blue;
            firstArrowDown.Stroke = Brushes.Blue;
            secondArrowUp.Stroke = Brushes.Blue;
            secondArrowDown.Stroke = Brushes.Blue;
        }

        public void CUC_Apdated()
        {
            if (ConstrApdatedEvent != null)
                ConstrApdatedEvent(this);
        }
    }
}
