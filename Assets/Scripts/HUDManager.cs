using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
	[Header("Health & Money")]
	[SerializeField] private GameObject healthBar;
	[SerializeField] private Text extraHealthText;
	[SerializeField] private Text moneyAmountText;
	[SerializeField] private GameObject life;
	[SerializeField] private GameObject lifeEmpty;
	[Header("Timer")]
	[SerializeField] private Text timerText;
	[Header("Effects")]
	[SerializeField] private GameObject effectsContainer;
	[SerializeField] private GameObject slownessEffect;
	[SerializeField] private GameObject speednessEffect;
	[SerializeField] private GameObject jumpBoostEffect;
	[SerializeField] private GameObject drunknessEffect;

	private float start;
	private Dictionary<Effect, float> effects;
	private Dictionary<Effect, Text> durations = new Dictionary<Effect, Text>();

	private void Awake() {
		EventManager.Instance.AddListener<LevelStartedEvent>(this.OnLevelStarted);
		EventManager.Instance.AddListener<EffectsUpdatedEvent>(this.OnEffectsUpdated);
		EventManager.Instance.AddListener<HealthUpdatedEvent>(this.OnHealthUpdated);
		EventManager.Instance.AddListener<MoneyUpdatedEvent>(this.OnMoneyUpdated);
	}

	private IEnumerator Start() {
		while (this.enabled) {
			this.UpdateEffects();
			this.UpdateTimer();
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void OnDestroy() {
		EventManager.Instance.RemoveListener<LevelStartedEvent>(this.OnLevelStarted);
		EventManager.Instance.RemoveListener<EffectsUpdatedEvent>(this.OnEffectsUpdated);
		EventManager.Instance.RemoveListener<HealthUpdatedEvent>(this.OnHealthUpdated);
		EventManager.Instance.RemoveListener<MoneyUpdatedEvent>(this.OnMoneyUpdated);
	}

	private void UpdateEffects() {
		if (this.effects == null || this.effects.Count == 0) {
        	// No active effect: Hide effects container
        	this.effectsContainer.SetActive(false);
        } else {
        	// Active effects
        	float time = Time.time;
        	this.effectsContainer.SetActive(true);
        	foreach (Effect effect in Enum.GetValues(typeof(Effect))) {
        		float end;
        		if (this.effects.TryGetValue(effect, out end)) {
        			// Effect is active
        			string duration = new TimeSpan(0, 0, Mathf.CeilToInt(end - time)).ToString("mm':'ss");
        			Text durationText;
        			if (this.durations.TryGetValue(effect, out durationText)) {
        				// And was already created
        				durationText.text = duration;
        			} else {
        				// But needs to be created
        				GameObject effectModel = effect switch {
        					Effect.Slowness => this.slownessEffect,
        					Effect.Speedness => this.speednessEffect,
        					Effect.JumpBoost => this.jumpBoostEffect,
        					Effect.Drunkness => this.drunknessEffect,
        					_ => null
        				};
        				if (effectModel != null) {
        					GameObject effectInstance = Instantiate(effectModel, this.effectsContainer.transform);
        					Text text = effectInstance.GetComponentInChildren<Text>();
        					text.text = duration;
        					this.durations[effect] = text;
        				}
        			}
        		} else {
        			// Effect is inactive
        			Text text;
        			if (this.durations.TryGetValue(effect, out text)) {
        				// And needs to be removed
        				Destroy(text.transform.parent.gameObject);
        				this.durations.Remove(effect);
        			}
        		}
        	}
        }
	}

	private void UpdateTimer() {
		this.timerText.text = new TimeSpan(0, 0, Mathf.CeilToInt(Time.time - this.start)).ToString("hh'h 'mm'm 'ss's'");
	}

	private void OnLevelStarted(LevelStartedEvent e) {
		this.start = e.Time;
	}

	private void OnEffectsUpdated(EffectsUpdatedEvent e) {
		this.effects = e.Durations;
		this.UpdateEffects();
	}

	private void OnHealthUpdated(HealthUpdatedEvent e) {
		foreach (Transform children in this.healthBar.transform)
			Destroy(children.gameObject);
		int total, count;
		if (e.Total > 10) {
			total = 10;
			count = Mathf.Min(e.Count, 10);
			this.extraHealthText.text = $"+{Mathf.Max(0, e.Count - 10)}/{e.Total - 10}";
			this.extraHealthText.gameObject.SetActive(true);
		} else {
			total = e.Total;
			count = e.Count;
			this.extraHealthText.gameObject.SetActive(false);
		}
		for (int i = 0; i < total; i++)
			Instantiate(i < count ? this.life : this.lifeEmpty, this.healthBar.transform);
	}

	private void OnMoneyUpdated(MoneyUpdatedEvent e) {
		this.moneyAmountText.text = e.Coins.ToString();
	}
}
