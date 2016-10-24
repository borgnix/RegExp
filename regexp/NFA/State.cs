using System;
using System.Collections.Generic;

namespace regexp
{
	public class State
	{
		public int StateID { get;}
		public Dictionary<char, List<State>> To{ get; }
		public State (int id)
		{
			StateID = id;
			To = new Dictionary<char, List<State>> ();
		}


	}
}

