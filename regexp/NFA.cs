using System.Collections;

namespace regexp
{
    public class State
    {
        enum StateType
        {
            Start,
            Terminal,
            NonTerminal
        }
        public StateType Type { get; set }
    }

    public class Edge
    {
        public char Char { get ; set }
        public State from { get; set };
        public State to { get; set };
    }

    public class NFA
    {
        private ArrayList Edges;
        private ArrayList States;

    }
}
