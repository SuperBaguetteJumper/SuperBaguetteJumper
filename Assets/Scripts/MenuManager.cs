using System.Collections;
using System.Collections.Generic;
using Common;
using Events;
using UnityEngine;

public class MenuManager : Manager<MenuManager> {
	#region Manager implementation

	protected override IEnumerator InitCoroutine() {
		yield break;
	}

	#endregion

	[Header("MenuManager")]

	#region Panels

	[Header("Panels")]
	[SerializeField] private GameObject panelMainMenu;

	[SerializeField] private GameObject panelInGameMenu;
	[SerializeField] private GameObject panelGameOver;

	private List<GameObject> allPanels;

	#endregion

	#region Events' subscription

	#endregion

	#region Monobehaviour lifecycle

	protected override void Awake() {
		base.Awake();
		this.RegisterPanels();
	}

	private void Update() {
		if (Input.GetButtonDown("Cancel"))
			this.EscapeButtonHasBeenClicked();
	}

	#endregion

	#region Panel Methods

	private void RegisterPanels() {
		this.allPanels = new List<GameObject>();
		this.allPanels.Add(this.panelMainMenu);
		this.allPanels.Add(this.panelInGameMenu);
		this.allPanels.Add(this.panelGameOver);
	}

	private void OpenPanel(GameObject panel) {
		foreach (GameObject item in this.allPanels)
			if (item)
				item.SetActive(item == panel);
	}

	#endregion

	#region UI OnClick Events

	public void EscapeButtonHasBeenClicked() {
		EventManager.Instance.Raise(new EscapeButtonClickedEvent());
	}

	public void PlayButtonHasBeenClicked() {
		EventManager.Instance.Raise(new PlayButtonClickedEvent());
	}

	public void ResumeButtonHasBeenClicked() {
		EventManager.Instance.Raise(new ResumeButtonClickedEvent());
	}

	public void MainMenuButtonHasBeenClicked() {
		EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
	}

	public void QuitButtonHasBeenClicked() {
		EventManager.Instance.Raise(new QuitButtonClickedEvent());
	}

	#endregion

	#region Callbacks to GameManager events

	protected override void OnGameMenu(GameMenuEvent e) {
		this.OpenPanel(this.panelMainMenu);
	}

	protected override void OnGamePlay(GamePlayEvent e) {
		this.OpenPanel(null);
	}

	protected override void OnGamePause(GamePauseEvent e) {
		this.OpenPanel(this.panelInGameMenu);
	}

	protected override void OnGameResume(GameResumeEvent e) {
		this.OpenPanel(null);
	}

	protected override void OnGameOver(GameOverEvent e) {
		this.OpenPanel(this.panelGameOver);
	}

	#endregion
}
