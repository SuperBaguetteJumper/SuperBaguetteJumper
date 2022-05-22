using Common;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState {
	TitleScreen,
	Hub,
	Level,
	Pause,
	End
}

public class GameManager : Singleton<GameManager> {
	[Header("Title Screen")]
	[SerializeField] private GameObject titleScreen;
	[SerializeField] private Button playButton;
	[SerializeField] private Button quitButton;
	[Header("End Overlay")]
	[SerializeField] private GameObject endOverlay;
	[SerializeField] private Text levelStateText;
	[SerializeField] private Button continueButton;
	[Header("Pause Overlay")]
	[SerializeField] private GameObject pauseOverlay;
	[SerializeField] private Button resumeButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button returnButton;

	private GameState gameState;

	public bool IsPlaying => this.gameState == GameState.Hub || this.gameState == GameState.Level;

	public int Money {
		get => PlayerPrefs.GetInt("Money", 0);
		private set => PlayerPrefs.SetInt("Money", value);
	}

	protected override void Awake() {
		base.Awake();
		DontDestroyOnLoad(this.gameObject);
		EventManager.Instance.AddListener<LevelLaunchEvent>(this.OnLevelLaunch);
		EventManager.Instance.AddListener<LevelWonEvent>(this.OnLevelWon);
		EventManager.Instance.AddListener<LevelLostEvent>(this.OnLevelLost);
		// Title Screen
		this.playButton.onClick.AddListener(this.Play);
		this.quitButton.onClick.AddListener(this.Quit);
		// End Overlay
		this.continueButton.onClick.AddListener(this.Continue);
		// Pause Overlay
		this.resumeButton.onClick.AddListener(this.Resume);
		this.restartButton.onClick.AddListener(this.Restart);
		this.returnButton.onClick.AddListener(this.Return);
	}

	private void Start() {
		this.gameState = GameState.TitleScreen;
		this.titleScreen.SetActive(true);
	}

	private void Update() {
		if (Input.GetButtonDown("Cancel")) {
			if (this.IsPlaying)
            	this.Pause();
            else if (this.gameState == GameState.Pause)
            	this.Resume();
			else if (this.gameState == GameState.End)
				this.Continue();
		}
	}

	private void OnDestroy() {
		EventManager.Instance.RemoveListener<LevelLaunchEvent>(this.OnLevelLaunch);
		EventManager.Instance.RemoveListener<LevelWonEvent>(this.OnLevelWon);
		EventManager.Instance.RemoveListener<LevelLostEvent>(this.OnLevelLost);
		// Title Screen
		this.playButton.onClick.RemoveListener(this.Play);
		this.quitButton.onClick.RemoveListener(this.Quit);
		// End Overlay
		this.continueButton.onClick.RemoveListener(this.Continue);
		// Pause Overlay
		this.resumeButton.onClick.RemoveListener(this.Resume);
		this.restartButton.onClick.RemoveListener(this.Restart);
		this.returnButton.onClick.RemoveListener(this.Return);
	}

	private void Play() {
		this.gameState = GameState.Hub;
		this.titleScreen.SetActive(false);
	}

	private void Level(string levelName) {
		this.gameState = GameState.Level;
		SceneManager.LoadScene("Levels/" + levelName);
	}

	private void End(string state) {
		this.gameState = GameState.End;
		this.levelStateText.text = state;
		this.endOverlay.SetActive(true);
	}

	private void Continue() {
		this.gameState = GameState.Hub;
		this.endOverlay.SetActive(false);
	}

	private void Pause() {
		bool isInLevel = this.gameState == GameState.Level;
		this.gameState = GameState.Pause;
		this.restartButton.interactable = isInLevel;
		this.returnButton.interactable = isInLevel;
		this.pauseOverlay.SetActive(true);
		Time.timeScale = 0;
	}

	private void Resume() {
		this.gameState = SceneManager.GetActiveScene().name == "main" ? GameState.Hub : GameState.Level;
		Time.timeScale = 1;
	}

	private void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		Time.timeScale = 1;
	}

	private void Return() {
		this.gameState = GameState.Hub;
		SceneManager.LoadScene("main");
		Time.timeScale = 1;
	}

	private void Quit() {
		Application.Quit();
	}

	private void OnLevelLaunch(LevelLaunchEvent e) {
		this.Level(e.Name);
	}

	private void OnLevelWon(LevelWonEvent e) {
		this.Money += e.Coins;
		this.End("Victoire! +" + e.Coins);
	}

	private void OnLevelLost(LevelLostEvent e) {
		this.End("Défaite :(");
	}
}
