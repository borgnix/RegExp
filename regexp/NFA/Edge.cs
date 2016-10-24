using System;

namespace regexp
{
	public class Edge
	{
		public State From{ get; set;}
		public State To{ get; set;}
		public char? C { get; set;}
		public Edge (State from, State to, char? c)
		{
			From = from;
			To = to;
			C = c;
		}
	}
}

