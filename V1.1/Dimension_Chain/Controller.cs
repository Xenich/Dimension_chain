using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dimension_Chain
{
    interface IController
    {
        //void SetStateIsCreatingDimension(bool b);
        bool isCreateDimension();
    }
    [Serializable]
    class Controller : IController
    {
        //public static  int n; // test

        public Graph graph;                                             // Модель
        private static bool isCreatingDimension;                        // переменная показывает, происходит ли в данный момент создание размера
        public bool isCreateDimension() { return isCreatingDimension; }
        private static UI_Dimension creatingDimAtThisMoment;            // создаваемый в данный момент размер
        public List<UI_Dimension> listOfDimensions;                     // список всех UI размеров

        private Dictionary<Dimension, List<Dimension>> dicDim_L;        // словарь: размер (констр. или припуск -> список размеров, по которым он рассчитывается - размерная цепочка, в которой он является замыкающим звеном)
                                                                        //                 (техн. размер -> список констр. размеров или припусков, на которые он влияет). Будет использоваться для подсвечивания размеров при нажатиии на размерную линию
        private Dictionary<UI_Dimension, Dimension> dicUI_Dim;          // словарь: визуальный размер -> размер
        private Dictionary<Dimension, UI_Dimension> dicDim_UI;          // словарь: размер -> визуальный размер

        private static Canvas canv;                                     // канвас, на котором будет отображаться графика  
        public static TechUserControl TUC = new TechUserControl();                  // юзерконтролы с инфой о выбраном или создаваемом размере
        public static PripuskUserControl PUC = new PripuskUserControl();
        public static ConstructorUserControl CUC = new ConstructorUserControl();

        private StackPanel leftStackPanel;                              // StackPanel, на которой располагаются кнопки приложения
        private StackPanel rightStackPanel;                             // StackPanel, на которой располагаются инфо о размерной линии - TUC, PUC, CUC
        private MainWindow window;                                      // главное окно
        private UI_Dimension chosedDimension;                           // выбраный сейчас размер
        private List<UI_Dimension> pickedDimensionList;                 // список подсвеченых сейчас размеров
        private string fileName;                                        // имя файла с сохраненным проектом
        private double M = 1.1;                                         // масштабный коефициент - изменение масштаба за один поворот колёсика мыши
        //ScaleTransform scale;

        public Controller(Canvas canvas, StackPanel leftStackPanel, StackPanel rightStackPanel, MainWindow window)
        {
            canv = canvas;
            this.leftStackPanel = leftStackPanel;
            this.rightStackPanel = rightStackPanel;
            this.window = window;
            //scale = new ScaleTransform(M, M);
            
            listOfDimensions = new List<UI_Dimension>();
            dicUI_Dim = new Dictionary<UI_Dimension, Dimension>();
            dicDim_UI = new Dictionary<Dimension, UI_Dimension>();
            dicDim_L = new Dictionary<Dimension, List<Dimension>>();
            pickedDimensionList = new List<UI_Dimension>();
            graph = new Graph();
            
            UI_Dimension.listOfDimensions = listOfDimensions;
            UI_Dimension.canv = canvas;
            UI_Dimension.stackPanel = rightStackPanel;

                // подписки
            UI_Dimension.DimensionCreated           += new UI_Dimension.DimensionCreatedEventHandler(newDim_DimensionCreated);    // подписка на событие создания размера
            MainWindow.DelPressed                   += new MainWindow.DelPressedEventHandler(MainWindow_DelPressed);
            MainWindow.EscPressed                   += new MainWindow.EscPressedEventHandler(MainWindow_EscPressed);
            MainWindow.MouseLeftButtonDownOnCanvas  += new MainWindow.MouseLeftButtonDownOnCanvasEventHandler(MainWindow_MouseLeftButtonDown);
            MainWindow.SaveAllAs                    += new MainWindow.SaveEventHandler(MainWindow_SaveAllAs);
            MainWindow.SaveAll                      += new MainWindow.SaveEventHandler(MainWindow_SaveAll);
            MainWindow.OpenSaved                    += new MainWindow.OpenEventHandler(MainWindow_OpenSaved);
            MainWindow.NewProj                      += new MainWindow.NewProjectEventHandler(MainWindow_NewProj);
            MainWindow.WheelUp                      += new MainWindow.WheelEventHandler(MainWindow_WheelUp);
            MainWindow.WheelDown                    += new MainWindow.WheelEventHandler(MainWindow_WheelDown);


        }

//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            /// <summary>
        /// Создание технологического размера
        /// </summary>
        public void NewTechDimension()
        {
            if (!isCreatingDimension)
            {
                foreach (UIElement UI in leftStackPanel.Children)
                {
                    UI.IsEnabled = false;
                }
                isCreatingDimension = true;
                UnPickIfChosed();
                UI_Dimension newDim = new UI_TechDimension(TUC);
                creatingDimAtThisMoment = newDim;
                (newDim as UI_TechDimension).TechDimensionApdatedEvent += new UI_TechDimension.TechDimensionApdatedEventHandler(Controller_TechDimensionApdated);     // подписываемся на событие изменения нового размера
            }
        }

        /// <summary>
        /// Создание припуска
        /// </summary>
        public void NewPrirusk()
        {
            if (!isCreatingDimension)
            {
                foreach (UIElement UI in leftStackPanel.Children)
                {
                    UI.IsEnabled = false;
                }
                isCreatingDimension = true;
                UnPickIfChosed();
                UI_Dimension newDim = new UI_PripuskDimension(PUC);
                creatingDimAtThisMoment = newDim;
                (newDim as UI_PripuskDimension).PripuskApdatedEvent += new UI_PripuskDimension.PripuskApdatedEventHandler(Controller_PripuskApdated);     // подписываемся на событие изменения нового размера
            }
         }

        /// <summary>
        /// Создание конструкторского размера
        /// </summary>
        public void NewConstr()
        {
            if (!isCreatingDimension)
            {
                foreach (UIElement UI in leftStackPanel.Children)
                {
                    UI.IsEnabled = false;
                }
                isCreatingDimension = true;
                UnPickIfChosed();
                UI_Dimension newDim = new UI_ConstrDimension(CUC);
                creatingDimAtThisMoment = newDim;
                (newDim as UI_ConstrDimension).ConstrApdatedEvent += new UI_ConstrDimension.ConstrApdatedEventHandler(Controller_ConstrApdated);     // подписываем новый размер на событие изменения 
            }
        }

                
        /// <summary>
        /// обработчик события при завершении создании размера
        /// </summary>
        /// <param name="dim">созданный размер</param>
        private void newDim_DimensionCreated(UI_Dimension dim)
        {
            rightStackPanel.Children.Clear();    // удаление юзерконтрола, привязанного к размеру с правой панели
            creatingDimAtThisMoment = null;      // в настоящий момент ничего не создаётся
            
            dim.dimensionClicked += new UI_Dimension.dimensionClickedEventHandler(Controller_ClickOnDimension);         // подписываемся на событие клика по размеру
            
            Vertex v1 = graph.CreateVertex();
            Vertex v2 = graph.CreateVertex();
                        // смотрим, совпадают ли концы размерной линии с имеющимися размерами, если да - создаём нулевую связку размеров
            foreach (UI_Dimension UI_Dim in listOfDimensions)
            {
                if (dim.firstElX == UI_Dim.firstElX)
                    graph.CreateNul(v1, dicUI_Dim[UI_Dim].firstVertex);
                if (dim.firstElX == UI_Dim.secondElX)
                    graph.CreateNul(v1, dicUI_Dim[UI_Dim].secondVertex);
                if (dim.secondElX == UI_Dim.firstElX)
                    graph.CreateNul(v2, dicUI_Dim[UI_Dim].firstVertex);
                if (dim.secondElX == UI_Dim.secondElX)
                    graph.CreateNul(v2, dicUI_Dim[UI_Dim].secondVertex);
            }
            /*Vertex v1 = null;
            Vertex v2 = null;
                        // смотрим, совпадают ли концы размерной линии с имеющимися размерами, если да - создаём нулевую связку размеров
            if (listOfDimensions.Count == 0)
            {
                v1 = graph.CreateVertex();
                v2 = graph.CreateVertex();
            }
            else
            {
                foreach (UI_Dimension UI_Dim in listOfDimensions)
                {
                    if (dim.firstElX == UI_Dim.firstElX)
                        v1 = dicUI_Dim[UI_Dim].firstVertex;
                    else
                    {
                        if (dim.firstElX == UI_Dim.secondElX)
                            v1 = dicUI_Dim[UI_Dim].secondVertex;
                        else
                            v1 = graph.CreateVertex();
                    }
                    if (dim.secondElX == UI_Dim.firstElX)
                        v2 = dicUI_Dim[UI_Dim].firstVertex;
                    else
                    {
                        if (dim.secondElX == UI_Dim.secondElX)
                            v2 = dicUI_Dim[UI_Dim].secondVertex;
                        else
                            v2 = graph.CreateVertex();
                    }
                }
            }*/
                // создаём сам размер в модели и список его размерной цепи, если это конструкторский р-р или припуск
            Dimension dimension = null;
            switch(dim.tp)
            {
                case type.tech:
                    dimension = graph.CreateTechDimension(v1, v2, new Value(dim.nominal, dim.up, dim.down));
                    break;
                case type.pripusk:
                    dimension = graph.CreateClosingLink(v1, v2, type.pripusk);
                    break;
                case type.konstr:
                    dimension = graph.CreateClosingLink(v1, v2, type.konstr);
                    break;

            }
            dicDim_L.Add(dimension, new List<Dimension>());
            listOfDimensions.Add(dim);
            dicUI_Dim.Add(dim, dimension);
            dicDim_UI.Add(dimension, dim);   

            ReBuildAll();             // пересчитываем все замыкающие звенья
            foreach (UIElement UI in leftStackPanel.Children)       // открываем все кнопки на левой панели
            {
                UI.IsEnabled = true;
            }
            isCreatingDimension=false;          // размер больше не создаётся
                // проверяем на наличие цикла
            if (dim.tp == type.tech)
            {
                if (graph.isCicle)
                    window.lblStateCicle.Content = "Размерная цепь замкнута!";
                else
                    window.lblStateCicle.Content = "";
            }
        }

//******************************************************************************************************************************************************************

            // пересчёт всех замыкающих звеньев
        private void ReBuildAll()
        {
            
            foreach (Dimension d in graph.dimensionList)
            {
                if (d._type == type.konstr || d._type == type.pripusk)
                {
                    List<Dimension> list = new List<Dimension>();
                    Value val = graph.BFS_FindShortWay(d.firstVertex, d.secondVertex, list);    //
                    if (val != null)
                    {
                        d.value = val;
                        dicDim_UI[d].SetUp(val.upTolerance.ToString());
                        dicDim_UI[d].SetDown(val.downTolerance.ToString());
                        dicDim_UI[d].SetNominal(val.nominal.ToString());
                        if (d._type == type.pripusk)
                            Controller_PripuskApdated(dicDim_UI[d]);    // проверим припуск - не выходит ли он за пределы допустимого
                        if (d._type == type.konstr)
                            Controller_ConstrApdated(dicDim_UI[d]);    // проверим конструкторский размер - не выходит ли он за пределы допустимого
                    }
                    else
                    {
                        dicDim_UI[d].SetNominal("?");
                        dicDim_UI[d].SetUp("?");
                        dicDim_UI[d].SetDown("?");
                        dicDim_UI[d].NotAlarm();
                    }
                    dicDim_L[d] = list;
                    dicDim_UI[d].lblNominal.UpdateLayout();
                    dicDim_UI[d].SetUpDownLbls();
                }
                else
                    dicDim_L[d] = new List<Dimension>();        // обнуляем список зависимых у технологического размера
            }
            foreach (Dimension dim in dicDim_L.Keys)
            {
                if (dim._type == type.konstr || dim._type == type.pripusk)
                {
                    foreach (Dimension tDim in dicDim_L[dim])               // в словаре dicDim_L[dim] для ключа - припуска или констр. размера значением будет только техн. размер - см. BFS_FindShortWay - в формировании пути к замыкающему звену учавствуют только техн. типы разера
                    {
                        dicDim_L[tDim].Add(dim);
                    }
                }
            }
        }

//--------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Обработка события изменения технологического размера пользователем
        /// </summary>
        /// <param name="dim">UI-размер, который сгенерировал событие</param>
        void Controller_TechDimensionApdated(UI_TechDimension dim)
        {
            if (dicUI_Dim.ContainsKey(dim))     // размеры, которые находятся в процессе создания, но уже генерят событие не будут учитываться
            {
                Value val = new Value(dim.nominal, dim.up, dim.down);
                dicUI_Dim[dim].firstVertex.distanceTo[dicUI_Dim[dim].secondVertex] = null;
                dicUI_Dim[dim].secondVertex.distanceTo[dicUI_Dim[dim].firstVertex] = null;
                dicUI_Dim[dim].firstVertex.distanceTo[dicUI_Dim[dim].secondVertex] = val;
                dicUI_Dim[dim].secondVertex.distanceTo[dicUI_Dim[dim].firstVertex] = val.Inverse();
                ReBuildAll();
            }
        }

        /// <summary>
        /// обработка события при изменении припуска пользователем (желаемого пользователем максимума или минимума)
        /// </summary>
        /// <param name="dim">UI-припуск, который сгенерировал событие</param>
        void Controller_PripuskApdated(UI_Dimension dim)
        {
            UI_PripuskDimension UI_pd = dim as UI_PripuskDimension;
            if (dim.lblNominal.Content.ToString() != "?")
            {
                double _max = Double.Parse(dim.lblNominal.Content.ToString()) + Double.Parse(dim.lblUp.Content.ToString());
                double _min = Double.Parse(dim.lblNominal.Content.ToString()) + Double.Parse(dim.lblDown.Content.ToString());
                if (_max > (dim as UI_PripuskDimension).max || _min < (dim as UI_PripuskDimension).min)
                    (dim as UI_PripuskDimension).Alarm();
                else
                    (dim as UI_PripuskDimension).NotAlarm();
            }
        }

        /// <summary>
        /// Обработка события при изменении конструкторского размера пользователем (желаемого пользователем максимума или минимума)
        /// </summary>
        /// <param name="dim"></param>
        void Controller_ConstrApdated(UI_Dimension dim)
        {
            if (dim.lblNominal.Content.ToString() != "?")
            {
                double _max = Double.Parse(dim.lblNominal.Content.ToString()) + Double.Parse(dim.lblUp.Content.ToString());
                double _min = Double.Parse(dim.lblNominal.Content.ToString()) + Double.Parse(dim.lblDown.Content.ToString());
                if (_max > (dim as UI_ConstrDimension).max || _min < (dim as UI_ConstrDimension).min)
                    (dim as UI_ConstrDimension).Alarm();
                else
                    (dim as UI_ConstrDimension).NotAlarm();
            }
        }


            // обработка события нажатия кнопки *Delete* по окну приложения
        void MainWindow_DelPressed()
        {
            if (chosedDimension != null)
            {
                rightStackPanel.Children.Clear();
                chosedDimension.RemoveFromCanv();
                graph.DeleteDimension(dicUI_Dim[chosedDimension]);
                dicDim_L.Remove(dicUI_Dim[chosedDimension]);        // а в ReBuildAll() пересчитываются так же все зависимые размеры
                dicDim_UI.Remove(dicUI_Dim[chosedDimension]);
                dicUI_Dim.Remove(chosedDimension);
                listOfDimensions.Remove(chosedDimension);

                ReBuildAll();             // пересчитываем все замыкающие звенья
                if (chosedDimension.tp == type.tech )  // перепроверяем на наличие цикла
                {
                    if (graph.isCicle)
                        window.lblStateCicle.Content = "Размерная цепь замкнута!";
                    else
                        window.lblStateCicle.Content = "";
                }
                chosedDimension = null;
                UnPickIfChosed();

            }
        }

            // обработка события нажатия кнопки ESC - отмена создания размера
        void MainWindow_EscPressed()
        {
            if (creatingDimAtThisMoment != null)
            {
                rightStackPanel.Children.Clear();
                creatingDimAtThisMoment.OtpiskaWhenCreate();
                creatingDimAtThisMoment.RemoveFromCanv();

                foreach (UIElement UI in leftStackPanel.Children)
                {
                    UI.IsEnabled = true;
                }
                creatingDimAtThisMoment = null;
                isCreatingDimension = false;
                UI_Dimension.temporaryUI=null;
            }
        }

            // обработчик события при клике на размер
        void Controller_ClickOnDimension(UI_Dimension dim)
        {
            if (!isCreatingDimension)
            {
                UnPickIfChosed();
                dim.Pick();
                pickedDimensionList.Add(dim);
                chosedDimension = dim;
                switch(chosedDimension.tp)
                {
                    case type.tech:
                        dim.UC = TUC;
                        TUC.SetUI_TD(dim as UI_TechDimension);
                        break;
                    case type.konstr:
                        dim.UC = CUC;
                        CUC.SetUI_CD(dim as UI_ConstrDimension);
                        break;
                    case type.pripusk:
                        dim.UC = PUC;
                        PUC.SetUI_PD(dim as UI_PripuskDimension);
                        break;
                }   
                rightStackPanel.Children.Add(dim.UC);
                foreach (Dimension d in dicDim_L[dicUI_Dim[dim]])
                {
                    dicDim_UI[d].Pick();
                    pickedDimensionList.Add(dicDim_UI[d]);
                }
            }
        }

                // обработка события нажатия на канвас
        void MainWindow_MouseLeftButtonDown()
        {
            UnPickIfChosed();
        }

            // отмена выбора размера
        private void UnPickIfChosed()
        {
            foreach (UI_Dimension UId in pickedDimensionList)
            {
                UId.Unpick();
            }
            pickedDimensionList.Clear();
            if (chosedDimension != null)
            {
                rightStackPanel.Children.Clear();
                chosedDimension.Unchoose();
                chosedDimension = null;
            }
        }

                // сохранение проекта с выбором файла
        void MainWindow_SaveAllAs()
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            fileName = SFD.FileName;
            if (fileName == "")
            {
                MessageBox.Show("Файл не выбран!");
                return;
            }
            if (fileName.Substring(fileName.Length - 4, 4) != ".dch")
            {
                fileName += ".dch";
            }

            Save save = new Save(graph,dicUI_Dim);
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, save);
            fs.Close();
            window.SetSaveEnable(true);
            window.Title = "Автоматический рассчёт размерных цепей - " + fileName;
        }

                // сохранение проекта в уже выбраный раннее файл
        void MainWindow_SaveAll()
        {
            if (fileName == "")
            {
                MessageBox.Show("Файл не выбран!");
                return;
            }
            Save save = new Save(graph, dicUI_Dim);
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, save);
            fs.Close();
        }

        void MainWindow_OpenSaved()
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "*.dch|*.dch";
            OFD.ShowDialog();
            fileName = OFD.FileName;
            if (fileName == "")
            {
                MessageBox.Show("Файл не выбран!");
                return;
            }

            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                Save save = (Save)bf.Deserialize(fs);
                fs.Close();
                MainWindow_EscPressed();    // если вдруг в этот момент создаётся размер
                window.SetSaveEnable(true);
                window.Title = "Автоматический рассчёт размерных цепей - " + fileName;

                canv.Children.Clear();
                rightStackPanel.Children.Clear();
            
                this.graph = save.graph;
                listOfDimensions.Clear();                  // список всех размеров
                dicUI_Dim.Clear();                         // словарь: визуальный размер -> размер
                dicDim_UI.Clear();                         // словарь: размер -> визуальный размер

                isCreatingDimension=false; 
                chosedDimension=null;
                creatingDimAtThisMoment=null;

                foreach (UI_Dimension_Save UIDSaved in save.dic_UISave_Dim.Keys)
                {
                    UI_Dimension newUIDim=null;
                    switch (UIDSaved.typ)
                    {
                        case type.tech:
                            newUIDim = new UI_TechDimension(UIDSaved);
                            (newUIDim as UI_TechDimension).TechDimensionApdatedEvent += new UI_TechDimension.TechDimensionApdatedEventHandler(Controller_TechDimensionApdated);
                            break;
                        case type.pripusk:
                            newUIDim = new UI_PripuskDimension(UIDSaved);
                            (newUIDim as UI_PripuskDimension).PripuskApdatedEvent += new UI_PripuskDimension.PripuskApdatedEventHandler(Controller_PripuskApdated);
                            break;
                        case type.konstr:
                            newUIDim = new UI_ConstrDimension(UIDSaved);
                            (newUIDim as UI_ConstrDimension).ConstrApdatedEvent += new UI_ConstrDimension.ConstrApdatedEventHandler(Controller_ConstrApdated);
                            break;
                    }
                    newUIDim.dimensionClicked += new UI_Dimension.dimensionClickedEventHandler(Controller_ClickOnDimension);
                    newUIDim.lblClickedOtherPodpiska();
                    listOfDimensions.Add(newUIDim);
                    dicUI_Dim.Add(newUIDim, save.dic_UISave_Dim[UIDSaved]);
                    dicDim_UI.Add(save.dic_UISave_Dim[UIDSaved], newUIDim);
                }
                ReBuildAll();
                if (graph.isCicle)
                    window.lblStateCicle.Content = "Размерная цепь замкнута!";
                else
                    window.lblStateCicle.Content = "";
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

                // создание нового проекта - удаление всего с канваса и обнуление
        void MainWindow_NewProj()
        {
            MainWindow_EscPressed();    // если вдруг в этот момент создаётся размер
            canv.Children.Clear();
            //canv.Children.Add(new Line());
            rightStackPanel.Children.Clear();

            graph = new Graph();
            listOfDimensions.Clear();                  // список всех размеров
            dicUI_Dim.Clear();                         // словарь: визуальный размер -> размер
            dicDim_UI.Clear();                         // словарь: размер -> визуальный размер

            isCreatingDimension = false;
            chosedDimension = null;
            window.SetSaveEnable(false);
            window.Title = "Автоматический рассчёт размерных цепей"; 
        }

//---------------------------------------     Масштабирование колёсиком    ---------------------------------------------------------------------------------------------------

        void MainWindow_WheelUp(Point p)
        {
            /*scale.CenterX = p.X;      // другой вариант
            scale.CenterY = p.Y;
            scale.ScaleX += 0.1;
            scale.ScaleY += 0.1;
            canv.LayoutTransform = scale;
            canv.UpdateLayout();*/
            
            if (UI_Dimension.temporaryUI!=null)
                listOfDimensions.Add(UI_Dimension.temporaryUI);
            foreach (UI_Dimension dim in listOfDimensions)
            {
                dim.firstElX = p.X + (dim.firstElX - p.X) * M;
                dim.firstElY = p.Y + (dim.firstElY - p.Y) * M;
                dim.secondElX = p.X + (dim.secondElX - p.X) * M;
                dim.secondElY = p.Y + (dim.secondElY - p.Y) * M;
                dim.mainLine.X1 = dim.firstElX;
                dim.mainLine.X2 = dim.secondElX;
                dim.mainLine.Y1 = p.Y + (dim.mainLine.Y1 - p.Y) * M;
                dim.mainLine.Y2 = dim.mainLine.Y1;
                dim.RebuildAfterScale();
            }
            if (UI_Dimension.temporaryUI != null)
                listOfDimensions.Remove(UI_Dimension.temporaryUI);
        }

        void MainWindow_WheelDown(Point p)
        {
            if (UI_Dimension.temporaryUI != null)
                listOfDimensions.Add(UI_Dimension.temporaryUI);
            foreach (UI_Dimension dim in listOfDimensions)
            {
                dim.firstElX = p.X + (dim.firstElX - p.X) / M;
                dim.firstElY = p.Y + (dim.firstElY - p.Y) / M;
                dim.secondElX = p.X + (dim.secondElX - p.X) / M;
                dim.secondElY = p.Y + (dim.secondElY - p.Y) / M;
                dim.mainLine.X1 = dim.firstElX;
                dim.mainLine.X2 = dim.secondElX;
                dim.mainLine.Y1 = p.Y + (dim.mainLine.Y1 - p.Y) / M;
                dim.mainLine.Y2 = dim.mainLine.Y1;
                dim.RebuildAfterScale();
            }
            if (UI_Dimension.temporaryUI != null)
                listOfDimensions.Remove(UI_Dimension.temporaryUI);
        }
    }
}
