using System;
using System.Collections.Generic;
using System.IO;

namespace regexp
{
	public class Nfa
	{
		public static char EPSILON = '\0';

		public NfaState Start { get; }

		public NfaState Terminal { get; }

		public Exp Ast { get; }

		public List<NfaState> States { get; }

		public Nfa (Exp ast)
		{
			States = new List<NfaState> ();
			Ast = ast;
			var result = buildNfa (ast);
			Start = result.Item1;
			Terminal = result.Item2;
		}

		private NfaState CreateState ()
		{
			NfaState state = new NfaState (States.Count);
			States.Add (state);
			return state;
		}

		private void AddEdge (NfaState from, NfaState to, char c)
		{
			if (!from.To.ContainsKey (c)) {
				from.To.Add (c, new List<NfaState> ());
			}
			from.To [c].Add (to);
		}

		private Tuple<NfaState, NfaState> buildNfa (Exp ast)
		{
			NfaState start = null;
			NfaState terminal = null;

			if (ast.Type != Exp.ExpType.Concat) {
				start = CreateState ();
				terminal = CreateState ();
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
				
			return new Tuple<NfaState, NfaState> (start, terminal);
		}

		public bool Match (string s)
		{
			var heads = new HashSet<NfaState> () { Start };
			for (int cursor = 0; cursor != s.Length; cursor++) {
				if (heads.Count == 0) {
					return false;
				}

				char c = s [cursor];
				heads = EpsilonClosure (heads);
				var new_heads = new HashSet<NfaState> ();

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

		public HashSet<char> TransitionChars (HashSet<NfaState> states)
		{
			HashSet<char> chars = new HashSet<char> ();
			foreach (var state in states) {
				chars.UnionWith (state.To.Keys);
			}
			return chars;
		}

		public HashSet<NfaState> Closure (HashSet<NfaState> states, char transitionChar)
		{
			var new_states = new HashSet<NfaState> ();
			foreach (var state in states) {
				if (state.To.ContainsKey (transitionChar)) {
					new_states.UnionWith (state.To [transitionChar]);
				}
			}

			return new_states;
		}

		public HashSet<NfaState> EpsilonClosure (HashSet<NfaState> states)
		{
			var new_states = new HashSet<NfaState> (states);
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

		public void dump(string filename) {
			using (StreamWriter file
				= File.CreateText (filename)) 
			{
				file.WriteLine ("digraph Nfa {");
				foreach (var state in States) {
					var id = state.StateID;
					foreach (var tc in state.To.Keys) {
						var to_state_list = state.To[tc];
						string tran_char = tc == EPSILON ? "eps" : tc.ToString();

						foreach (var to_state in to_state_list) {
							file.WriteLine ("  " + id + " -> " + to_state.StateID + " [label = \"" + tran_char + "\"]");
						}
					}
				}
				file.WriteLine ("}");

			};

		}
	}
}

