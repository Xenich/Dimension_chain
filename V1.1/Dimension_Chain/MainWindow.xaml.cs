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
using System.IO;
using System.Windows.Markup;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dimension_Chain
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml   - VIEW
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TextBox testTB ;
        bool isDown;                                                // нажата ли средняя кнопка мыши - перетаскивание канваса
        Point startPoint;
        public delegate void DelPressedEventHandler();
        public static event DelPressedEventHandler DelPressed;      // событие нажатия кнопки Del
        public delegate void EscPressedEventHandler();
        public static event EscPressedEventHandler EscPressed;      // событие нажатия кнопки Esc

        public delegate void WheelEventHandler(Point p);
        public static event WheelEventHandler WheelUp;             // событие прокрутки колёсика вверх - масштабирование                              
        public static event WheelEventHandler WheelDown;           // событие прокрутки колёсика вниз - масштабирование

        public delegate void MouseLeftButtonDownOnCanvasEventHandler();
        public static event MouseLeftButtonDownOnCanvasEventHandler MouseLeftButtonDownOnCanvas;      // событие нажатия на канвас ЛКМ
        public delegate void SaveEventHandler();
        public static event SaveEventHandler SaveAllAs;         // событие сохранения c выбором файла
        public static event SaveEventHandler SaveAll;           // событие сохранения в имеющийся файл
        public delegate void OpenEventHandler();
        public static event OpenEventHandler OpenSaved;         // событие открытия сохранения
        public delegate void NewProjectEventHandler();
        public static event NewProjectEventHandler NewProj;     // событие создания нового проекта
   

        Controller controller;

        public MainWindow()
        {
            InitializeComponent();
            testTB = textBoxTest;
            controller = new Controller(MainCanvas, leftStackPanel, rightStackPanel, this);
        }

            // создание технологического размера
        private void buttonTech_Click(object sender, RoutedEventArgs e)
        {
            controller.NewTechDimension();
        }
            // создание припуска
        private void buttonPripusk_Click(object sender, RoutedEventArgs e)
        {
            controller.NewPrirusk();
        }
            // создание конструкторского размера
        private void buttonKonstr_Click(object sender, RoutedEventArgs e)
        {
             controller.NewConstr();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Delete)
                DelPressed();

            if (e.Key == Key.Escape)
                EscPressed();

            if (e.Key == Key.C)         // установка канваса на центр
            {
                Canvas.SetTop(MainCanvas, -2500);
                Canvas.SetLeft(MainCanvas, -2500);
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonDownOnCanvas();
        }

        private void textBoxTest_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)(sender)).ScrollToEnd();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAllAs();
        }

        private void Save_Click_1(object sender, RoutedEventArgs e)
        {
            SaveAll();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenSaved();
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetTop(MainCanvas, -2500);
            Canvas.SetLeft(MainCanvas, -2500);
            NewProj();
        }
        
        public void SetSaveEnable(bool isEnabled)
        {
            Save.IsEnabled = isEnabled;   // менюайтем
        }

// -------------------------------------      ПРЕМЕЩЕНИЕ, масштабирование       ----------------------------------------------------------------------------------------------------------------------------------------------------------

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                isDown = true;             
                startPoint = ((UIElement)sender).PointToScreen(new Point(e.MouseDevice.GetPosition(MainCanvas).X, e.MouseDevice.GetPosition(MainCanvas).Y));
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                Point endPoint = ((UIElement)sender).PointToScreen(new Point(e.MouseDevice.GetPosition(MainCanvas).X, e.MouseDevice.GetPosition(MainCanvas).Y));
                double width = endPoint.X - startPoint.X;
                double height = endPoint.Y - startPoint.Y;

                Canvas.SetTop(MainCanvas, Canvas.GetTop(MainCanvas) + height);
                Canvas.SetLeft(MainCanvas,Canvas.GetLeft(MainCanvas) + width);
                startPoint = endPoint;
            }
            textBoxTest.Text = "x = " + Convert.ToInt16(e.MouseDevice.GetPosition(MainCanvas).X).ToString() + "\r\ny = " + Convert.ToInt16(e.MouseDevice.GetPosition(MainCanvas).Y).ToString();
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDown = false;
        }

        private void MainCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            isDown = false;
        }
                // масштабирование
        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                WheelUp(new Point(e.MouseDevice.GetPosition(MainCanvas).X, e.MouseDevice.GetPosition(MainCanvas).Y));
            else
                WheelDown(new Point(e.MouseDevice.GetPosition(MainCanvas).X, e.MouseDevice.GetPosition(MainCanvas).Y));
        }

        private void MenuItemHow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Конструкторский размер (синий) и припуск (зелёный) - замыкающие звенья, они рассчитываются программой исходя из цепочки технологических размеров (чёрный), которые нужно задавать. При добавлении замыкающего звена (припуска или конструкторского размера) необходимо указать пределы, за которые он не должен выходить.  Если после рассчёта замыкающее звено выходит за эти пределы, оно подсвечивается красным цветом. При недоопределении размерной цепи замыкающие звенья рассчитаны не будут, вместо размера будет стоять знак вопроса");
            
        }
//-------------------------------------------------- ПЕЧАТЬ ------------------------------------------------
        
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(MainCanvas, "Print...");
            }
        }



//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}
