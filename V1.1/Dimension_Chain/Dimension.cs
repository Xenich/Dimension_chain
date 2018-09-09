using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dimension_Chain
{
    public enum type
    {konstr, pripusk, tech, nul}

    [Serializable]
    class Dimension
    {
        public Vertex firstVertex;
        public Vertex secondVertex;
        public type _type;
        public Value value;     // значение размера от firstVertex до secondVertex. в другом направлении будет значение value.Inverse()
        public bool visited;

        public Dimension(Vertex firstVertex, Vertex secondVertex, type _type, Value value)
        {
            this.firstVertex = firstVertex;
            this.secondVertex = secondVertex;
            this._type = _type;
            this.value = value;
            visited = false;
        }
    }

    [Serializable]    
    class Value
    {
        public double nominal;
        public double upTolerance;
        public double downTolerance;
        public bool error = false;

        public Value()
        { }

        public Value(double nominal, double upTolerance, double downTolerance)
        {
            this.nominal = nominal;
            this.upTolerance = upTolerance;
            this.downTolerance = downTolerance;
        }

        public Value Inverse()
        {
            return new Value(0 - nominal, 0 - downTolerance, 0 - upTolerance);
        }
    }
}
