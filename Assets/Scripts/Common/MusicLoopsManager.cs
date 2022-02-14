using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common {
	/// <summary>
	///     Music loops manager.
	///     Gestion de boucles musicales avec Fade-In/Fade-Out entre 2 boucles
	/// </summary>
	public class MusicLoopsManager : Singleton<MusicLoopsManager> {
		[Header("MusicLoopsManager")]
		[SerializeField] private List<AudioClip> clips = new List<AudioClip>();
		[SerializeField] private float fadeDuration;
		[SerializeField] private bool showGui;

		private AudioSource[] audioSources;

		private int currClipIndex;

		private int indexFadeIn;
		private readonly float[] maxVolumes = new float[2];

		protected override void Awake() {
			base.Awake();

			this.indexFadeIn = 0;

			this.audioSources = this.GetComponents<AudioSource>();
			if (this.audioSources.Length != 2)
				Debug.LogError("MusicLoopsManager needs 2 AudioSource to work properly!");

			for (int i = 0; i < this.audioSources.Length; i++)
				this.maxVolumes[i] = this.audioSources[i].volume;
			// this.audioSources[i].clip = this.clips[i];
		}

		private void OnGUI() {
			if (!this.showGui)
				return;

			GUILayout.BeginArea(new Rect(Screen.width / 2 - 210, 10, 200, Screen.height));

			GUILayout.Label("MUSIC LOOPS MANAGER");
			GUILayout.Space(20);
			for (var i = 0; i < this.clips.Count; i++)
				if (GUILayout.Button("PLAY " + this.clips[i].name))
					this.PlayMusic(i);
			GUILayout.Space(20);
			if (GUILayout.Button("PLAY CURRENT MUSIC"))
				this.PlayCurrentMusic();

			if (GUILayout.Button("PLAY NEXT MUSIC"))
				this.PlayNextMusic();

			if (GUILayout.Button("STOP ALL - FADEOUT"))
				this.StopAll(0);

			if (GUILayout.Button("STOP ALL - NO FADEOUT"))
				this.StopAllRightAway();

			GUILayout.EndArea();
		}

		private IEnumerator FadeOutAndStopAll(float delay) {
			yield return new WaitForSeconds(delay + .1f); // Unity bug possiblement si la durée d'attente est nulle... on ajoute 0,1 pour que cette durée ne soit jamais véritablement nulle
			float elapsedTime = 0;

			while (elapsedTime < this.fadeDuration) {
				float k = elapsedTime / this.fadeDuration;
				this.audioSources[this.indexFadeIn].volume = Mathf.Lerp(0, this.maxVolumes[this.indexFadeIn], 1 - k); // Fade out 1st audiosource
				this.audioSources[1 - this.indexFadeIn].volume = Mathf.Lerp(0, this.maxVolumes[1 - this.indexFadeIn], 1 - k); // Fade out 2nd audiosource
				elapsedTime += Time.timeScale != 0 ? Time.deltaTime : 1 / 60f;
				yield return null;
			}

			this.audioSources[this.indexFadeIn].volume = 0;
			this.audioSources[this.indexFadeIn].Stop();
			this.audioSources[1 - this.indexFadeIn].volume = 0;
			this.audioSources[1 - this.indexFadeIn].Stop();
		}

		private IEnumerator FadeCoroutine() {
			float elapsedTime = 0;
			while (elapsedTime < this.fadeDuration) {
				float k = elapsedTime / this.fadeDuration;
				this.audioSources[this.indexFadeIn].volume = Mathf.Lerp(0, this.maxVolumes[this.indexFadeIn], k); // Fade in 1st audiosource
				this.audioSources[1 - this.indexFadeIn].volume = Mathf.Lerp(0, this.maxVolumes[1 - this.indexFadeIn], 1 - k); // Fade out 2nd audiosource
				elapsedTime += Time.timeScale != 0 ? Time.deltaTime : 1 / 60f;
				yield return null;
			}

			this.audioSources[this.indexFadeIn].volume = Mathf.Lerp(0, this.maxVolumes[this.indexFadeIn], 1);
			this.audioSources[1 - this.indexFadeIn].volume = Mathf.Lerp(0, this.maxVolumes[1 - this.indexFadeIn], 0);
			this.audioSources[1 - this.indexFadeIn].Stop();
		}

		public void PlayMusic(int index, bool fade = true) {
			this.currClipIndex = index % this.clips.Count;
			if (fade) {
				this.audioSources[1 - this.indexFadeIn].clip = this.clips[this.currClipIndex];
				this.indexFadeIn = 1 - this.indexFadeIn;
				this.StartCoroutine(this.FadeCoroutine());

				float currentTimeScale = Time.timeScale;
				Time.timeScale = 1;
				this.audioSources[this.indexFadeIn].Play();
				Time.timeScale = currentTimeScale;
			}
		}

		public void PlayCurrentMusic() {
			if (!FlagsManager.Instance || FlagsManager.Instance.GetFlag("SETTINGS_MUSIC", true))
				this.PlayMusic(this.currClipIndex);
		}

		public void PlayNextMusic() {
			if (!FlagsManager.Instance || FlagsManager.Instance.GetFlag("SETTINGS_MUSIC", true))
				this.PlayMusic(this.currClipIndex + 1);
		}

		public void StopAll(float delay) {
			Debug.Log("InGameMusicManager StopAll(" + delay + ")");
			this.StartCoroutine(this.FadeOutAndStopAll(delay));
		}

		public void StopAllRightAway() {
			this.StopAllCoroutines();
			this.audioSources[this.indexFadeIn].volume = 0;
			this.audioSources[1 - this.indexFadeIn].volume = 0;
			this.audioSources[1 - this.indexFadeIn].Stop();
			this.audioSources[this.indexFadeIn].Stop();
		}
	}
}
