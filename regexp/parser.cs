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
		public static bool InRange(char c) {
			return c != '\\' && c != '(' && c != ')' && c != '|' && c != '*';
		}
			
		/* Grammar
		 * regexp := <exp> '|' <regexp> | <exp>
		 * exp := <repexp> <exp> | <repexp>
		 * repexp := <basicexp> * | <basicexp>
		 * basicexp := <alnum> | ( <regexp> )
		 */

		public static Tuple<int,Exp> parseRegExp(string expstr, int idx) {
			var first = parseExp (expstr, idx);
			int idx1 = first.Item1;
			Exp exp1 = first.Item2;

			if (idx1 >= expstr.Length) {
				return first;
			}

			Tuple<int, Exp> result = null;

			if (expstr [idx1] == '|') {
				var second = parseRegExp (expstr, idx1+1);
				int idx2 = second.Item1;
				Exp exp2 = second.Item2;

				Exp exp = Exp.buildAlter (exp1, exp2);

				result = new Tuple<int, Exp> (idx2, exp);
			} else {
				result = new Tuple<int, Exp> (idx1, exp1);
			}

			return result;
		}

		public static Tuple<int, Exp> parseExp(string expstr, int idx) {
			var first = parseRepExp (expstr, idx);
			if (first == null) {
				return null;
			}
			int idx1 = first.Item1;
			Exp exp1 = first.Item2;

			if (idx1 >= expstr.Length) {
				return first;
			}
			var rest = parseExp (expstr, idx1);

			Tuple<int, Exp> result = null;
			if (rest != null) {
				int idx2 = rest.Item1;
				Exp exp2 = rest.Item2;

				Exp exp = Exp.buildConcat (exp1, exp2);
				result = new Tuple<int, Exp> (idx2, exp);
			} else {
				result = new Tuple<int, Exp> (idx1, exp1);
			}

			return result;
		}

		public static Tuple<int, Exp> parseRepExp(string expstr, int idx) {
			var first = parseBasicExp (expstr, idx);


			if (first == null) {
				return null;
			} else {
				int idx1 = first.Item1;
				Exp exp1 = first?.Item2;

				if (idx1 >= expstr.Length) {
					return first;
				}

				if (expstr [idx1] == '*') {
					Exp exp = Exp.buildKleene (exp1);
					return new Tuple<int, Exp> (idx1 + 1, exp);
				} else {
					return first;
				}
			}
		}

		public static Tuple<int, Exp> parseBasicExp(string expstr, int idx) {
			if (idx >= expstr.Length) {
				return null;
			}

			char c = expstr [idx];
			if (InRange (c)) {
				return new Tuple<int, Exp> (idx + 1, Exp.buildToken (c));
			} else if (c == '\\') {
				switch (expstr[idx + 1]) {
				case '\\': 
				case '(':
				case ')':
				case '|':
				case '*':
					return new Tuple<int, Exp> (idx + 1, Exp.buildToken(expstr[idx + 1]));
				default:
					return null;
				}
			} if (c == '(') {
				var regexp = parseRegExp (expstr, idx + 1);
				int idx1 = regexp.Item1;
				Exp exp1 = regexp.Item2;

				if (expstr [idx1] == ')') {
					return new Tuple<int, Exp> (idx1 + 1, exp1);
				}
			}

			return null;
		}

		Exp parse(String expstr) {
			var result = parseRegExp (expstr, 0);
			return result.Item2;
		}

		public static void PrintAst(Exp ast) {
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
			

	}

}