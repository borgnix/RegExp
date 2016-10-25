using System;
using System.Collections.Generic;

namespace regexp
{
	public class NfaState
	{
		public int StateID { get; }

		public Dictionary<char, List<NfaState>> To{ get; }

		public NfaState (int id)
		{
			StateID = id;
			To = new Dictionary<char, List<NfaState>> ();
		}
			
	}
}

