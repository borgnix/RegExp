using System;
using System.Collections;

namespace regexp
{
	public class Exp {
		public enum ExpType {
			Token,
			Concat,
			Alter,
			Kleene,
		};

		public Exp() {}
		public char C { get; set; }
		public Exp E1 { get; set;}
		public Exp E2 { get; set;}
		public ExpType Type { get; set;}

		public static Exp buildToken(char c) {
			Exp exp = new Exp ();
			exp.Type = ExpType.Token;
			exp.C = c;
			return exp;
		}

		public static Exp buildConcat(Exp e1, Exp e2) {
			Exp exp = new Exp ();
			exp.Type = ExpType.Concat;
			exp.E1 = e1;
			exp.E2 = e2;
			return exp;
		}

		public static Exp buildAlter(Exp e1, Exp e2) {
			Exp exp = new Exp ();
			exp.Type = ExpType.Alter;
			exp.E1 = e1;
			exp.E2 = e2;
			return exp;
		}

		public static Exp buildKleene(Exp e) {
			Exp exp = new Exp ();
			exp.Type = ExpType.Kleene;
			exp.E1 = e;
			return exp;
		}
	}

	class Parser {
		public string ExpStr { get; }
		public Exp Ast { get; }

		private int Cursor = 0;

		public Parser (string regexp) {
			ExpStr = regexp;
			Ast = ParseRegExp ();
		}

		private bool InRange(char c) {
			return c != '\\' && c != '(' && c != ')' && c != '|' && c != '*';
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
				switch (ExpStr[Cursor]) {
				case '\\': 
				case '(':
				case ')':
				case '|':
				case '*':
					Cursor += 1;
					return Exp.buildToken(ExpStr[Cursor - 1]);
				default:
					return null;
				}
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
			
		private void PrintAst(Exp ast) {
			if (ast.Type == Exp.ExpType.Token) {
				Console.Write ("(Token " + ast.C + ")");
				Console.WriteLine ();
			} else if (ast.Type == Exp.ExpType.Concat) {
				Console.Write ("(Concat ");
				PrintAst (ast.E1);
				PrintAst (ast.E2);
				Console.Write (")");
				Console.WriteLine ();
			} else if (ast.Type == Exp.ExpType.Alter) {
				Console.Write ("(Alter ");
				PrintAst (ast.E1);
				PrintAst (ast.E2);
				Console.Write (")");
				Console.WriteLine ();
			} else if (ast.Type == Exp.ExpType.Kleene) {
				Console.Write ("(Kleene ");
				PrintAst (ast.E1);
				Console.Write(")");
				Console.WriteLine ();
			}
		}
			
		public void Print() {
			PrintAst (Ast);
		}
	}

}