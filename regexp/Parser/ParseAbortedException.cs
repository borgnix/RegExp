using System;

namespace regexp
{
	public class ParseAbortedExcetpion : Exception
	{
		public ParseAbortedExcetpion() 
		{
		}

		public ParseAbortedExcetpion(string message):
			base(message) 
		{
		}

	}
}

