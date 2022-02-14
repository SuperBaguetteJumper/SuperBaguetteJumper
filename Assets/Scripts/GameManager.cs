using System.Collections;
using Common;
using Events;
using UnityEngine;

public enum GameState {
	GAME_MENU,
	GAME_PLAY,
	GAME_NEXT_LEVEL,
	GAME_PAUSE,
	GAME_OVER,
	GAME_VICTORY
}

public class GameManager : Manager<GameManager> {
	#region Time

	private void SetTimeScale(float newTimeScale) {
		Time.timeScale = newTimeScale;
	}

	#endregion

	#region Manager implementation

	protected override IEnumerator InitCoroutine() {
		this.Menu();
		this.InitNewGame(); // Essentiellement pour que les statistiques du jeu soient mise à jour en HUD
		yield break;
	}

	#endregion

	#region Game flow & Gameplay

	// Game initialization
	private void InitNewGame(bool raiseStatsEvent = true) {
		this.SetScore(0);
	}

	#endregion

	#region Callbacks to events issued by Score items

	private void ScoreHasBeenGained(ScoreItemEvent e) {
		if (this.IsPlaying)
			IncrementScore(e.eScore);
	}

	#endregion

	#region Game State

	private GameState gameState;
	public bool IsPlaying => this.gameState == GameState.GAME_PLAY;

	#endregion

	#region Lives

	[Header("GameManager")]
	[SerializeField] private int nStartLives;

	public int NLives { get; private set; }

	private void DecrementNLives(int decrement) {
		this.SetNLives(this.NLives - decrement);
	}

	private void SetNLives(int nLives) {
		this.NLives = nLives;
		EventManager.Instance.Raise(new GameStatisticsChangedEvent { eBestScore = this.BestScore, eScore = this.score, eNLives = this.NLives });
	}

	#endregion

	#region Score

	private float score;

	public float Score {
		get => this.score;
		set {
			this.score = value;
			this.BestScore = Mathf.Max(this.BestScore, value);
		}
	}

	public float BestScore {
		get => PlayerPrefs.GetFloat("BEST_SCORE", 0);
		set => PlayerPrefs.SetFloat("BEST_SCORE", value);
	}

	private void IncrementScore(float increment) {
		this.SetScore(score + increment);
	}

	private void SetScore(float score, bool raiseEvent = true) {
		this.Score = score;
		if (raiseEvent)
			EventManager.Instance.Raise(new GameStatisticsChangedEvent { eBestScore = this.BestScore, eScore = this.score, eNLives = this.NLives });
	}

	#endregion

	#region Events' subscription

	public override void SubscribeEvents() {
		base.SubscribeEvents();

		// MainMenuManager
		EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(this.MainMenuButtonClicked);
		EventManager.Instance.AddListener<PlayButtonClickedEvent>(this.PlayButtonClicked);
		EventManager.Instance.AddListener<ResumeButtonClickedEvent>(this.ResumeButtonClicked);
		EventManager.Instance.AddListener<EscapeButtonClickedEvent>(this.EscapeButtonClicked);
		EventManager.Instance.AddListener<QuitButtonClickedEvent>(this.QuitButtonClicked);

		// Score Item
		EventManager.Instance.AddListener<ScoreItemEvent>(this.ScoreHasBeenGained);
	}

	public override void UnsubscribeEvents() {
		base.UnsubscribeEvents();

		// MainMenuManager
		EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(this.MainMenuButtonClicked);
		EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(this.PlayButtonClicked);
		EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(this.ResumeButtonClicked);
		EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(this.EscapeButtonClicked);
		EventManager.Instance.RemoveListener<QuitButtonClickedEvent>(this.QuitButtonClicked);

		// Score Item
		EventManager.Instance.RemoveListener<ScoreItemEvent>(this.ScoreHasBeenGained);
	}

	#endregion

	#region Callbacks to Events issued by MenuManager

	private void MainMenuButtonClicked(MainMenuButtonClickedEvent e) {
		this.Menu();
	}

	private void PlayButtonClicked(PlayButtonClickedEvent e) {
		this.Play();
	}

	private void ResumeButtonClicked(ResumeButtonClickedEvent e) {
		this.Resume();
	}

	private void EscapeButtonClicked(EscapeButtonClickedEvent e) {
		if (this.IsPlaying)
			this.Pause();
	}

	private void QuitButtonClicked(QuitButtonClickedEvent e) {
		Application.Quit();
	}

	#endregion

	#region GameState methods

	private void Menu() {
		this.SetTimeScale(1);
		this.gameState = GameState.GAME_MENU;
		if (MusicLoopsManager.Instance)
			MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
		EventManager.Instance.Raise(new GameMenuEvent());
	}

	private void Play() {
		this.InitNewGame();
		this.SetTimeScale(1);
		this.gameState = GameState.GAME_PLAY;

		if (MusicLoopsManager.Instance)
			MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_MUSIC);
		EventManager.Instance.Raise(new GamePlayEvent());
	}

	private void Pause() {
		if (!this.IsPlaying)
			return;

		this.SetTimeScale(0);
		this.gameState = GameState.GAME_PAUSE;
		EventManager.Instance.Raise(new GamePauseEvent());
	}

	private void Resume() {
		if (this.IsPlaying)
			return;

		this.SetTimeScale(1);
		this.gameState = GameState.GAME_PLAY;
		EventManager.Instance.Raise(new GameResumeEvent());
	}

	private void Over() {
		this.SetTimeScale(0);
		this.gameState = GameState.GAME_OVER;
		EventManager.Instance.Raise(new GameOverEvent());
		if (SfxManager.Instance)
			SfxManager.Instance.PlaySfx2D(Constants.GAMEOVER_SFX);
	}

	#endregion
}
