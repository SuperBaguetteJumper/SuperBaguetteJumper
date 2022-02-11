using Events;

#region GameManager Events

public class GameMenuEvent : Event {}
public class GamePlayEvent : Event {}
public class GamePauseEvent : Event {}
public class GameResumeEvent : Event {}
public class GameOverEvent : Event {}
public class GameVictoryEvent : Event {}
public class GameStatisticsChangedEvent : Event {
	public float eBestScore { get; set; }
	public float eScore { get; set; }
	public int eNLives { get; set; }
}

#endregion

#region MenuManager Events

public class EscapeButtonClickedEvent : Event {}
public class PlayButtonClickedEvent : Event {}
public class ResumeButtonClickedEvent : Event {}
public class MainMenuButtonClickedEvent : Event {}
public class QuitButtonClickedEvent : Event {}

#endregion

#region Score Event

public class ScoreItemEvent : Event {
	public float eScore;
}

#endregion
