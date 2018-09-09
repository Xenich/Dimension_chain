using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimension_Chain
{
    [Serializable]
    class MegaVertex        // вершина графа, которая объединяет все простые vertex, достижимые друг друга через размер type.null
    {
        public List<Vertex> vertexList = new List<Vertex>();
        public List<MegaVertex> neighbors = new List<MegaVertex>();                     // список смежных вершин, которые достижимы по пути dimension._type=type.tech
        public bool visited = false;                                                    // будет использоваться в алгоритмах
    }
}
