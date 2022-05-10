using System.Collections.Generic;
using Elements.Flags;
using Events;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	[Header("Level Manager")]
	[SerializeField] private Flag start;
	[SerializeField] private List<Flag> checkpoints;
	[SerializeField] private Flag end;
	[SerializeField] private int lives = 3;

	private int lastCheckpoint = -1;

	public Transform RespawnPoint {
		get => this.lastCheckpoint < 0 || this.lastCheckpoint >= this.checkpoints.Count ? this.start.transform : this.checkpoints[this.lastCheckpoint].transform;
	}

	protected void Awake() {
		Instantiate(Resources.Load<GameObject>("Player"), this.start.transform.position, this.start.transform.rotation * Quaternion.LookRotation(Vector3.right));
		EventManager.Instance.AddListener<CheckpointReachedEvent>(this.OnCheckpointReached);
		EventManager.Instance.AddListener<EndReachedEvent>(this.OnEndReached);
		EventManager.Instance.AddListener<PlayerDiedEvent>(this.OnPlayerDied);
		EventManager.Instance.AddListener<CoinObjectPickedUpEvent>(this.OnCoinObjectPickedUp);
	}

	private void OnDestroy() {
		EventManager.Instance.RemoveListener<CheckpointReachedEvent>(this.OnCheckpointReached);
		EventManager.Instance.RemoveListener<EndReachedEvent>(this.OnEndReached);
		EventManager.Instance.RemoveListener<PlayerDiedEvent>(this.OnPlayerDied);
		EventManager.Instance.RemoveListener<CoinObjectPickedUpEvent>(this.OnCoinObjectPickedUp);
	}

	private void OnCheckpointReached(CheckpointReachedEvent e) {
		int checkpoint = this.checkpoints.IndexOf(e.Flag);
		if (checkpoint > this.lastCheckpoint)
			this.lastCheckpoint = checkpoint;
	}

	private void OnEndReached(EndReachedEvent e) {
		Debug.Log("You won!");
	}

	private void OnPlayerDied(PlayerDiedEvent e) {
		if (this.lives <= 0)
			Debug.Log("You lost!");
		else
			this.lives--;
		Transform respawnPoint = this.RespawnPoint;
	    e.Player.transform.position = respawnPoint.position;
	    e.Player.transform.rotation = respawnPoint.rotation * Quaternion.LookRotation(Vector3.right);
	    e.Player.rigidbody.velocity = Vector3.zero;
	    e.Player.ResetCamera();
	}

	private void OnCoinObjectPickedUp(CoinObjectPickedUpEvent e) {
		Debug.Log("+" + e.Object.Value + " money");
	}
}
