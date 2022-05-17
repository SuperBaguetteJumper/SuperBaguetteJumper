using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Common {
	public class CsharpUtils {
		public static void FixCsharpBadDecimalSeparator() {
			CultureInfo customCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
			customCulture.NumberFormat.NumberDecimalSeparator = ".";
			Thread.CurrentThread.CurrentCulture = customCulture;
		}
	}

	public class PriorityQueue<T> {
		private SortedSet<T> data;

		public int Count => this.data.Count;
		public bool IsEmpty => this.Count == 0;

		public PriorityQueue(Comparer<T> comparer) {
			this.data = new SortedSet<T>(comparer);
		}

		public void Enqueue(T element) {
			this.data.Add(element);
		}

		public T Dequeue() {
			T element = this.Peek();
			this.data.Remove(element);
			return element;
		}

		public T Peek() => this.data.Min;
	}
}
