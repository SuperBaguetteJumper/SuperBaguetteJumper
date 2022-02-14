using UnityEngine;
using UnityEngine.UI;

namespace Common {
	public class PlaySoundOnClick : MonoBehaviour {
		[SerializeField] private string soundName;

		// Use this for initialization
		private void Start() {
			var button = this.GetComponent<Button>();
			if (button)
				button.onClick.AddListener(this.PlaySound);
		}

		private void PlaySound() {
			if (SfxManager.Instance)
				SfxManager.Instance.PlaySfx2D(this.soundName);
		}
	}
}
