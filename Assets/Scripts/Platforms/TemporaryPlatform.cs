using System.Collections;
using UnityEngine;

namespace Platforms {
	public class TemporaryPlatform : PressurePlatePlatform {
		[SerializeField] private float lifespanAfterActivation = 1;
		[SerializeField] private float cooldownBeforeRespawn = 5;
		[SerializeField] private GameObject gfx;
		[SerializeField] private GameObject hologram;

		private new BoxCollider collider;

		private void Awake() {
			this.collider = this.GetComponent<BoxCollider>();
		}

		private IEnumerator Start() {
			while (this.enabled) {
				yield return new WaitUntil(() => this.PressurePlate.IsActivated);
                yield return new WaitForSeconds(this.lifespanAfterActivation);
                this.SetHidden(true);
                yield return new WaitForSeconds(this.cooldownBeforeRespawn);
                this.SetHidden(false);
				this.PressurePlate.Deactivate();
			}
		}

		private void SetHidden(bool hidden) {
			this.collider.enabled = !hidden;
			this.gfx.SetActive(!hidden);
			this.hologram.SetActive(hidden);
		}
	}
}
