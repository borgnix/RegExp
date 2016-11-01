using System;
using System.Collections.Generic;
namespace regexp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string str = @"dd*(.|d)";
			Parser parser = new Parser (str);
			parser.Print ();
			Nfa nfa = new Nfa (parser.Ast);

			Console.WriteLine(nfa.Match(@"d..dd"));
			Console.WriteLine (nfa.Match (@"dd"));
			Console.WriteLine (nfa.Match (@"321"));
			nfa.dump ("/tmp/nfa.dot");

			Dfa dfa = new Dfa (nfa);
			Console.WriteLine(dfa.Match(@"d..dd"));
			Console.WriteLine (dfa.Match (@"dd"));
			Console.WriteLine (dfa.Match (@"321"));
			dfa.dump ("/tmp/dfa.dot");

			dfa.MinimizeDfa ();
			Console.WriteLine(dfa.Match(@"d..dd"));
			Console.WriteLine (dfa.Match (@"dd"));
			Console.WriteLine (dfa.Match (@"d."));
			dfa.dump ("/tmp/minimizedDfa.dot");

		}
	}
}
