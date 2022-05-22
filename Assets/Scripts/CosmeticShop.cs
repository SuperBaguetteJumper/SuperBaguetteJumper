using System;
using Events;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CosmeticShop : MonoBehaviour {
	[HideInInspector] public int CosmeticID;
	[SerializeField] private Transform instanceContainer;
	[SerializeField] private Text[] nameTexts;
	[SerializeField] private Text[] priceTexts;
	[SerializeField] private GameObject buyOverlay;
	[SerializeField] private Button confirmBuy;
	[SerializeField] private Button cancelBuy;
	[SerializeField] private Text statusText;

	[field: NonSerialized]
	public Cosmetic Cosmetic { get; set; }
	private int firstPersonHiddenLayer;
	private new Collider collider;
	private Player player;

	private static bool ShopOpen;

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
		this.collider = this.GetComponent<Collider>();
		this.player = FindObjectOfType<Player>();
		this.confirmBuy.onClick.AddListener(this.Buy);
		this.cancelBuy.onClick.AddListener(this.CloseBuyOverlay);
	}

	private void Update() {
		if (!ShopOpen && Input.GetButtonDown("Fire1")) {
			if (this.collider.bounds.Contains(this.player.LookPosition))
				this.OpenBuyOverlay();
			else {
				RaycastHit hit;
				if (Physics.Raycast(this.player.LookPosition, this.player.LookDirection, out hit, 5) && hit.collider == this.collider)
					this.OpenBuyOverlay();
			}
		}
		if (ShopOpen && Input.GetButtonDown("Cancel"))
			this.CloseBuyOverlay();
	}

	private void Buy() {
		MoneyWithdrawEvent e = new MoneyWithdrawEvent(this.Cosmetic.Cost);
		EventManager.Instance.Raise(e);
		if (e.Success) {
			EventManager.Instance.Raise(new CosmeticUnlockedEvent(this.Cosmetic, this.player));
			this.CloseBuyOverlay();
		}
	}

	private void OpenBuyOverlay() {
		ShopOpen = true;
		bool canUnlock = !this.Cosmetic.IsUnlocked;
		bool canAfford = GameManager.Instance.Money >= this.Cosmetic.Cost;
		this.confirmBuy.interactable = canUnlock && canAfford;
		if (canUnlock)
			this.statusText.text = canAfford ? "Achetable !" : "Pas assez d'argent";
		else
			this.statusText.text = "Débloqué le " + this.Cosmetic.UnlockDate.ToString("dd/MM/yyyy' à 'HH:mm:ss");
		this.buyOverlay.SetActive(true);
		GameManager.UnlockCursor();
		this.player.ViewLocked = true;
		this.player.CanMove = false;
	}

	private void CloseBuyOverlay() {
		ShopOpen = false;
		this.buyOverlay.SetActive(false);
		GameManager.LockCursor();
		this.player.ViewLocked = false;
		this.player.CanMove = true;
	}

	public void RefreshModel() {
		if (this.Cosmetic == null || this.instanceContainer == null
#if UNITY_EDITOR
		                          || PrefabStageUtility.GetCurrentPrefabStage() != null
#endif
		                          || this.instanceContainer.gameObject.scene.name == null)
			return;
        this.ClearModel();
        GameObject model = Instantiate(this.Cosmetic.Model, this.instanceContainer);
        foreach (Transform child in model.transform.GetComponentsInChildren<Transform>(true))
	        child.gameObject.layer &= ~this.firstPersonHiddenLayer;
        for (var i = 0; i < this.nameTexts.Length; i++)
	        this.nameTexts[i].text = this.Cosmetic.Name;
        for (var i = 0; i < this.priceTexts.Length; i++)
	        this.priceTexts[i].text = this.Cosmetic.Cost.ToString();
	}

	private void ClearModel() {
		foreach (Transform child in this.instanceContainer)
			DestroyImmediate(child.gameObject);
	}

	private void OnDestroy() {
		this.ClearModel();
		this.confirmBuy.onClick.RemoveListener(this.Buy);
		this.cancelBuy.onClick.RemoveListener(this.CloseBuyOverlay);
	}

	private void OnTriggerEnter(Collider other) {
		if (this.Cosmetic == null)
			return;
		Player p = other.gameObject.GetComponent<Player>();
		if (p != null)
			EventManager.Instance.Raise(new CosmeticPreviewBeganEvent(this.Cosmetic, p));
	}

	private void OnTriggerExit(Collider other) {
		if (this.Cosmetic == null)
			return;
		Player p = other.gameObject.GetComponent<Player>();
		if (p != null)
			EventManager.Instance.Raise(new CosmeticPreviewEndedEvent(this.Cosmetic, p));
	}
}
