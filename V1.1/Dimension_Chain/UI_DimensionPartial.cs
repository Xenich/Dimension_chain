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
using System.Windows.Shapes;

namespace Dimension_Chain
{
        // куски классов, отвечающие за создание раннее сохраненного размера

    public partial class UI_Dimension
    {
        public UI_Dimension(UI_Dimension_Save saved)
        {
            CreateBasicFields();
            firstElX = saved.firstElX;
            firstElY = saved.firstElY;

            secondElX = saved.secondElX;
            secondElY = saved.secondElY;
            
            mainLine.X1 = firstElX;
            mainLine.X2 = secondElX;
            mainLine.Y1 = saved.mainLineY;
            mainLine.Y2 = saved.mainLineY;

            firstLine.X1 = firstElX;
            firstLine.X2 = firstElX;
            firstLine.Y1 = firstElY;
            if (firstLine.Y1 < mainLine.Y2)                     // выступание боковой размерной линии за основную на v пикселя вверх или вниз            
                firstLine.Y2 = mainLine.Y2 + v;
            else
                firstLine.Y2 = mainLine.Y2 - v;

            secondLine.X1 = secondElX;
            secondLine.Y1 = secondElY;
            secondLine.X2 = secondElX;
            if (secondLine.Y1 < mainLine.Y2)                     // выступание боковой размерной линии за основную на v пикселя вверх или вниз      
                secondLine.Y2 = mainLine.Y2 + v;
            else
                secondLine.Y2 = mainLine.Y2 - v;

            ArrowsAdjustment();            // простановка стрелок
            
            Canvas.SetTop(firstEllipse, firstElY - radius);
            Canvas.SetLeft(firstEllipse, firstElX - radius);
            Canvas.SetTop(secondEllipse, secondElY - radius);
            Canvas.SetLeft(secondEllipse, secondElX - radius);
            
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
           
            firstEllipse.Fill = Brushes.Black;
            secondEllipse.Fill = Brushes.Black;

            firstEllipse.MouseMove          += new MouseEventHandler(Ellipse_MouseMove);
            secondEllipse.MouseMove         += new MouseEventHandler(Ellipse_MouseMove);
            lblNominal.MouseLeftButtonDown  += new MouseButtonEventHandler(lblNominal_MouseLeftButtonDown);      // подписываемся на событие нажатие ЛКМ по метке с номиналом - выбор размера
        }
    }

    public partial class UI_TechDimension 
    {
        public UI_TechDimension(UI_Dimension_Save saved) : base(saved)
        {
            ConstructorPart();
            nominal = (saved as UI_TechDimension_Save).nominal;
            up = (saved as UI_TechDimension_Save).up;
            down = (saved as UI_TechDimension_Save).down;
            lblNominal.Content = nominal;
            lblUp.Content = up;
            lblDown.Content = down;
            lblNominal.UpdateLayout();  // для обновления lblNominal
            SetUpDownLbls();
        }
    }

    partial class UI_PripuskDimension
    {
        public UI_PripuskDimension(UI_Dimension_Save saved) : base(saved)
        {
            max = (saved as UI_PripuskDimension_Save).max;
            min = (saved as UI_PripuskDimension_Save).min;
            ConstructorPart();

            lblNominal.Content = "?";
            lblUp.Content = "?";
            lblDown.Content = "?";

            lblNominal.UpdateLayout();  // для обновления lblNominal
            SetUpDownLbls();
        }
    }

    partial class UI_ConstrDimension
    {
        public UI_ConstrDimension(UI_Dimension_Save saved) : base(saved)
        {
            nominal = (saved as UI_ConstrDimension_Save).nominal;
            up = (saved as UI_ConstrDimension_Save).up;
            down = (saved as UI_ConstrDimension_Save).down;
            max = nominal + up;
            min = nominal + down;
            ConstructorPart();

            lblNominal.Content = "?";
            lblUp.Content = "?";
            lblDown.Content = "?";

            lblNominal.UpdateLayout();  // для обновления lblNominal
            SetUpDownLbls();
        }
    }
}
