using Events;
using UnityEngine;

namespace Common {
	public abstract class SimpleGameStateObserver : MonoBehaviour, IEventHandler {
		protected virtual void Awake() {
			this.SubscribeEvents();
		}

		protected virtual void OnDestroy() {
			this.UnsubscribeEvents();
		}

		public virtual void SubscribeEvents() {
			EventManager.Instance.AddListener<GameMenuEvent>(this.GameMenu);
			EventManager.Instance.AddListener<GamePlayEvent>(this.GamePlay);
			EventManager.Instance.AddListener<GamePauseEvent>(this.GamePause);
			EventManager.Instance.AddListener<GameResumeEvent>(this.GameResume);
			EventManager.Instance.AddListener<GameOverEvent>(this.GameOver);
			EventManager.Instance.AddListener<GameVictoryEvent>(this.GameVictory);
			EventManager.Instance.AddListener<GameStatisticsChangedEvent>(this.GameStatisticsChanged);
		}

		public virtual void UnsubscribeEvents() {
			EventManager.Instance.RemoveListener<GameMenuEvent>(this.GameMenu);
			EventManager.Instance.RemoveListener<GamePlayEvent>(this.GamePlay);
			EventManager.Instance.RemoveListener<GamePauseEvent>(this.GamePause);
			EventManager.Instance.RemoveListener<GameResumeEvent>(this.GameResume);
			EventManager.Instance.RemoveListener<GameOverEvent>(this.GameOver);
			EventManager.Instance.RemoveListener<GameVictoryEvent>(this.GameVictory);
			EventManager.Instance.RemoveListener<GameStatisticsChangedEvent>(this.GameStatisticsChanged);
		}

		protected virtual void GameMenu(GameMenuEvent e) {}
		protected virtual void GamePlay(GamePlayEvent e) {}
		protected virtual void GamePause(GamePauseEvent e) {}
		protected virtual void GameResume(GameResumeEvent e) {}
		protected virtual void GameOver(GameOverEvent e) {}
		protected virtual void GameVictory(GameVictoryEvent e) {}
		protected virtual void GameStatisticsChanged(GameStatisticsChangedEvent e) {}
	}
}
