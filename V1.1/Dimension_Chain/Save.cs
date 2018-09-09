using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimension_Chain
{
    [Serializable]
    class Save
    {
        public Graph graph;
        public Dictionary<UI_Dimension_Save, Dimension> dic_UISave_Dim = new Dictionary<UI_Dimension_Save, Dimension>();

        public Save(Graph graph, Dictionary<UI_Dimension, Dimension> dicUI_Dim)
        {
            this.graph = graph;

            foreach (UI_Dimension UI_Dim in dicUI_Dim.Keys)
            {
                UI_Dimension_Save UIDS;
                switch (UI_Dim.tp)
                {
                    case type.tech:
                        UIDS = new UI_TechDimension_Save(UI_Dim as UI_TechDimension);
                        dic_UISave_Dim.Add(UIDS, dicUI_Dim[UI_Dim]);
                        break;
                    case type.pripusk:
                        UIDS = new UI_PripuskDimension_Save(UI_Dim as UI_PripuskDimension);
                        dic_UISave_Dim.Add(UIDS, dicUI_Dim[UI_Dim]);
                        break;
                    case type.konstr:
                        UIDS = new UI_ConstrDimension_Save(UI_Dim as UI_ConstrDimension);
                        dic_UISave_Dim.Add(UIDS, dicUI_Dim[UI_Dim]);
                        break;
                }
            }
        }
    }


//******************************************************************************************************************************************

    [Serializable]
    public class UI_Dimension_Save
    {
        public type typ;
        public double firstElX;
        public double firstElY;
        public double secondElX;
        public double secondElY;
        public double mainLineY;

        public UI_Dimension_Save(UI_Dimension dim)
        {
            firstElX = dim.firstElX;
            firstElY = dim.firstElY;
            secondElX = dim.secondElX;
            secondElY = dim.secondElY;
            mainLineY = dim.mainLine.Y1;
        }
    }

    [Serializable]
    class UI_TechDimension_Save : UI_Dimension_Save
    {
        public double nominal;
        public double up;
        public double down;
        public UI_TechDimension_Save(UI_TechDimension dim) : base(dim)
        {
            nominal = dim.nominal;
            up = dim.up;
            down = dim.down;
            typ = type.tech;
        }
    }

    [Serializable]
    class UI_ConstrDimension_Save : UI_Dimension_Save
    {
        public double nominal;
        public double up;
        public double down;
        public UI_ConstrDimension_Save(UI_ConstrDimension dim) : base(dim)
        {
            nominal = dim.nominal;
            up = dim.up;
            down = dim.down;
            typ = type.konstr;
        }
    }

    [Serializable]
    class UI_PripuskDimension_Save : UI_Dimension_Save
    {
        public double max;     
        public double min;
        public UI_PripuskDimension_Save(UI_PripuskDimension dim) : base(dim)
        {
            max = dim.max;
            min = dim.min;
            typ = type.pripusk;
        }
    }
}
