using System;

namespace regexp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string str = @"ds(d)*s|s";
			Parser parser = new Parser (str);
			parser.Print ();
			Nfa nfa = new Nfa (parser.Ast);

			Console.WriteLine(nfa.match(@"dsddds"));
		}
	}
}
