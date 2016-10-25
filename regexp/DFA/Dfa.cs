using System;
using System.Collections.Generic;

namespace regexp
{
	public class Dfa
	{
		public Nfa Nfa { get; }

		private DfaState Start;
		private List<DfaState> Terminals;
		private List<DfaState> States;

		private DfaState CreateState ()
		{
			DfaState state = new DfaState (States.Count);
			States.Add (state);
			return state;
		}

		public Dfa (Nfa nfa)
		{
			Terminals = new List<DfaState> ();
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
							States.Add (new_state);
							cur_state.AddEdge (new_state, c);

							if (closure.Contains (nfa.Terminal)) {
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
	}
}

