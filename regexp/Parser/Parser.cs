using System;
using System.Collections.Generic;

namespace regexp
{
	class Parser {
		private static HashSet<Char> SpecialTokens = new HashSet<Char> {'\\', '(', ')', '|', '*'};
		private static HashSet<Char> EscapeTokens = new HashSet<Char> {'n', 't'};

		private Stack<char> Symbols;

		public string ExpStr { get; }
		public Exp Ast { get; }

		private int Cursor = 0;

		public Parser (string regexp) {
			Symbols = new Stack<char> ();
			ExpStr = regexp;
			Ast = ParseRegExp ();
			if (Symbols.Count != 0) {
				throw new ParseAbortedExcetpion ("Parenthesis doesn't match: " + Symbols.Peek());
			}
		}

		private static bool InRange(char c) {
			return !Parser.SpecialTokens.Contains (c);
		}

		private static bool EscapeToken(char c) {
			return EscapeTokens.Contains (c) || SpecialTokens.Contains (c);
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

				if (exp2 == null) {
					throw new ParseAbortedExcetpion ("Expected expression after '|' ");
				}
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

			if (c == '*') {
				throw new ParseAbortedExcetpion ("Unexpected Kleene star At " + Cursor);
			}

		    if (c == '\\') {
				Cursor += 1;
				if (EscapeToken (c)) {
					Cursor += 1;
					return Exp.buildToken (ExpStr [Cursor - 1]);
				}
				throw new ParseAbortedExcetpion ("Unexpected Escape Token: " + ExpStr[Cursor] + " At " + Cursor.ToString());
			} 

			if (c == '(') {
				Symbols.Push ('(');
				Cursor += 1;
				Exp exp = ParseRegExp ();

				if (exp == null) {
					throw new ParseAbortedExcetpion ("Empty Group: At " + Cursor.ToString());
				}

				if (Cursor >= ExpStr.Length) {
					return null;
				}

				if (ExpStr [Cursor] == ')') {
					if (Symbols.Peek() == '(') {
						Symbols.Pop();
					}
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
				Console.WriteLine ("(Token " + ast.C + ")");
				return;
			} else if (ast.Type == Exp.ExpType.Concat) {
				Console.WriteLine ("(Concat ");
				PrintAst (ast.E1, level + 1);
				PrintAst (ast.E2, level + 1);
			} else if (ast.Type == Exp.ExpType.Alter) {
				Console.WriteLine ("(Alter ");
				PrintAst (ast.E1, level + 1);
				PrintAst (ast.E2, level + 1);
			} else if (ast.Type == Exp.ExpType.Kleene) {
				Console.WriteLine ("(Kleene ");
				PrintAst (ast.E1, level + 1);
			}

			for (int i = 0; i < level; i++) {
				Console.Write ("  ");
			}
			Console.WriteLine (")");
		}
			
		public void Print()  {
			PrintAst (Ast, 0);
		}
	}
}