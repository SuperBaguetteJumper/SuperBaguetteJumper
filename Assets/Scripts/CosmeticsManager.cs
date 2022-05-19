using System;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class CosmeticsManager : MonoBehaviour {
	[field: SerializeField] public List<Cosmetic> Cosmetics { get; private set; }

	private List<GameObject> instances = new List<GameObject>();
	private List<GameObject> previewInstances = new List<GameObject>();

	private void Awake() {
		EventManager.Instance.AddListener<PlayerLoadedEvent>(this.OnPlayerLoaded);
		EventManager.Instance.AddListener<CosmeticUnlockedEvent>(this.OnCosmeticUnlocked);
		EventManager.Instance.AddListener<CosmeticPreviewBeganEvent>(this.OnCosmeticPreviewBegan);
		EventManager.Instance.AddListener<CosmeticPreviewEndedEvent>(this.OnCosmeticPreviewEnded);
	}

	private void OnDestroy() {
		for (var i = 0; i < this.instances.Count; i++)
			Destroy(this.instances[i]);
		this.instances.Clear();
		EventManager.Instance.RemoveListener<PlayerLoadedEvent>(this.OnPlayerLoaded);
		EventManager.Instance.RemoveListener<CosmeticUnlockedEvent>(this.OnCosmeticUnlocked);
		EventManager.Instance.RemoveListener<CosmeticPreviewBeganEvent>(this.OnCosmeticPreviewBegan);
		EventManager.Instance.RemoveListener<CosmeticPreviewEndedEvent>(this.OnCosmeticPreviewEnded);
	}

	private void OnPlayerLoaded(PlayerLoadedEvent e) {
		for (int i = 0; i < this.Cosmetics.Count; i++) {
			Cosmetic cosmetic = this.Cosmetics[i];
			if (cosmetic.IsUnlocked)
				this.instances.Add(Instantiate(cosmetic.Model, e.Player.transform, false));
		}
	}

	private void OnCosmeticUnlocked(CosmeticUnlockedEvent e) {
		e.Cosmetic.Unlock();
		this.instances.Add(Instantiate(e.Cosmetic.Model, e.Player.transform, false));
	}

	private void OnCosmeticPreviewBegan(CosmeticPreviewBeganEvent e) {
		this.previewInstances.Add(Instantiate(e.Cosmetic.Model, e.Player.transform, false));
	}

	private void OnCosmeticPreviewEnded(CosmeticPreviewEndedEvent e) {
		string instance = e.Cosmetic.Model.name + "(Clone)";
		for (int i = 0; i < this.previewInstances.Count; i++) {
			GameObject preview = this.previewInstances[i];
            if (preview == null || preview.name == instance) {
	            Destroy(preview);
	            this.previewInstances.RemoveAt(i--);
            }
		}
	}
}

[Serializable]
public class Cosmetic {
	[field: SerializeField] public GameObject Model { get; private set; }
	[field: SerializeField] public string Name { get; private set; }
	[field: SerializeField] public int Cost { get; private set; }

	private string id => "Cosmetic." + this.Model.name;
	public bool IsUnlocked => PlayerPrefs.HasKey(this.id);
	public DateTimeOffset UnlockDate => DateTimeOffset.FromUnixTimeSeconds(PlayerPrefs.GetInt(this.id, 0));

	public void Unlock() {
		PlayerPrefs.SetInt(this.id, (int) DateTimeOffset.Now.ToUnixTimeSeconds());
	}
}
