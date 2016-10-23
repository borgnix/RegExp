using System;

namespace regexp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string str = @"\*dsd(ds|s)dc";
			Parser parser = new Parser(str);
			parser.ToString ();
		}
	}
}
