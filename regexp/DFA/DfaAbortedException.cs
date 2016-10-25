using System;

namespace regexp
{
	public class DfaAbortedException : Exception
	{
		public DfaAbortedException ()
		{
		}

		public DfaAbortedException(string msg)
			: base(msg)
		{
		}
	}
}

