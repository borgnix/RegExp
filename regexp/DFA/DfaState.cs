using System;
using System.Collections.Generic;

namespace regexp
{
	public class DfaState
	{
		public int DfaID { get; }

		public DfaState (int dfaId)
		{
			DfaID = dfaId;
			To = new Dictionary<char, DfaState> ();
		}

		public Dictionary<char, DfaState> To { get; }

		public void AddEdge (DfaState to, char transitionChar)
		{
			if (To.ContainsKey (transitionChar)) {
				return;
			}
			To.Add (transitionChar, to);
		}

		public DfaState EdgeTo (char c)
		{
			DfaState state;
			if (!To.TryGetValue (c, out state)) {
				return null;
			}
			return state;
		}
	}
}

