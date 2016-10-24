using System;

namespace regexp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string str = @"\*";
			Parser parser = new Parser(str);
			parser.Print ();
		}
	}
}
