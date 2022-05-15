using System.Collections.Generic;
using Elements.Flags;
using Events;
using Objects;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	[Header("Level Manager")]
	[SerializeField] private Flag start;
	[SerializeField] private List<Flag> checkpoints;
	[SerializeField] private Flag end;
	[SerializeField] private int initialLives = 3;

	private int lastCheckpoint = -1;
	private int lives;
	private int coins;
	private PhysicalObject[] collectibles;

	public Transform RespawnPoint {
		get => this.lastCheckpoint < 0 || this.lastCheckpoint >= this.checkpoints.Count ? this.start.transform : this.checkpoints[this.lastCheckpoint].transform;
	}

	protected void Awake() {
		this.lives = this.initialLives;
		this.collectibles = FindObjectsOfType<PhysicalObject>();
		Instantiate(Resources.Load<GameObject>("Player"));
		EventManager.Instance.AddListener<CheckpointReachedEvent>(this.OnCheckpointReached);
		EventManager.Instance.AddListener<EndReachedEvent>(this.OnEndReached);
		EventManager.Instance.AddListener<PlayerDiedEvent>(this.OnPlayerDied);
		EventManager.Instance.AddListener<CoinObjectPickedUpEvent>(this.OnCoinObjectPickedUp);
		EventManager.Instance.AddListener<BaguetteObjectPickedUpEvent>(this.OnBaguetteObjectPickedUp);
	}

	private void Start() {
		EventManager.Instance.Raise(new PlayerSpawnedEvent(this.start.transform));
	}

	private void OnDestroy() {
		EventManager.Instance.RemoveListener<CheckpointReachedEvent>(this.OnCheckpointReached);
		EventManager.Instance.RemoveListener<EndReachedEvent>(this.OnEndReached);
		EventManager.Instance.RemoveListener<PlayerDiedEvent>(this.OnPlayerDied);
		EventManager.Instance.RemoveListener<CoinObjectPickedUpEvent>(this.OnCoinObjectPickedUp);
		EventManager.Instance.RemoveListener<BaguetteObjectPickedUpEvent>(this.OnBaguetteObjectPickedUp);
	}

	private void OnCheckpointReached(CheckpointReachedEvent e) {
		int checkpoint = this.checkpoints.IndexOf(e.Flag);
		if (checkpoint > this.lastCheckpoint)
			this.lastCheckpoint = checkpoint;
	}

	private void OnEndReached(EndReachedEvent e) {
		if (e.Flag == this.end) {
			Debug.Log("You won!");
			EventManager.Instance.Raise(new LevelWonEvent(this.coins));
		}
	}

	private void OnPlayerDied(PlayerDiedEvent e) {
		if (this.lives <= 0) {
			Debug.Log("You lost!");
			EventManager.Instance.Raise(new LevelLostEvent());
		} else {
			this.lives--;
			for (int i = 0; i < this.collectibles.Length; i++)
				if (this.collectibles[i].ReactivateOnRespawn)
					this.collectibles[i].gameObject.SetActive(true);
			EventManager.Instance.Raise(new PlayerSpawnedEvent(this.RespawnPoint));
		}
	}

	private void OnCoinObjectPickedUp(CoinObjectPickedUpEvent e) {
		this.coins += e.Object.Value;
	}

	private void OnBaguetteObjectPickedUp(BaguetteObjectPickedUpEvent e) {
		if (this.lives <= this.initialLives)
			this.lives++;
		else
			e.CanPickup = false;
	}
}
