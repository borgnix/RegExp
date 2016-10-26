using System;
using System.Collections.Generic;
namespace regexp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string str = @"(1|2|3)*123";
			Parser parser = new Parser (str);
			parser.Print ();
			Nfa nfa = new Nfa (parser.Ast);

			Console.WriteLine(nfa.Match(@"123"));
			Console.WriteLine (nfa.Match (@"11123"));
			Console.WriteLine (nfa.Match (@"321"));

			Dfa dfa = new Dfa (nfa);
			Console.WriteLine(dfa.Match(@"123"));
			Console.WriteLine (dfa.Match (@"11123"));
			Console.WriteLine (dfa.Match (@"321"));

			dfa.MinimizeDfa ();
			Console.WriteLine(dfa.Match(@"123"));
			Console.WriteLine (dfa.Match (@"11123"));
			Console.WriteLine (dfa.Match (@"321"));
		}
	}
}
