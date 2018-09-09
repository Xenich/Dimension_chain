#define d2
//#define d1

using System;
using System.Collections.Generic;

namespace Dimension_Chain
{
    [Serializable]
    class Vertex
    {
        public int num;
        public List<Vertex> neighbors = new List<Vertex>();                     // список смежных вершин
        public bool visited = false;                                            // будет использоваться в алгоритмах

        public Dictionary<Vertex, Dimension> dimensionTo = new Dictionary<Vertex, Dimension>();         // словарь : смежная вершина - размер до неё
        public Dictionary<Vertex, Value> distanceTo = new Dictionary<Vertex, Value>();                  // словарь : смежная вершина - расстояние до неё

        /*public Vertex()
        { }*/

        public Vertex(int num)
        {
            this.num = num; 
        }

        public void AddNeighbor(Vertex neighbor, Value val, Dimension dim)
        {

            if (!neighbors.Contains(neighbor))
            {
                neighbors.Add(neighbor);
                distanceTo.Add(neighbor, val);
                dimensionTo.Add(neighbor, dim);
            }
            else
            {
                distanceTo[neighbor] = val;
                dimensionTo[neighbor] = dim;
            }
        }

        public void DeleteNeighbor(Vertex neighbor)
        {
            if (neighbors.Contains(neighbor))
            {
                neighbors.Remove(neighbor);
                distanceTo.Remove(neighbor);
                dimensionTo.Remove(neighbor);
            }
        }
    }
}
