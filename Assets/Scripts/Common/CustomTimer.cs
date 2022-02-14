using System.Collections.Generic;
using UnityEngine;

namespace Common {
	public class CustomTimer : MonoBehaviour {
		private static Dictionary<string, CustomTimer> dicoCustomTimers;

		public bool IsRunning { get; private set; }

		public float TimeScale { get; set; }

		public float DeltaTime => this.IsRunning ? UnityEngine.Time.deltaTime * this.TimeScale : 0;

		public float FixedDeltaTime => this.IsRunning ? UnityEngine.Time.fixedDeltaTime * this.TimeScale : 0;

		public float Time { get; private set; }

		private void Awake() {
			if (dicoCustomTimers == null) {
				dicoCustomTimers = new Dictionary<string, CustomTimer>();
				CustomTimer[] customTimers = FindObjectsOfType<CustomTimer>();
				foreach (CustomTimer item in customTimers)
					dicoCustomTimers.Add(item.name, item);
			}
		}

		public void Reset() {
			this.Time = 0;
		}

		public void Reset(bool startTimer) {
			this.Time = 0;
			this.IsRunning = startTimer;
		}

		// Use this for initialization
		private void Start() {
			this.Reset(false);
		}

		// Update is called once per frame
		private void Update() {
			this.Time += this.DeltaTime;
		}

		public static CustomTimer GetCustomTimer(string name) {
			CustomTimer customTimer;
			dicoCustomTimers.TryGetValue(name, out customTimer);
			return customTimer;
		}

		public void StopTimer() {
			this.IsRunning = false;
		}

		public void StartTimer() {
			this.IsRunning = true;
		}

		public void ResetAndStart() {
			this.Reset(true);
		}

		public void ResetAndStop() {
			this.Reset(false);
		}
	}
}
