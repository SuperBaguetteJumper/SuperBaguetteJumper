using Elements.Flags;
using Events;
using Objects;

#region GameManager Events

/// <summary>Game is now in the menu state</summary>
public class GameMenuEvent : Event {}
/// <summary>Game is now in the play state</summary>
public class GamePlayEvent : Event {}
/// <summary>Game is now in the pause state</summary>
public class GamePauseEvent : Event {}
/// <summary>Game switched from the pause state to the play state</summary>
public class GameResumeEvent : Event {}
/// <summary>Game is now in the over state</summary>
public class GameOverEvent : Event {}
/// <summary>Game is now in the victory state</summary>
public class GameVictoryEvent : Event {}
/// <summary>Game statistics has changed</summary>
public class GameStatisticsChangedEvent : Event {
	public float eBestScore { get; set; }
	public float eScore { get; set; }
	public int eNLives { get; set; }
}

#endregion

#region MenuManager Events

/// <summary>Escape menu button has been clicked</summary>
public class EscapeButtonClickedEvent : Event {}
/// <summary>Play menu button has been clicked</summary>
public class PlayButtonClickedEvent : Event {}
/// <summary>Resume menu button has been clicked</summary>
public class ResumeButtonClickedEvent : Event {}
/// <summary>Main menu button has been clicked</summary>
public class MainMenuButtonClickedEvent : Event {}
/// <summary>Quit menu button has been clicked</summary>
public class QuitButtonClickedEvent : Event {}

#endregion

#region Level Events

/// <summary>Checkpoint has been reached in a level</summary>
public class CheckpointReachedEvent : Event {
	public Flag Flag { get; }

	public CheckpointReachedEvent(Flag flag) {
		this.Flag = flag;
	}
}
/// <summary>Level end has been reached</summary>
public class EndReachedEvent: Event {}

#endregion

#region Player Events

public class PlayerEvent : Event {
	public Player Player { get; }

	public PlayerEvent(Player player) {
		this.Player = player;
	}
}
/// <summary>Player spawned in the level</summary>
public class PlayerSpawnEvent : PlayerEvent {
	public PlayerSpawnEvent(Player player) : base(player) {}
}
/// <summary>Player died in the level</summary>
public class PlayerDiedEvent : PlayerEvent {
	public PlayerDiedEvent(Player player) : base(player) {}
}
/// <summary>Player got trapped in the level</summary>
public class PlayerTrappedEvent : PlayerEvent {
	public PlayerTrappedEvent(Player player) : base(player) {}
}

#endregion

#region Objets Events

/// <summary>Physical Object has been picked up</summary>
public class ObjectPickedUpEvent<T> : Event where T : PhysicalObject {
	public T Object { get; }

	public ObjectPickedUpEvent(T obj) {
		this.Object = obj;
	}
}
/// <summary>Cheese (speedness) object has been picked up</summary>
public class CheeseObjectPickedUpEvent : ObjectPickedUpEvent<CheeseObject> {
	public CheeseObjectPickedUpEvent(CheeseObject obj) : base(obj) {}
}
/// <summary>Frog leg (jump boost) object has been picked up</summary>
public class FrogLegObjectPickedUpEvent : ObjectPickedUpEvent<FrogLegObject> {
	public FrogLegObjectPickedUpEvent(FrogLegObject obj) : base(obj) {}
}
/// <summary>French flag (music easter egg) object has been picked up</summary>
public class FrenchFlagObjectPickedUpEvent : ObjectPickedUpEvent<FrenchFlagObject> {
	public FrenchFlagObjectPickedUpEvent(FrenchFlagObject obj) : base(obj) {}
}
/// <summary>Snail (slowness) object has been picked up</summary>
public class SnailObjectPickedUpEvent : ObjectPickedUpEvent<SnailObject> {
	public SnailObjectPickedUpEvent(SnailObject obj) : base(obj) {}
}
/// <summary>Wine bottle (drunkness / nausea) object has been picked up</summary>
public class WineBottleObjectPickedUpEvent : ObjectPickedUpEvent<WineBottleObject> {
	public WineBottleObjectPickedUpEvent(WineBottleObject obj) : base(obj) {}
}
/// <summary>Old car (blindness) object has been picked up</summary>
public class OldCarObjectPickedUpEvent : ObjectPickedUpEvent<OldCarObject> {
	public OldCarObjectPickedUpEvent(OldCarObject obj) : base(obj) {}
}

#endregion

#region Score Event

/// <summary>???</summary>
public class ScoreItemEvent : Event {
	public float eScore;
}

#endregion
