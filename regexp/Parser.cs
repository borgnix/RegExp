using System;
using System.Collections.Generic;

namespace regexp
{
	class Parser {
		private static HashSet<Char> SpecialTokens = new HashSet<Char> {'\\', '(', ')', '|', '*'};

		public string ExpStr { get; }
		public Exp Ast { get; }

		private int Cursor = 0;

		public Parser (string regexp) {
			ExpStr = regexp;
			Ast = ParseRegExp ();
		}

		private static bool InRange(char c) {
			return !Parser.SpecialTokens.Contains (c);
		}
			
		/* Grammar
		 * regexp := <exp> '|' <regexp> | <exp>
		 * exp := <repexp> <exp> | <repexp>
		 * repexp := <basicexp> * | <basicexp>
		 * basicexp := <alnum> | ( <regexp> )
		 */

		private Exp ParseRegExp() {
			var exp1 = ParseExp ();

			if (Cursor >= ExpStr.Length) {
				return exp1;
			}
				
			if (ExpStr [Cursor] == '|') {
				Cursor += 1;
				var exp2 = ParseRegExp ();

				Exp exp = Exp.buildAlter (exp1, exp2);

				return exp;
			} 

			return exp1;
		}

		private Exp ParseExp() {
			Exp exp1 = ParseRepExp ();

			if (exp1 == null) {
				return null;
			}

			if (Cursor >= ExpStr.Length) {
				return exp1;
			}

			Exp exp2 = ParseExp ();

			if (exp2 != null) {
				Exp exp = Exp.buildConcat (exp1, exp2);
				return exp;
			}

			return exp1;
		}

		private Exp ParseRepExp() {
			Exp exp = ParseBasicExp ();

			if (exp == null) {
				return null;
			} 

			if (Cursor >= ExpStr.Length) {
				return exp;
			}

			if (ExpStr [Cursor] == '*') {
				Cursor += 1;
				exp = Exp.buildKleene (exp);
			}

			return exp;
		}

		private Exp ParseBasicExp() {
			if (Cursor >= ExpStr.Length) {
				return null;
			}

			char c = ExpStr [Cursor];

			if (InRange (c)) {
				Cursor += 1;
				return Exp.buildToken (c);
			} 

		    if (c == '\\') {
				Cursor += 1;
				if (!InRange (c)) {
					Cursor += 1;
					return Exp.buildToken (ExpStr [Cursor - 1]);
				}
				return null;
			} 

			if (c == '(') {
				Cursor += 1;
				Exp exp = ParseRegExp ();

				if (ExpStr [Cursor] == ')') {
					Cursor += 1;
					return exp;
				}
			}

			return null;
		}
			
		private void PrintAst(Exp ast, int level) {
			for (int i = 0; i < level; i++) {
				Console.Write ("  ");
			}

			if (ast.Type == Exp.ExpType.Token) {
				Console.Write ("(Token " + ast.C + ")");
				Console.WriteLine ();
				return;
			} else if (ast.Type == Exp.ExpType.Concat) {
				Console.Write ("(Concat ");
				Console.WriteLine ();
				PrintAst (ast.E1, level + 1);
				PrintAst (ast.E2, level + 1);
			} else if (ast.Type == Exp.ExpType.Alter) {
				Console.Write ("(Alter ");
				Console.WriteLine ();
				PrintAst (ast.E1, level + 1);
				PrintAst (ast.E2, level + 1);
			} else if (ast.Type == Exp.ExpType.Kleene) {
				Console.Write ("(Kleene ");
				Console.WriteLine ();
				PrintAst (ast.E1, level + 1);
			}

			for (int i = 0; i < level; i++) {
				Console.Write ("  ");
			}
			Console.Write (")");
			Console.WriteLine ();
		}
			
		public void Print() {
			PrintAst (Ast, 0);
		}
	}
}