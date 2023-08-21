using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BanhMy.Tools
{
	public class BMStateMachine<T> where T : struct, IComparable, IConvertible, IFormattable
	{
		public event Action OnStateChanged;

		public T CurrentState { get; private set; }
		public T PreviousState { get; private set; }

		public void ChangeState(T newState)
		{
			if (EqualityComparer<T>.Default.Equals(newState, CurrentState))
			{
				return;
			}

			PreviousState = CurrentState;
			CurrentState = newState;

			OnStateChanged?.Invoke();
		}

		public void RestorePreviousState()
		{
			CurrentState = PreviousState;
			
			OnStateChanged?.Invoke();
		}
	}
}
