using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimension_Chain
{
    [Serializable]
    class Graph             // Класс Graph - это MODEL проекта
    {
        public int num = 0;                         
        public List<Vertex> vertexList = new List<Vertex>();                // список вершин графа 
        public List<Dimension> dimensionList = new List<Dimension>();       // список размеров(дуг) в графе 
        private TechGraph techGraph = new TechGraph();                      // граф, составленый из технологических размеров, будет использоваться в алгоритме нахождения циклов - при проверке на переопределённость размерной цепи
        public bool isCicle
        { get { return techGraph.cicle; } }

        public Vertex CreateVertex()
        {
            num++;
            Vertex v = new Vertex(num);
            vertexList.Add(v);
            return v;
        }
//-------------------------------------------------------------------------------------------------
        public bool CreateNul(Vertex v1, Vertex v2)
        {
            if (vertexList.Contains(v1) && vertexList.Contains(v2))
            {
                Value val = new Value(0, 0, 0);
                Dimension dim = new Dimension(v1, v2, type.nul, val);
                dimensionList.Add(dim);
                v1.AddNeighbor(v2, new Value(0, 0, 0), dim);
                v2.AddNeighbor(v1, new Value(0, 0, 0), dim);
                return true;
            }
            return false;
        }

        /*public void CreateNul(List<Vertex> list)        // создание связей всех со всеми в списке вершин list         тут всё правильно, но пока что этот метод не нужен
        {
            foreach (Vertex v1 in list)
            {
                foreach (Vertex v2 in list)
                {
                    if (v1 != v2)
                    {
                        Dimension dim = new Dimension(v1, v2, type.nul);
                        dimensionList.Add(dim);
                        v1.AddNeighbor(v2, new Value(0, 0, 0), dim);
                        v2.AddNeighbor(v1, new Value(0, 0, 0), dim);
                    }
                }
            }
        }*/

        public Dimension CreateClosingLink(Vertex v1, Vertex v2, type typ)
        {
            if (vertexList.Contains(v1) && vertexList.Contains(v2))
            {
                Value val = new Value(0, 0, 0);
                Dimension dim = new Dimension(v1, v2, typ, val);
                dimensionList.Add(dim);
                v1.AddNeighbor(v2, val, dim);
                v2.AddNeighbor(v1, new Value(0, 0, 0), dim);
                return dim;
            }
            return null;
        }

        public Dimension CreateTechDimension(Vertex v1, Vertex v2, Value val)
        {
            if (vertexList.Contains(v1) && vertexList.Contains(v2))
            {
                Dimension dim = new Dimension(v1, v2, type.tech, val);
                dimensionList.Add(dim);
                v1.AddNeighbor(v2, val, dim);
                v2.AddNeighbor(v1, val.Inverse(), dim);
                techGraph.CreateTechDimension(dim);         // создать связь в технологическом графе
                return dim;
            }
            return null;
        }
 //-----------------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Удаление размера и всех связанных с ним вершин и всех связанных с ними размерами
        /// </summary>
        /// <param name="dim">Удаляемый размер</param>
        public void DeleteDimension(Dimension dim)
        {
            if (dim._type == type.tech)
                techGraph.DeleteDimension(dim);
            DeleteDimensionPart(dim);
            foreach (Dimension d in dimensionList.ToArray())
            {
                if (d.firstVertex == dim.firstVertex || d.firstVertex == dim.secondVertex||d.secondVertex==dim.firstVertex||d.secondVertex==dim.secondVertex)
                {
                    DeleteDimensionPart(d);
                }
            }
            if (dim.firstVertex.neighbors.Count == 0)
                vertexList.Remove(dim.firstVertex);
            if (dim.secondVertex.neighbors.Count == 0)
                vertexList.Remove(dim.secondVertex);
        }
            // часть метода
        private void DeleteDimensionPart(Dimension dim)
        {
             dim.firstVertex.DeleteNeighbor(dim.secondVertex);
             dim.secondVertex.DeleteNeighbor(dim.firstVertex);
             dimensionList.Remove(dim);
        }
//---------------------------------------------------------------------------------------------------------------------------------
                // поиск кратчайшего пути в ширину в невзвешенном графе - рассчёт замыкающего звена в размерной цепи, все остальные размеры которой являются технологическими
        public Value BFS_FindShortWay(Vertex source, Vertex destination, List<Dimension> receivedList)         // receivedList - будем возвращать, чтоб воссоздать последовательность размеров
        {
            Queue<Vertex> qwewe = new Queue<Vertex>();
            Dictionary<Vertex, Vertex> parentOf = new Dictionary<Vertex, Vertex>();         // словарь предков - из какой вершины мы пришли в данную вершину
            Vertex current;
            bool finded = false;

            VisitedFalse();
            qwewe.Enqueue(source);
            source.visited = true;
            while (qwewe.Count > 0 & !finded)
            {
                current = qwewe.Dequeue();
                foreach (Vertex v in current.neighbors)
                {
                    if (!v.visited && (current.dimensionTo[v]._type == type.tech || current.dimensionTo[v]._type==type.nul))      // ходим только по технологическим размерам и непосещённым вершинам
                    {
                        parentOf[v] = current;
                        if (v == destination)           // подошли к целевой вершине - нашли путь - цикл
                        {
                            finded = true;
                            break;
                        }
                        qwewe.Enqueue(v);
                        v.visited = true;
                    }
                }
            }
                    // восстанавливаем обратный путь
            if (!finded)
                return null;

            Stack<Vertex> stack = new Stack<Vertex>();
            Vertex next;
            current = destination;
            while (current != source)       // проходим обратным путём, заполняя стек , НУЖНО БУДЕТ ПЕРЕДЕЛАТЬ без использования вспомогательного стека
            {
                stack.Push(current);
                current = parentOf[current];
            }
            current = source;
            Value val = new Value();
            while (stack.Count > 0)
            {
                next = stack.Pop();
                val.nominal += current.distanceTo[next].nominal;
                val.upTolerance += current.distanceTo[next].upTolerance;
                val.downTolerance += current.distanceTo[next].downTolerance;
                if (current.dimensionTo[next]._type == type.tech)
                    receivedList.Add(current.dimensionTo[next]);
                current = next;
            }

            val.nominal = Math.Round(val.nominal,4);
            val.upTolerance = Math.Round(val.upTolerance, 4);
            val.downTolerance = Math.Round(val.downTolerance, 4);
            return val;
        }

            // обнуляем посещаемость всех вершин графа
        public void VisitedFalse()
        {
            foreach (Vertex v in vertexList)
            { v.visited = false; }        
        }
    }
}
