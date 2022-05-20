using System.Collections;
using UnityEngine;

namespace Platforms {
	public class TemporaryPlatform : MonoBehaviour {
		[SerializeField] private float lifespanAfterActivation = 1;
		[SerializeField] private float cooldownBeforeRespawn = 5;
		[SerializeField] private GameObject gfx;
		[SerializeField] private GameObject hologram;

		private new BoxCollider collider;
		private PlatformPressurePlate pressurePlate;

		private void Awake() {
			this.collider = this.GetComponent<BoxCollider>();
			this.pressurePlate = this.GetComponentInChildren<PlatformPressurePlate>();
		}

		private IEnumerator Start() {
			while (this.enabled) {
				yield return new WaitUntil(() => this.pressurePlate.IsActivated);
                yield return new WaitForSeconds(this.lifespanAfterActivation);
                this.SetHidden(true);
                yield return new WaitForSeconds(this.cooldownBeforeRespawn);
                this.SetHidden(false);
				this.pressurePlate.Deactivate();
			}
		}

		private void SetHidden(bool hidden) {
			this.collider.enabled = !hidden;
			this.gfx.SetActive(!hidden);
			this.hologram.SetActive(hidden);
		}
	}
}
