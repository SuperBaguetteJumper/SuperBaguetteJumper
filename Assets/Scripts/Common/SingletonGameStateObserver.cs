using Events;
using UnityEngine;

namespace Common {
	public abstract class SingletonGameStateObserver<T> : Singleton<T>, IEventHandler where T : Component {
		protected override void Awake() {
			base.Awake();
			this.SubscribeEvents();
		}

		protected virtual void OnDestroy() {
			this.UnsubscribeEvents();
		}

		public virtual void SubscribeEvents() {
			EventManager.Instance.AddListener<GameMenuEvent>(this.OnGameMenu);
			EventManager.Instance.AddListener<GamePlayEvent>(this.OnGamePlay);
			EventManager.Instance.AddListener<GamePauseEvent>(this.OnGamePause);
			EventManager.Instance.AddListener<GameResumeEvent>(this.OnGameResume);
			EventManager.Instance.AddListener<GameOverEvent>(this.OnGameOver);
			EventManager.Instance.AddListener<GameVictoryEvent>(this.OnGameVictory);
			EventManager.Instance.AddListener<GameStatisticsChangedEvent>(this.OnGameStatisticsChanged);
		}

		public virtual void UnsubscribeEvents() {
			EventManager.Instance.RemoveListener<GameMenuEvent>(this.OnGameMenu);
			EventManager.Instance.RemoveListener<GamePlayEvent>(this.OnGamePlay);
			EventManager.Instance.RemoveListener<GamePauseEvent>(this.OnGamePause);
			EventManager.Instance.RemoveListener<GameResumeEvent>(this.OnGameResume);
			EventManager.Instance.RemoveListener<GameOverEvent>(this.OnGameOver);
			EventManager.Instance.RemoveListener<GameVictoryEvent>(this.OnGameVictory);
			EventManager.Instance.RemoveListener<GameStatisticsChangedEvent>(this.OnGameStatisticsChanged);
		}

		protected virtual void OnGameMenu(GameMenuEvent e) {}
		protected virtual void OnGamePlay(GamePlayEvent e) {}
		protected virtual void OnGamePause(GamePauseEvent e) {}
		protected virtual void OnGameResume(GameResumeEvent e) {}
		protected virtual void OnGameOver(GameOverEvent e) {}
		protected virtual void OnGameVictory(GameVictoryEvent e) {}
		protected virtual void OnGameStatisticsChanged(GameStatisticsChangedEvent e) {}
	}
}
