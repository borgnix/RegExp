using System;
using System.Collections.Generic;
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
			Console.WriteLine (nfa.match (@"dss"));
			Console.WriteLine (nfa.match (@"dsd"));

			Dfa dfa = new Dfa (nfa);
			Console.WriteLine(dfa.Match(@"dsddds"));
			Console.WriteLine (dfa.Match (@"dss"));
			Console.WriteLine (dfa.Match (@"dsd"));


		}
	}
}
