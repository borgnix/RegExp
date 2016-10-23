using System;

namespace regexp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string str = @"\*dsd(ds|s)dc";
			var result = Parser.parseRegExp (str, 0);
			var ast = result.Item2;

			Parser.PrintAst (ast);
		}
	}
}
