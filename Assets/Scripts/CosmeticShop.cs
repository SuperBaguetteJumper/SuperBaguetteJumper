using System;
using Events;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CosmeticShop : MonoBehaviour {
	[HideInInspector] public int CosmeticID;
	[SerializeField] private Transform instanceContainer;
	[SerializeField] private Text nameText;
	[SerializeField] private Text priceText;

	[field: NonSerialized]
	public Cosmetic Cosmetic { get; set; }
	private int firstPersonHiddenLayer;

	private void Start() {
		this.firstPersonHiddenLayer = LayerMask.NameToLayer("First Person Hidden");
		CosmeticsManager manager = FindObjectOfType<CosmeticsManager>();
		if (manager != null && this.CosmeticID >= 0 && this.CosmeticID < manager.Cosmetics.Count) {
			this.Cosmetic = manager.Cosmetics[this.CosmeticID];
			this.RefreshModel();
		} else {
			Debug.LogWarning($"Invalid cosmetic shop: {this.name} (cosmetic ID #{this.CosmeticID} not found). Destroying!");
			Destroy(this.gameObject);
		}
	}

	public void RefreshModel() {
		if (this.Cosmetic == null || this.instanceContainer == null || PrefabStageUtility.GetCurrentPrefabStage() != null || this.instanceContainer.gameObject.scene.name == null)
			return;
        this.ClearModel();
        GameObject model = Instantiate(this.Cosmetic.Model, this.instanceContainer);
        foreach (Transform child in model.transform.GetComponentsInChildren<Transform>(true))
	        child.gameObject.layer &= ~this.firstPersonHiddenLayer;
		this.nameText.text = this.Cosmetic.Name;
		this.priceText.text = this.Cosmetic.Cost.ToString();
	}

	private void ClearModel() {
		foreach (Transform child in this.instanceContainer)
			DestroyImmediate(child.gameObject);
	}

	private void OnDestroy() {
		this.ClearModel();
	}

	private void OnTriggerEnter(Collider other) {
		if (this.Cosmetic == null)
			return;
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null)
			EventManager.Instance.Raise(new CosmeticPreviewBeganEvent(this.Cosmetic, player));
	}

	private void OnTriggerExit(Collider other) {
		if (this.Cosmetic == null)
			return;
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null)
			EventManager.Instance.Raise(new CosmeticPreviewEndedEvent(this.Cosmetic, player));
	}
}
