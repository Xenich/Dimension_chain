using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimension_Chain
{
    /// <summary>
    /// Граф, состоящий из вершин, которые связаны технологическими дугами
    /// </summary>
    [Serializable]
    class TechGraph
    {
        public Dictionary<Vertex, MegaVertex> dicV_M = new Dictionary<Vertex, MegaVertex>();        // принадлежность вертекса определённому мегавертексу
        public List<MegaVertex> megaList = new List<MegaVertex>();                                  // список мегавертексов техграфа
        public List<MegaVertex> connectedComponentList = new List<MegaVertex>();                    // список компонент связности - первые мегавертексы, принадлежащие разным компонентам связности графа. Количество элементов этого списка = количеству компонентов связности
        public bool cicle = false;                                                                  // есть ли в графе цикл


        /*public TechGraph(List<Vertex> vertexList)
        {
            foreach (Vertex v in vertexList)
            { v.visited = false; } 
                    // создаём мегаввертексы
            foreach (Vertex v in vertexList)
            {
                if (!v.visited)
                {
                    List<Vertex> list = new List<Vertex>();
                    MegaVertex mv = new MegaVertex();
                    dicV_M.Add(v, mv);
                    megaList.Add(mv);
                    v.visited = true;
                    list.Add(v);
                    foreach (Vertex n in v.neighbors)
                    {
                        if (v.dimensionTo[n]._type == type.nul)
                        {
                            list.Add(n);
                            dicV_M.Add(n, mv);
                            n.visited = true;
                        }
                    }
                    mv.vertexList = list;
                }
            }
                    // создаём связи между megaVertex-ами
            foreach (MegaVertex mv in megaList)
            {
                foreach (Vertex v in mv.vertexList)
                {
                    foreach (Vertex n in v.neighbors)
                    {
                        if (v.dimensionTo[n]._type == type.tech)
                        {
                            mv.neighbors.Add(dicV_M[n]);
                        }
                    }
                }
            }
        }
        */
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CreateTechDimension(Dimension d)
        {
            if(d._type!=type.tech)
                return;
            CheckVertex(d.firstVertex);         
            CheckVertex(d.secondVertex);
                // добавляем технологическую связь между мегавертексами, которым принадлежат вертексы создаваемого размера
            dicV_M[d.firstVertex].neighbors.Add(dicV_M[d.secondVertex]);
            dicV_M[d.secondVertex].neighbors.Add(dicV_M[d.firstVertex]);
            connectedComponentSearch();
            SearchCircle();
        }

        private void CheckVertex(Vertex v)
        {
            if (v.neighbors.Count == 1)        // если сосед только один, то он - вторая вершина этого технологического размера d и сам вертекс - висячий
            {
                MegaVertex mv = new MegaVertex();
                mv.vertexList.Add(v);
                megaList.Add(mv);
                dicV_M.Add(v, mv);
            }
            else
            {
                for (int i = 0; i < v.neighbors.Count; i++)
                {
                    if (v.dimensionTo[v.neighbors[i]]._type == type.nul && dicV_M.ContainsKey(v.neighbors[i]))
                    {
                        dicV_M[v.neighbors[i]].vertexList.Add(v);        // этому мегавертексу теперь принадлежит и d.firstVertex
                        dicV_M.Add(v, dicV_M[v.neighbors[i]]);
                        break;                                           // достаточно первого попавшегося
                    }
                    if (i == v.neighbors.Count - 1)                      // если перечислили всё и дошли до сюда, то значит, что эта вершина соединена с нетехнологическим размером   
                    {
                        MegaVertex mv = new MegaVertex();            
                        mv.vertexList.Add(v);
                        megaList.Add(mv);
                        dicV_M.Add(v, mv);
                    }
                }
            }
        }
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void DeleteDimension(Dimension d)
        {
            MegaVertex mv1 = dicV_M[d.firstVertex];
            MegaVertex mv2 = dicV_M[d.secondVertex];

            mv1.neighbors.Remove(mv2);                                  // удаляем из списка соседей мегавертексы
            mv2.neighbors.Remove(mv1);     
            mv1.vertexList.Remove(d.firstVertex);                       // удаляем из списка вертексов соответствующего мегавертекса вертексы
            mv2.vertexList.Remove(d.secondVertex);
            dicV_M.Remove(d.firstVertex);
            dicV_M.Remove(d.secondVertex);
            if (mv1.vertexList.Count == 0)
                megaList.Remove(mv1);
            if (mv2.vertexList.Count == 0)
                megaList.Remove(mv2);
            
            connectedComponentSearch();
            SearchCircle();
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
                // поиск компонент связности
        public void connectedComponentSearch()
        {
            List<MegaVertex> connectedComponentList = new List<MegaVertex>();
            VisitedFalse();
            foreach (MegaVertex v in megaList)
            {
                if (!v.visited)                         // если вершина не была посещена - то нашли новую компоненту связности; запускаем из этой вершины глубинный обход и посещаем все вершины из этой найденной компоненты связности
                {
                    connectedComponentList.Add(v);
                    DFS(v);
                }
            }
            this.connectedComponentList = connectedComponentList;
        }
                // обход в глубину
        private void DFS(MegaVertex start)
        {
            start.visited = true;
            foreach (MegaVertex n in start.neighbors)
            {
                if (!n.visited)
                    DFS(n);
            }
        }
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Поиск цикла, - проверка на замкнутость размерной цепи
        /// </summary>
        public void SearchCircle()
        {
            cicle = false;
            VisitedFalse();
            foreach (MegaVertex mv in connectedComponentList)        // ищем цикл в каждой компоненте связности
            {
                if (!cicle)
                    DFS_Cicle(mv, null);
            }
        }
        private void DFS_Cicle(MegaVertex v, MegaVertex parent)
        {
            if (v.visited)
            {
                cicle = true;
                return;
            }
            v.visited = true;
            foreach (MegaVertex n in v.neighbors)
            {
                if (n != parent)
                {
                    DFS_Cicle(n, v);
                }
            }
        }
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void VisitedFalse()
        {
            foreach (MegaVertex mv in megaList)
            { mv.visited = false; } 
        }
    }
}
