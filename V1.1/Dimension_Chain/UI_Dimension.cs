using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dimension_Chain
{
    public partial class UI_Dimension
    {
        //int n; //test
        public delegate void DimensionCreatedEventHandler(UI_Dimension dim);
        public static event DimensionCreatedEventHandler DimensionCreated;      // событие при завершении создания размера

        public delegate void dimensionClickedEventHandler(UI_Dimension dim);
        public event dimensionClickedEventHandler dimensionClicked;           // событие при нажатии на размер - выбор размера

            // делегаты 
        MouseButtonEventHandler MouseLeftButtonDownFirstEllipse;
        MouseEventHandler MouseMoveSecondEllipse;
        MouseButtonEventHandler MouseLeftButtonDownSecondEllipse;
        MouseEventHandler MouseMoveMainLine;
        MouseButtonEventHandler MouseLeftButtonDownMainLine;

                                                                      // передаётся контроллером, на этапе его конструирования :
        public static Canvas canv;                                    // канвас на котором располагаются ВСЕ размеры
        public static StackPanel stackPanel;                          // панель, на которую будет выводится юзерконтрол с инфой о размере
        public static List<UI_Dimension> listOfDimensions;            // список всех размеров, к которым будем искать привязки
        public static UI_Dimension temporaryUI;                       // временный размер, который создаётся в данный момент

        static int radius = 5;
        static double minDistToSnap = radius;
        public UserControl UC;                          // юзерконтрол, с которого будут считываться размеры
        public type tp;                                 // тип размера - технологический, конструкторский или припуск
        public double nominal;
        public double up;
        public double down;
        public Line mainLine;
        protected Line firstLine;
        protected Line secondLine;
        protected Line firstArrowUp;
        protected Line firstArrowDown;
        protected Line secondArrowUp;
        protected Line secondArrowDown;
        protected Ellipse firstEllipse;
        protected Ellipse secondEllipse;
        public Label lblNominal;
        public Label lblUp;
        public Label lblDown;
        public double firstElX;
        public double firstElY;
        public double secondElX;
        public double secondElY;
        private static int v = 5;                  // выступание боковой размерной линии за основную на 5 пикселей вверх или вниз
        private double thickness = 1.5;              // толщина размерной линии 
            // нужно при создании
        bool waitSecondEllipse = true;  
        bool waitMainLine = true;

        protected UI_Dimension(UserControl UC)
        {
            //this.n = Controller.n;              // тест
            MouseLeftButtonDownFirstEllipse = new MouseButtonEventHandler(canv_MouseLeftButtonDownFirstEllipse);
            MouseMoveSecondEllipse = new MouseEventHandler(canv_MouseMoveSecondEllipse);
            MouseLeftButtonDownSecondEllipse = new MouseButtonEventHandler(canv_MouseLeftButtonDownSecondEllipse);
            MouseMoveMainLine = new MouseEventHandler(canv_MouseMoveMainLine);
            MouseLeftButtonDownMainLine = new MouseButtonEventHandler(canv_MouseLeftButtonDownMainLine);

            this.UC = UC;
            stackPanel.Children.Add(UC);

            CreateBasicFields();
            canv.MouseLeftButtonDown += MouseLeftButtonDownFirstEllipse;
        }

        protected void CreateBasicFields()
        {
            mainLine = new Line();
            mainLine.StrokeThickness = thickness;
            mainLine.StrokeStartLineCap = PenLineCap.Triangle;
            mainLine.StrokeEndLineCap = PenLineCap.Triangle;
            firstArrowUp = new Line();
            firstArrowDown = new Line();
            secondArrowUp = new Line();
            secondArrowDown = new Line();
            firstArrowUp.StrokeThickness = thickness;
            firstArrowDown.StrokeThickness = thickness;
            secondArrowUp.StrokeThickness = thickness;
            secondArrowUp.StrokeThickness = thickness;

            lblNominal = new Label();
            lblUp = new Label();
            lblDown = new Label();
            lblNominal.FontSize = 19;
            lblNominal.Margin = new Thickness(1);
            lblNominal.Padding = new Thickness(1);
            lblUp.FontSize = 13;
            lblUp.Margin = new Thickness(1);
            lblUp.Padding = new Thickness(1);
            lblDown.FontSize = 13;
            lblDown.Margin = new Thickness(1);
            lblDown.Padding = new Thickness(1);

            firstEllipse = new Ellipse();
            firstEllipse.Height = radius * 2;
            firstEllipse.Width = radius * 2;
            firstEllipse.Stroke = Brushes.Black;

            secondEllipse = new Ellipse();
            secondEllipse.Height = radius * 2;
            secondEllipse.Width = radius * 2;
            secondEllipse.Stroke = Brushes.Black;

            firstLine = new Line();
            firstLine.StrokeThickness = thickness;
            secondLine = new Line();
            secondLine.StrokeThickness = thickness;
        }

                // создаём первую точку размера в момент первого нажатия ЛКМ по канвасу
        protected void canv_MouseLeftButtonDownFirstEllipse(object sender, MouseButtonEventArgs e)
        {
            //MainWindow.testTB.AppendText(Controller.n.ToString()+".1 ");
            firstElX = e.MouseDevice.GetPosition(canv).X;
            firstElY = e.MouseDevice.GetPosition(canv).Y;
                // проверка привязки к существующим размерам
            foreach (UI_Dimension d in listOfDimensions)
            {
                if (Distance(firstElX, firstElY, d.firstElX, d.firstElY) < minDistToSnap)
                {
                    firstElX = d.firstElX;
                    firstElY = d.firstElY;
                    break;
                }
                if (Distance(firstElX, firstElY, d.secondElX, d.secondElY) < minDistToSnap)
                {
                    firstElX = d.secondElX;
                    firstElY = d.secondElY;
                    break;
                }
            }
            Canvas.SetTop(firstEllipse, firstElY - radius);
            Canvas.SetLeft(firstEllipse, firstElX - radius);
            canv.Children.Add(firstEllipse);
            canv.Children.Add(secondEllipse);
            canv.Children.Add(mainLine);
            canv.Children.Add(firstLine);
            canv.Children.Add(secondLine);
            canv.Children.Add(firstArrowUp);
            canv.Children.Add(firstArrowDown);
            canv.Children.Add(secondArrowUp);
            canv.Children.Add(secondArrowDown);
            
            canv.Children.Add(lblNominal);
            canv.Children.Add(lblUp);
            canv.Children.Add(lblDown);

            temporaryUI = this;

            canv.MouseMove += MouseMoveSecondEllipse;
            canv.MouseLeftButtonDown += MouseLeftButtonDownSecondEllipse;
            canv.MouseLeftButtonDown -= MouseLeftButtonDownFirstEllipse;
        }
                // ожидаем вторую точку размера , процесс перетаскивания по канвасу
        private void canv_MouseMoveSecondEllipse(object sender, MouseEventArgs e)
        {
            //MainWindow.testTB.AppendText(Controller.n.ToString() + ".2 ");
            if (waitSecondEllipse)
            {
                secondElX = e.MouseDevice.GetPosition(canv).X;
                secondElY = e.MouseDevice.GetPosition(canv).Y;

                Canvas.SetTop(secondEllipse, secondElY - radius);
                Canvas.SetLeft(secondEllipse, secondElX - radius);

                //Canvas.SetTop(lblNominal, secondElY - lblNominal.ActualHeight + 3);
                //Canvas.SetLeft(lblNominal, (firstElX + secondElX) / 2 + lblNominal.ActualWidth/2);
                SetUpDownLbls();
                firstLine.X1 = firstElX;
                firstLine.Y1 = firstElY;
                firstLine.X2 = firstElX;
                firstLine.Y2 = secondElY;

                secondLine.X1 = secondElX;
                secondLine.Y1 = secondElY;
                secondLine.X2 = secondElX;
                secondLine.Y2 = secondElY;

                mainLine.X1 = firstElX;
                mainLine.Y1 = secondElY;
                mainLine.X2 = secondElX;
                mainLine.Y2 = secondElY;
                    // простановка стрелок
                ArrowsAdjustment();
            }
            else
            {
                canv.MouseMove += MouseMoveMainLine;
                canv.MouseLeftButtonDown += MouseLeftButtonDownMainLine;
                canv.MouseLeftButtonDown -= MouseLeftButtonDownSecondEllipse;
                canv.MouseMove -= MouseMoveSecondEllipse;
            }
        }

                // завершение создания второй точки размера - нажатие ЛКМ
        private void canv_MouseLeftButtonDownSecondEllipse(object sender, MouseButtonEventArgs e)
        {
            //MainWindow.testTB.AppendText(Controller.n.ToString() + ".3 ");
            waitSecondEllipse = false;
            secondElX = e.MouseDevice.GetPosition(canv).X;
            secondElY = e.MouseDevice.GetPosition(canv).Y;
                // проверка привязки к существующим размерам
            foreach (UI_Dimension d in listOfDimensions)
            {
                if (Distance(secondElX, secondElY, d.firstElX, d.firstElY) < minDistToSnap)
                {
                    secondElX = d.firstElX;
                    secondElY = d.firstElY;
                    break;
                }
                if (Distance(secondElX, secondElY, d.secondElX, d.secondElY) < minDistToSnap)
                {
                    secondElX = d.secondElX;
                    secondElY = d.secondElY;
                    break;
                }
            }
            Canvas.SetTop(secondEllipse, secondElY - radius);
            Canvas.SetLeft(secondEllipse, secondElX - radius);
            if (firstElX > secondElX)       // делаем первую точку размера слева от второй
                Swap();
            firstLine.X1 = firstElX;
            firstLine.Y1 = firstElY;
            firstLine.X2 = firstElX;
            firstLine.Y2 = secondElY;

            secondLine.X1 = secondElX;
            secondLine.Y1 = secondElY;
            secondLine.X2 = secondElX;
            secondLine.Y2 = secondElY;

            mainLine.X1 = firstElX;
            mainLine.Y1 = secondElY;
            mainLine.X2 = secondElX;
            mainLine.Y2 = secondElY;
            ArrowsAdjustment();
            SetUpDownLbls();
        }
                // размещение размерной линии - процесс перетаскивания по канвасу
        private void canv_MouseMoveMainLine(object sender, MouseEventArgs e)
        {
            //MainWindow.testTB.AppendText(Controller.n.ToString() + ".4 ");
            if (waitMainLine)
            {
                mainLine.X1 = firstElX + 1;
                mainLine.Y1 = e.MouseDevice.GetPosition(canv).Y;
                mainLine.X2 = secondElX - 1;
                mainLine.Y2 = mainLine.Y1;
                SideLinesAdjustment();
                SetUpDownLbls();
                ArrowsAdjustment();
            }
            else
            {
                canv.MouseMove           -= MouseMoveMainLine;
                canv.MouseLeftButtonDown -= MouseLeftButtonDownMainLine;
            }
        }

                // размещение размерной линии - окончательный выбор точки размещения - нажатие ЛКМ
        private void canv_MouseLeftButtonDownMainLine(object sender, MouseButtonEventArgs e)
        {
            ArrowsAdjustment();
            waitMainLine = false;
            firstEllipse.Fill = Brushes.Black;
            secondEllipse.Fill = Brushes.Black;

            firstEllipse.MouseMove += new MouseEventHandler(Ellipse_MouseMove);
            secondEllipse.MouseMove += new MouseEventHandler(Ellipse_MouseMove);
            SetUpDownLbls();
            temporaryUI = null;
            lblNominal.MouseLeftButtonDown += new MouseButtonEventHandler(lblNominal_MouseLeftButtonDown);      // подписываемся на событие нажатия ЛКМ по метке с номиналом - выбор размера
            lblClickedOtherPodpiska();                          // подписка на событие нажатия ЛКМ по остальным лейблам в наследованых классах
            Unchoose();                                         // отменяем выбор размера
            DimensionCreated(this);                             // вызываем событие: размер создан
        }

            // наведение курсора на эллипс - привязка
        protected void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            NativeMethods.SetCursorPos((int)ellipse.PointToScreen(new Point(0, 0)).X + radius, (int)ellipse.PointToScreen(new Point(0, 0)).Y + radius);
        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }
                // меняем местами стороны
        private void Swap()
        {
            Ellipse e = firstEllipse;
            firstEllipse = secondEllipse;
            secondEllipse = e;
            double dx = firstElX;
            double dy = firstElY;
            firstElX = secondElX;
            firstElY = secondElY;
            secondElX = dx;
            secondElY = dy;
        }


                 // простановка стрелок
        private void ArrowsAdjustment()
        {
            // первая стрелка
            firstArrowUp.X1 = mainLine.X1;
            firstArrowUp.Y1 = mainLine.Y1;
            firstArrowUp.Y2 = mainLine.Y1 - 4;

            firstArrowDown.X1 = mainLine.X1;
            firstArrowDown.Y1 = mainLine.Y1;
            firstArrowDown.Y2 = mainLine.Y1 + 4;

            if (mainLine.X1 < mainLine.X2)
            {
                firstArrowUp.X2 = mainLine.X1 + 10;
                firstArrowDown.X2 = mainLine.X1 + 10;
            }
            else
            {
                firstArrowUp.X2 = mainLine.X1 - 10;
                firstArrowDown.X2 = mainLine.X1 - 10;
            }
                // вторая стрелка
            secondArrowUp.X1 = mainLine.X2;
            secondArrowUp.Y1 = mainLine.Y2;
            secondArrowUp.Y2 = mainLine.Y2 - 4;

            secondArrowDown.X1 = mainLine.X2;
            secondArrowDown.Y1 = mainLine.Y2;
            secondArrowDown.Y2 = mainLine.Y2 + 4;

            if (mainLine.X1 > mainLine.X2)
            {
                secondArrowUp.X2 = mainLine.X2 + 10;
                secondArrowDown.X2 = mainLine.X2 + 10;
            }
            else
            {
                secondArrowUp.X2 = mainLine.X2 - 10;
                secondArrowDown.X2 = mainLine.X2 - 10;
            }
        }

        public void SetNominal(string s)
        {
            lblNominal.Content = s;
        }
        public void SetUp(string s)
        {
            lblUp.Content = s;
        }
        public void SetDown(string s)
        {
            lblDown.Content = s;
        }

            // нажатие ЛКМ по метке с номиналом - выбор размера
        protected void lblNominal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dimensionClick();           //см. ниже
            e.Handled = true;           // запретить дальнейшую обработку события, чтоб оно не попало на канвас
        }
        protected void dimensionClick()        // отдельный метод нужен, чтоб событие можно было вызвать из классов-наследников
        {
            dimensionClicked(this);    // вызываем событие - выбран размер
        }            

            // подписка на событие нажатия ЛКМ по остальным лейблам в наследованых классах
        public virtual void lblClickedOtherPodpiska()
        { }
                // выделение размера - толстыми линиями
        public void Pick()
        {
            mainLine.StrokeThickness = 3;
            firstLine.StrokeThickness = 3;
            secondLine.StrokeThickness = 3;
            firstArrowUp.StrokeThickness = 3;
            firstArrowDown.StrokeThickness = 3;
            secondArrowUp.StrokeThickness = 3;
            secondArrowDown.StrokeThickness = 3;
        }
                // отмена выделения размера
        public void Unpick()
        {
            mainLine.StrokeThickness = thickness;
            firstLine.StrokeThickness = thickness;
            secondLine.StrokeThickness = thickness;
            firstArrowUp.StrokeThickness = thickness;
            firstArrowDown.StrokeThickness = thickness;
            secondArrowUp.StrokeThickness = thickness;
            secondArrowDown.StrokeThickness = thickness;
        }
            // подсветка размера
        public void Alarm()
        {
            lblNominal.Background = Brushes.Red;
            /*mainLine.Stroke = Brushes.Red;
            firstLine.Stroke = Brushes.Red;
            secondLine.Stroke = Brushes.Red;
            firstArrowUp.Stroke = Brushes.Red;
            firstArrowDown.Stroke = Brushes.Red;
            secondArrowUp.Stroke = Brushes.Red;
            secondArrowDown.Stroke = Brushes.Red;*/
        }

        public virtual void NotAlarm()      // виртуальный
        { }

            // удаление размера с канваса
        public void RemoveFromCanv()
        {
            canv.Children.Remove(firstEllipse);
            canv.Children.Remove(secondEllipse);
            canv.Children.Remove(mainLine);
            canv.Children.Remove(firstLine);
            canv.Children.Remove(secondLine);
            canv.Children.Remove(firstArrowUp);
            canv.Children.Remove(firstArrowDown);
            canv.Children.Remove(secondArrowUp);
            canv.Children.Remove(secondArrowDown);

            canv.Children.Remove(lblNominal);
            canv.Children.Remove(lblUp);
            canv.Children.Remove(lblDown);
            RemoveOtherLabels();          // VIRTUAL
            canv.MouseLeftButtonDown -= canv_MouseLeftButtonDownFirstEllipse;
            lblNominal.MouseLeftButtonDown -= lblNominal_MouseLeftButtonDown;
            OtpiskaWhenDelate();          // VIRTUAL
            UC = null;
        }

            // Удаление с канваса всех остальных лейблов помимо номинала и пределов, которые содержит размер - виртуальный метод.
        public virtual void RemoveOtherLabels()                  
        { }

            // отписка от всех событий при отмене создании размера (например, нажата ESC)
        public void OtpiskaWhenCreate()
        {
            if (MouseLeftButtonDownFirstEllipse.GetInvocationList().Length != 0)
                canv.MouseLeftButtonDown -= MouseLeftButtonDownFirstEllipse;
            if (MouseMoveSecondEllipse.GetInvocationList().Length != 0)
                canv.MouseMove -= MouseMoveSecondEllipse;
            if (MouseLeftButtonDownSecondEllipse.GetInvocationList().Length != 0)
                canv.MouseLeftButtonDown -= MouseLeftButtonDownSecondEllipse;
            if (MouseMoveMainLine.GetInvocationList().Length != 0)
                canv.MouseMove -= MouseMoveMainLine;
            if (MouseLeftButtonDownMainLine.GetInvocationList().Length != 0)
                canv.MouseLeftButtonDown -= MouseLeftButtonDownMainLine;
        }
            // отписка от всех событий при удалении размера - виртуальный
        public virtual void OtpiskaWhenDelate() 
        { }

            // Расположение всех остальных лейблов помимо номинала и пределов, которые содержит размер - виртуальный метод.
        public virtual void SetOtherLabels()
        { }

            // отмена выбора размера - удаление юзерконтрола, к нему привязанного
        public virtual void Unchoose()
        { }

        // выступание боковой размерной линии за основную на 3 пикселя вверх или вниз
        private void SideLinesAdjustment()
        {
            if (firstLine.Y1 < mainLine.Y2)
                firstLine.Y2 = mainLine.Y2 + v;
            else
                firstLine.Y2 = mainLine.Y2 - v;

            if (secondLine.Y1 < mainLine.Y2)
                secondLine.Y2 = mainLine.Y2 + v;
            else
                secondLine.Y2 = mainLine.Y2 - v;
        }

            // установка  меток верхнего и нижнего отклонений
        public void SetUpDownLbls()
        {
            Canvas.SetTop(lblNominal, mainLine.Y1 - lblNominal.ActualHeight - 3);
            Canvas.SetLeft(lblNominal, (mainLine.X1 + mainLine.X2) / 2 - lblNominal.ActualWidth / 2);
            Canvas.SetTop(lblUp, Canvas.GetTop(lblNominal) - lblUp.ActualHeight / 4 + 2);
            Canvas.SetLeft(lblUp, Canvas.GetLeft(lblNominal) + lblNominal.ActualWidth);
            Canvas.SetTop(lblDown, Canvas.GetTop(lblNominal) + lblNominal.ActualHeight / 4 + 3);
            Canvas.SetLeft(lblDown, Canvas.GetLeft(lblUp));
            SetOtherLabels();
        }

            // приводим всё к эллипсам и размерной линии
        public void RebuildAfterScale()
        {
            Canvas.SetTop(firstEllipse, firstElY - radius);
            Canvas.SetLeft(firstEllipse, firstElX - radius);
            Canvas.SetTop(secondEllipse, secondElY - radius);
            Canvas.SetLeft(secondEllipse, secondElX - radius);
            firstLine.X1 = firstElX;
            firstLine.X2 = firstElX;
            firstLine.Y1 = firstElY;
            secondLine.X1 = secondElX;
            secondLine.X2 = secondElX;
            secondLine.Y1 = secondElY;
            SideLinesAdjustment();
            ArrowsAdjustment();
            SetUpDownLbls();
        }
    }

    


        // класс для установки курсора мыши в нужное положение
    public partial class NativeMethods
    {
        /// Return Type: BOOL->int  
        ///X: int  
        ///Y: int  
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);
    }
}