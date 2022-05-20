using System;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class EffectsManager : MonoBehaviour {
	private Effect active;
	private Dictionary<Effect, float> durations = new Dictionary<Effect, float>();

	public bool HasActiveEffect => this.active != Effect.None;
	public bool HasNoActiveEffect => this.active == Effect.None;
	public bool HasSlownessEffect => this.HasEffect(Effect.Slowness);
	public bool HasSpeednessEffect => this.HasEffect(Effect.Speedness);
	public bool HasJumpBoostEffect => this.HasEffect(Effect.JumpBoost);
	public bool HasDrunknessEffect => this.HasEffect(Effect.Drunkness);
	public bool HasBlindnessEffect => this.HasEffect(Effect.Blindness);

	public bool HasEffect(Effect effect) => (this.active & effect) == effect;

	private void Awake() {
		EventManager.Instance.AddListener<SnailObjectPickedUpEvent>(this.OnSnailObjectPickedUp);
		EventManager.Instance.AddListener<CheeseObjectPickedUpEvent>(this.OnCheeseObjectPickedUp);
		EventManager.Instance.AddListener<FrogLegObjectPickedUpEvent>(this.OnFrogLegObjectPickedUp);
		EventManager.Instance.AddListener<WineBottleObjectPickedUpEvent>(this.OnWineBottleObjectPickedUp);
	}

	private void OnDestroy() {
		EventManager.Instance.RemoveListener<SnailObjectPickedUpEvent>(this.OnSnailObjectPickedUp);
		EventManager.Instance.RemoveListener<CheeseObjectPickedUpEvent>(this.OnCheeseObjectPickedUp);
		EventManager.Instance.RemoveListener<FrogLegObjectPickedUpEvent>(this.OnFrogLegObjectPickedUp);
		EventManager.Instance.RemoveListener<WineBottleObjectPickedUpEvent>(this.OnWineBottleObjectPickedUp);
	}

	private void Update() {
		float time = Time.time;
		foreach (Effect effect in Enum.GetValues(typeof(Effect))) {
			float duration;
			if (this.durations.TryGetValue(effect, out duration) && duration < time) {
				this.active &= ~effect;
				this.durations.Remove(effect);
			}
		}
	}

	private void OnSnailObjectPickedUp(SnailObjectPickedUpEvent e) {
		this.ActivateEffect(Effect.Slowness, e.Object.Duration, e.Object.Strengh);
	}

	private void OnCheeseObjectPickedUp(CheeseObjectPickedUpEvent e) {
		this.ActivateEffect(Effect.Speedness, e.Object.Duration, e.Object.Strengh);
	}

	private void OnFrogLegObjectPickedUp(FrogLegObjectPickedUpEvent e) {
		this.ActivateEffect(Effect.JumpBoost, e.Object.Duration, e.Object.Strengh);
	}

	private void OnWineBottleObjectPickedUp(WineBottleObjectPickedUpEvent e) {
		this.ActivateEffect(Effect.Drunkness, e.Object.Duration, e.Object.Strengh);
	}

	private void ActivateEffect(Effect effect, float duration, float strengh) {
		bool alreadyActive = this.HasEffect(effect);
		this.active |= effect;
		this.durations[effect] = Time.time + duration;
		if (!alreadyActive)
			EventManager.Instance.Raise(new EffectActivatedEvent(this, effect, strengh));
		EventManager.Instance.Raise(new EffectsUpdatedEvent(this.durations));
	}
}

[Flags]
public enum Effect {
	None = 0,
	Slowness = 1,
	Speedness = 2,
	JumpBoost = 4,
	Drunkness = 8,
	Blindness = 16
}
