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
	[SerializeField] private Slider volumeSlider;
	[SerializeField] private Text volumeText;
	[SerializeField] private Slider sensitivitySlider;
	[SerializeField] private Text sensitivityText;
	[Header("End Overlay")]
	[SerializeField] private GameObject endOverlay;
	[SerializeField] private Text levelStateText;
	[SerializeField] private Button continueButton;
	[SerializeField] private Button retryButton;
	[Header("Pause Overlay")]
	[SerializeField] private GameObject pauseOverlay;
	[SerializeField] private Button resumeButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button returnButton;
	[SerializeField] private Button menuButton;

	private GameState gameState;
	private string level;

	public bool IsPlaying => this.gameState == GameState.Hub || this.gameState == GameState.Level;

	public int Money {
		get => PlayerPrefs.GetInt("Money", 0);
		private set {
			PlayerPrefs.SetInt("Money", value);
			if (value != 0)
				EventManager.Instance.Raise(new MoneyUpdatedEvent(value));
		}
	}

	protected override void Awake() {
		base.Awake();
		DontDestroyOnLoad(this.gameObject);
		EventManager.Instance.AddListener<LevelLaunchEvent>(this.OnLevelLaunch);
		EventManager.Instance.AddListener<LevelWonEvent>(this.OnLevelWon);
		EventManager.Instance.AddListener<LevelLostEvent>(this.OnLevelLost);
		EventManager.Instance.AddListener<MoneyWithdrawEvent>(this.OnMoneyWithdraw);
		// Title Screen
		this.playButton.onClick.AddListener(this.Play);
		this.quitButton.onClick.AddListener(this.Quit);
		this.volumeSlider.onValueChanged.AddListener(this.Volume);
		this.sensitivitySlider.onValueChanged.AddListener(this.Sensitivity);
		// End Overlay
		this.continueButton.onClick.AddListener(this.Continue);
		this.retryButton.onClick.AddListener(this.Retry);
		// Pause Overlay
		this.resumeButton.onClick.AddListener(this.Resume);
		this.restartButton.onClick.AddListener(this.Restart);
		this.returnButton.onClick.AddListener(this.Return);
		this.menuButton.onClick.AddListener(this.Menu);
	}

	private void Start() {
		EventManager.Instance.Raise(new MoneyUpdatedEvent(this.Money));
		this.gameState = GameState.TitleScreen;
		this.titleScreen.SetActive(true);
		Time.timeScale = 0;

		Button[] buttons = FindObjectsOfType<Button>(true);
		for (int i = 0; i < buttons.Length; i++) 
			buttons[i].onClick.AddListener(this.Click);

		this.volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
		this.sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1);
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
		EventManager.Instance.RemoveListener<MoneyWithdrawEvent>(this.OnMoneyWithdraw);
		// Title Screen
		this.playButton.onClick.RemoveListener(this.Play);
		this.quitButton.onClick.RemoveListener(this.Quit);
		this.volumeSlider.onValueChanged.RemoveListener(this.Volume);
		this.sensitivitySlider.onValueChanged.RemoveListener(this.Sensitivity);
		// End Overlay
		this.continueButton.onClick.RemoveListener(this.Continue);
		this.retryButton.onClick.RemoveListener(this.Retry);
		// Pause Overlay
		this.resumeButton.onClick.RemoveListener(this.Resume);
		this.restartButton.onClick.RemoveListener(this.Restart);
		this.returnButton.onClick.RemoveListener(this.Return);
		this.menuButton.onClick.RemoveListener(this.Menu);

		Button[] buttons = FindObjectsOfType<Button>(true);
		for (int i = 0; i < buttons.Length; i++) 
			buttons[i].onClick.RemoveListener(this.Click);
	}

	private void Play() {
		this.gameState = GameState.Hub;
		this.titleScreen.SetActive(false);
		Time.timeScale = 1;
		LockCursor();
	}

	private void Level(string levelName) {
		this.gameState = GameState.Level;
		this.level = levelName;
		SceneManager.LoadSceneAsync(this.level);
		Time.timeScale = 1;
		LockCursor();
	}

	private void End(string state, bool canRetry) {
		this.gameState = GameState.End;
		SceneManager.LoadSceneAsync("main");
		this.levelStateText.text = state;
		this.retryButton.interactable = canRetry;
		this.endOverlay.SetActive(true);
		Time.timeScale = 0;
		UnlockCursor();
	}

	private void Continue() {
		this.gameState = GameState.Hub;
		this.endOverlay.SetActive(false);
		Time.timeScale = 1;
		LockCursor();
	}

	private void Retry() {
		this.endOverlay.SetActive(false);
		this.Level(this.level);
	}

	private void Pause() {
		bool isInLevel = this.gameState == GameState.Level;
		this.gameState = GameState.Pause;
		this.restartButton.interactable = isInLevel;
		this.returnButton.gameObject.SetActive(isInLevel);
		this.menuButton.gameObject.SetActive(!isInLevel);
		this.pauseOverlay.SetActive(true);
		Time.timeScale = 0;
		UnlockCursor();
	}

	private void Resume() {
		this.gameState = SceneManager.GetActiveScene().name == "main" ? GameState.Hub : GameState.Level;
		this.pauseOverlay.SetActive(false);
		Time.timeScale = 1;
		LockCursor();
	}

	private void Restart() {
		SceneManager.LoadSceneAsync(this.level);
		this.pauseOverlay.SetActive(false);
		Time.timeScale = 1;
		LockCursor();
	}

	private void Return() {
		this.gameState = GameState.Hub;
		SceneManager.LoadSceneAsync("main");
		EventManager.Instance.Raise(new MoneyUpdatedEvent(this.Money));
		this.pauseOverlay.SetActive(false);
		Time.timeScale = 1;
		LockCursor();
	}

	private void Menu() {
		this.gameState = GameState.TitleScreen;
		this.pauseOverlay.SetActive(false);
		this.titleScreen.SetActive(true);
	}

	private void Quit() {
		Application.Quit();
	}

	private void Click() {
		SfxManager.Instance.PlaySfx2D("Click");
	}

	private void Volume(float volume) {
		PlayerPrefs.SetFloat("Volume", volume);
		this.volumeText.text = "Volume : " + volume.ToString("F2");
		AudioListener.volume = volume;
	}

	private void Sensitivity(float sensitivity) {
		PlayerPrefs.SetFloat("Sensitivity", sensitivity);
		this.sensitivityText.text = "Sensibilité : " + sensitivity.ToString("F2");
		Player.Sensitivity = sensitivity;
	}

	private void OnLevelLaunch(LevelLaunchEvent e) {
		SfxManager.Instance.PlaySfx2D("Portal");
		this.Level("Scenes/Levels/" + e.Name);
	}

	private void OnLevelWon(LevelWonEvent e) {
		this.Money += e.Coins;
		this.End("Victoire! +" + e.Coins, false);
	}

	private void OnLevelLost(LevelLostEvent e) {
		SfxManager.Instance.PlaySfx2D("GameOver");
		this.End("Défaite :(", true);
	}

	private void OnMoneyWithdraw(MoneyWithdrawEvent e) {
		if (e.Amount <= this.Money) {
			this.Money -= e.Amount;
			e.Success = true;
		}
	}

	public static void LockCursor() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public static void UnlockCursor() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
