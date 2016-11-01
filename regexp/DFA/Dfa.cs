using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace regexp
{
	public class Dfa
	{
		public Nfa Nfa { get; }

		private DfaState Start;
		private HashSet<DfaState> Terminals;
		private List<DfaState> States;

		private DfaState CreateState ()
		{
			DfaState state = new DfaState (States.Count);
			States.Add (state);
			return state;
		}

		public Dfa (Nfa nfa)
		{
			Terminals = new HashSet<DfaState> ();
			States = new List<DfaState> ();
			initDfa (nfa);
			Nfa = nfa;

		}

		public void initDfa (Nfa nfa)
		{
			List<HashSet<NfaState>> state_closures = new List<HashSet<NfaState>> ();
			Queue<int> closure_to_be_processed = new Queue<int> ();

			HashSet<NfaState> closure = nfa.EpsilonClosure (new HashSet<NfaState>{ nfa.Start });
			closure_to_be_processed.Enqueue (state_closures.Count);
			state_closures.Add (closure);

			Start = CreateState ();
			DfaState cur_state = Start;

			while (closure_to_be_processed.Count != 0) {
				int id = closure_to_be_processed.Dequeue ();
				closure = state_closures [id];

				cur_state = States [id];

				HashSet<char> chars = nfa.TransitionChars (closure);

				foreach (var c in chars) {
					if (c != Nfa.EPSILON) {
						HashSet<NfaState> new_closure = nfa.Closure (closure, c);
						new_closure = nfa.EpsilonClosure (new_closure);

						if (!state_closures.Exists (
							    x => x.SetEquals (new_closure))) {
							closure_to_be_processed.Enqueue (state_closures.Count);
							state_closures.Add (new_closure);

							DfaState new_state = CreateState ();
							cur_state.AddEdge (new_state, c);

							if (new_closure.Contains (nfa.Terminal)) {
								Terminals.Add (new_state);
							}
						} else {
							int index = state_closures.FindIndex (x => x.SetEquals (new_closure));
							cur_state.AddEdge (States [index], c);
						}
					}
				}
			}
		}


		public bool Match (string s)
		{
			int cursor = 0;
			DfaState head = Start;

			while (cursor != s.Length) {
				char c = s [cursor];
				head = head.EdgeTo (c);
				if (head == null) {
					return false;
				}
				cursor++;
			}

			if (Terminals.Contains (head)) {
				return true;
			}

			return false;
		}
			
		public void MinimizeDfa() {
			var equivalence_classes = States
				.GroupBy (state => Terminals.Contains (state))
				.Select (gp => gp.ToList ())
				.Select(lst => new HashSet<DfaState>(lst))
				.ToList ();
			var classes_to_be_processed = new Queue<HashSet<DfaState>> ();

			equivalence_classes.ForEach (cls => classes_to_be_processed.Enqueue (cls));

			HashSet<DfaState> head;
			while(classes_to_be_processed.Count != 0) {
				head = classes_to_be_processed.Dequeue ();
				var chars = head
					.Select (x => x.To.Keys.ToList ())
					.SelectMany (i => i)
					.Distinct ();
				
				foreach (var c in chars) {
					var clses = head
						.GroupBy (state => equivalence_classes.FindIndex (ec => ec.Contains (state.EdgeTo (c))))
						.Select (gp => gp.ToList ())
						.Select (lst => new HashSet<DfaState> (lst))
						.Distinct ()
						.ToList ();
				
					if (clses.Count > 1) {
						equivalence_classes.Remove (head);
						equivalence_classes.AddRange (clses);

						clses.ForEach (cls => classes_to_be_processed.Enqueue (cls));
						break;
					}
				}
			}

			var old_terminals = Terminals;
			var old_states = States;

			States = new List<DfaState> ();
			Terminals = new HashSet<DfaState> ();

			equivalence_classes.ForEach (_ => States.Add (new DfaState (States.Count)));
			foreach (var state in States) {
				var cls = equivalence_classes [state.DfaID];
				foreach (var old_state in cls) {
					foreach (var c in old_state.To.Keys) {
						var old_to = old_state.EdgeTo (c);
						int ni = equivalence_classes.FindIndex(ec => ec.Contains(old_to));
						state.AddEdge(States[ni], c);
					}

					if(old_terminals.Contains(old_state)) {
						Terminals.Add(state);
					}

					if (old_state == Start) {
						Start = state;
					}
				}
			}

		}

		public void dump(string filename) {
			using (StreamWriter file
				= File.CreateText (filename)) 
			{
				file.WriteLine ("digraph Dfa {");
				foreach (var state in States) {
					var id = state.DfaID;
					foreach (var tc in state.To.Keys) {
						var to_id = state.EdgeTo (tc).DfaID;
						file.WriteLine ("  " + id + " -> " + to_id + " [label = \"" + tc + "\"]");
					}
				}
				file.WriteLine ("}");

			};

		}
	}
}

