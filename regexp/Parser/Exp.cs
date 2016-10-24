using System;

namespace regexp
{
	public class Exp
	{
		public enum ExpType
		{
			Token,
			Concat,
			Alter,
			Kleene,
		};

		public Exp ()
		{
		}

		public char C { get; set; }

		public Exp E1 { get; set; }

		public Exp E2 { get; set; }

		public ExpType Type { get; set; }

		public static Exp buildToken (char c)
		{
			Exp exp = new Exp ();
			exp.Type = ExpType.Token;
			exp.C = c;
			return exp;
		}

		public static Exp buildConcat (Exp e1, Exp e2)
		{
			Exp exp = new Exp ();
			exp.Type = ExpType.Concat;
			exp.E1 = e1;
			exp.E2 = e2;
			return exp;
		}

		public static Exp buildAlter (Exp e1, Exp e2)
		{
			Exp exp = new Exp ();
			exp.Type = ExpType.Alter;
			exp.E1 = e1;
			exp.E2 = e2;
			return exp;
		}

		public static Exp buildKleene (Exp e)
		{
			Exp exp = new Exp ();
			exp.Type = ExpType.Kleene;
			exp.E1 = e;
			return exp;
		}
	}
}