using System.Collections.Generic;
using Elements.Flags;
using Objects;
using UnityEngine;
using Event = Events.Event;

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

/// <summary>A flag has been reached in a level</summary>
public abstract class FlagReachedEvent : Event {
	public Flag Flag { get; }

	public FlagReachedEvent(Flag flag) {
		this.Flag = flag;
	}
}
/// <summary>Checkpoint has been reached in a level</summary>
public class CheckpointReachedEvent : FlagReachedEvent {
	public CheckpointReachedEvent(Flag flag) : base(flag) {}
}
/// <summary>Level end has been reached</summary>
public class EndReachedEvent : FlagReachedEvent {
	public EndReachedEvent(Flag flag) : base(flag) {}
}
/// <summary>Level should be launched</summary>
public class LevelLaunchEvent : Event {
	public string Name { get; }

	public LevelLaunchEvent(string name) {
		this.Name = name;
	}
}
/// <summary>Level has been started</summary>
public class LevelStartedEvent : Event {
	public float Time { get; }

	public LevelStartedEvent(float time) {
		this.Time = time;
	}
}
/// <summary>Level has been lost</summary>
public class LevelLostEvent : Event {}
/// <summary>Level has been won</summary>
public class LevelWonEvent : Event {
	public int Coins { get; }

	public LevelWonEvent(int coins) {
		this.Coins = coins;
	}
}
/// <summary>Level health has been updated</summary>
public class HealthUpdatedEvent : Event {
	public int Count { get; }
	public int Total { get; }

	public HealthUpdatedEvent(int count, int total) {
		this.Count = count;
		this.Total = total;
	}
}

#endregion

#region Player Events

/// <summary>Player was loaded</summary>
public class PlayerLoadedEvent : Event {
	public Player Player { get; }

	public PlayerLoadedEvent(Player player) {
		this.Player = player;
	}
}
/// <summary>Player spawned in the level</summary>
public class PlayerSpawnedEvent : Event {
	public Transform RespawnPoint { get; }

	public PlayerSpawnedEvent(Transform respawnPoint) {
		this.RespawnPoint = respawnPoint;
	}
}
/// <summary>Player died in the level</summary>
public class PlayerDiedEvent : Event {}
/// <summary>Player got trapped in the level</summary>
public class PlayerTrappedEvent : Event {}

#endregion

#region Objets Events

/// <summary>Physical Object has been picked up</summary>
public abstract class ObjectPickedUpEvent<T> : Event where T : PhysicalObject {
	public T Object { get; }
	public bool CanPickup { get; set; }

	public ObjectPickedUpEvent(T obj) {
		this.Object = obj;
		this.CanPickup = obj.IsCollectible;
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
/// <summary>Coin (money) object has been picked up</summary>
public class CoinObjectPickedUpEvent : ObjectPickedUpEvent<CoinObject> {
	public CoinObjectPickedUpEvent(CoinObject obj) : base(obj) {}
}
/// <summary>Baguette (extra life) object has been picked up</summary>
public class BaguetteObjectPickedUpEvent : ObjectPickedUpEvent<BaguetteObject> {
	public BaguetteObjectPickedUpEvent(BaguetteObject obj) : base(obj) {}
}
/// <summary>Spring (vertical propulsion) object has been picked up</summary>
public class SpringObjectPickedUpEvent : ObjectPickedUpEvent<SpringObject> {
	public SpringObjectPickedUpEvent(SpringObject obj) : base(obj) {}
}

#endregion

#region Effects Events

/// <summary>Effect has been activated</summary>
public class EffectActivatedEvent : Event {
	public EffectsManager Manager { get; }
	public Effect Effect { get; }
	public float Strengh { get; }

	public EffectActivatedEvent(EffectsManager manager, Effect effect, float strengh) {
		this.Manager = manager;
		this.Effect = effect;
		this.Strengh = strengh;
	}
}
/// <summary>Effects durations have been updated</summary>
public class EffectsUpdatedEvent : Event {
	public Dictionary<Effect, float> Durations { get; }

	public EffectsUpdatedEvent(Dictionary<Effect, float> durations) {
		this.Durations = durations;
	}
}

#endregion

#region Cosmetics Events

/// <summary>Cosmetic is being involved in an event (preview, unlock, ...)</summary>
public abstract class CosmeticEvent : Event {
	public Cosmetic Cosmetic { get; }
	public Player Player { get; }

	public CosmeticEvent(Cosmetic cosmetic, Player player) {
		this.Cosmetic = cosmetic;
		this.Player = player;
	}
}
/// <summary>Cosmetic is being unlocked</summary>
public class CosmeticUnlockedEvent : CosmeticEvent {
	public CosmeticUnlockedEvent(Cosmetic cosmetic, Player player) : base(cosmetic, player) {}
}
/// <summary>Cosmetic is being previewed</summary>
public class CosmeticPreviewBeganEvent : CosmeticEvent {
	public CosmeticPreviewBeganEvent(Cosmetic cosmetic, Player player) : base(cosmetic, player) {}
}
/// <summary>Cosmetic is being unpreviewed</summary>
public class CosmeticPreviewEndedEvent : CosmeticEvent {
	public CosmeticPreviewEndedEvent(Cosmetic cosmetic, Player player) : base(cosmetic, player) {}
}

#endregion

#region Money Events

/// <summary>A transaction is requested</summary>
public class MoneyWithdrawEvent : Event {
	public int Amount { get; }
	public bool Success { get; set; }

	public MoneyWithdrawEvent(int amount) {
		this.Amount = amount;
	}
}
/// <summary>Money has been updated</summary>
public class MoneyUpdatedEvent : Event {
	public int Coins { get; }

	public MoneyUpdatedEvent(int coins) {
		this.Coins = coins;
	}
}

#endregion
