using System;
using System.Collections.Generic;

namespace regexp
{
	public class Nfa
	{
		private static char EPSILON = '\0';
		private State Start;
		private State Terminal;
		private Exp Ast;
		private List<State> States = new List<State>();

		public Nfa (Exp ast)
		{
			Ast = ast;
			var result = buildNfa (ast);
			Start = result.Item1;
			Terminal = result.Item2;
		}

		private State CreateState () {
			State state = new State (States.Count);
			States.Add (state);
			return state;
		}

		private void AddEdge(State from, State to, char c) {
			if (!from.To.ContainsKey(c))
			{
				from.To.Add (c, new List<State> ());
			}
			from.To [c].Add (to);
		}

		private Tuple<State, State> buildNfa(Exp ast) {
			State start = null;
			State terminal = null;

			if (ast.Type != Exp.ExpType.Concat) {
				start = CreateState();
				terminal = CreateState();
			}
				
			switch (ast.Type) {
			case Exp.ExpType.Token:
				{
					AddEdge (start, terminal, ast.C);
					break;
				}
			case Exp.ExpType.Concat:
				{
					var nfa1 = buildNfa (ast.E1);
					var nfa2 = buildNfa (ast.E2);
					start = nfa1.Item1;
					terminal = nfa2.Item2;

					AddEdge (nfa1.Item2, nfa2.Item1, EPSILON);
					break;
				}
			case Exp.ExpType.Alter:
				{
					var nfa1 = buildNfa (ast.E1);
					var nfa2 = buildNfa (ast.E2);
					AddEdge (start, nfa1.Item1, EPSILON);
					AddEdge (start, nfa2.Item1, EPSILON);
					AddEdge (nfa1.Item2, terminal, EPSILON);
					AddEdge (nfa2.Item2, terminal, EPSILON);
					break;
				}
			case Exp.ExpType.Kleene:
				{
					var nfa = buildNfa (ast.E1);
					AddEdge (start, terminal, EPSILON);
					AddEdge (start, nfa.Item1, EPSILON);
					AddEdge (nfa.Item2, nfa.Item1, EPSILON);
					AddEdge (nfa.Item2, terminal, EPSILON);
					break;
				}
			}
				
			return new Tuple<State, State>(start, terminal);
		}

		public bool match(string s) {
			var heads = new HashSet<State> () {Start};
			for (int cursor = 0; cursor != s.Length; cursor++) {
				if (heads.Count == 0) {
					return false;
				}

				char c = s [cursor];
				heads = EpsilonClosure (heads);
				var new_heads = new HashSet<State> ();

				foreach (var head in heads) {
					if (head.To.ContainsKey (c)) {
						new_heads.UnionWith (head.To [c]);
					}
				}

				heads = new_heads;
			}
			heads = EpsilonClosure (heads);
			foreach (var head in heads) {
				if (head == Terminal) {
					return true;
				}
			}
			return false;
		}

		private HashSet<State> EpsilonClosure(HashSet<State> states) {
			var new_states = new HashSet<State> (states);
			foreach (var state in states) {
				if (state.To.ContainsKey (EPSILON)) {
					new_states.UnionWith(state.To[EPSILON]);
				}
			}

			if (states.IsProperSubsetOf (new_states)) {
				return EpsilonClosure (new_states);
			}

			return new_states;
		}
	}
}

