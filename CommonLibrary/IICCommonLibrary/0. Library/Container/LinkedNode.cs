using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class LinkedNode<T>
	{
		private T _value;

		private LinkedNode<T> _next = null;

		public LinkedNode()
		{
		}

		public LinkedNode(T value)
		{
			_value = value;
		}

		public T Value
		{
			set { _value = value; }
			get { return _value; }
		}

		public LinkedNode<T> Next
		{
			set { _next = value; }
			get { return _next; }
		}

		public void AddNext(T value)
		{
			LinkedNode<T> p = this;
			while (p.Next != null)
				p = p.Next;

			p.Next = new LinkedNode<T>(value);
		}

		public IEnumerable<T> GetValues()
		{
			LinkedNode<T> p = this;
			while (p != null) {
				yield return p.Value;
				p = p.Next;
			}
		}
	}
}
