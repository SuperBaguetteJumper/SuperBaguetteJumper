using System.Collections;
using UnityEngine;

namespace Common {
	public abstract class Manager<T> : SingletonGameStateObserver<T> where T : Component {
		public bool IsReady { get; private set; }

		// Use this for initialization
		protected virtual IEnumerator Start() {
			IsReady = false;
			yield return this.StartCoroutine(this.InitCoroutine());
			IsReady = true;
		}

		protected abstract IEnumerator InitCoroutine();
	}
}
