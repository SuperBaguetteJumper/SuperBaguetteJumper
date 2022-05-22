using System.Collections;
using System.Collections.Generic;
using Common;
using Elements;
using UnityEngine;

public class TerrainManager : MonoBehaviour {
	[field: SerializeField] public int Size { get; private set; } = 128;
	[field: SerializeField] public int Resolution { get; private set; } = 1;
	[SerializeField] private Lambert93 coords; // Example: new Lambert93(652285.51, 100, 6861635.77)
	[SerializeField] private int renderDistance = 5;
	[SerializeField] private int maxConcurrentGenerations = 4;
	[SerializeField] private int minMaxEvaluationScale = 5;
	[SerializeField] private int minMaxEvaluationSampling = 16;

	public float MinHeight { get; private set; }
	public float MaxHeight { get; private set; }
	public float Height {
		get => this.MaxHeight - this.MinHeight;
	}

	public Dictionary<string, TerrainTile> Tiles { get; private set; } = new Dictionary<string, TerrainTile>();
	private PriorityQueue<(int x, int z)> generationQueue = new PriorityQueue<(int x, int z)>(new TileIDComparer());

	private IEnumerator Start() {
		yield return this.ComputeMinMax();
		for (int i = -this.renderDistance; i < this.renderDistance; i++)
			for (int j = -this.renderDistance; j < this.renderDistance; j++)
				this.generationQueue.Enqueue((i, j));
		for (int i = 0; i < this.maxConcurrentGenerations; i++)
			this.StartCoroutine(this.TilesGenerationFromQueue());
	}

	private IEnumerator TilesGenerationFromQueue() {
		while (this.enabled) {
			yield return new WaitWhile(() => this.generationQueue.IsEmpty);
			(int x, int z) = this.generationQueue.Dequeue();
			TerrainTile tile = this.AddTile(x, z);
			yield return new WaitUntil(() => tile.gameObject == null || tile.HasGenerated);
		}
	}

	private TerrainTile AddTile(int x, int z) {
		string key = this.TileID(x, z);
		if (this.Tiles.ContainsKey(key))
			return this.Tiles[key];
		GameObject terrain = Terrain.CreateTerrainGameObject(new TerrainData());
		terrain.transform.parent = this.transform;
		terrain.name = $"TerrainTile.{key}";
		terrain.transform.position = new Vector3(x * this.Size, (float) -this.coords.y, z * this.Size);
		//terrain.transform.localScale = new Vector3(1f / this.Resolution, 1, 1f / this.Resolution);
		TerrainTile tile = terrain.AddComponent<TerrainTile>();
		tile.x = x;
		tile.z = z;
		tile.manager = this;
		tile.realWorld = new Lambert93(this.coords.x + x * this.Size, 0, this.coords.z + z * this.Size);
		this.Tiles[key] = tile;
		return tile;
	}

	private IEnumerator ComputeMinMax() {
		int size = this.minMaxEvaluationSampling * this.minMaxEvaluationScale;
		int offsetValue = this.Size * this.minMaxEvaluationScale;
		Vector2 offset = new Vector2(offsetValue, offsetValue);
		string bbox = GeoDataUtils.BBOX(this.coords, offset, offset);

		MNSRequest mnsRequest = GeoDataUtils.MNSRequest(size, bbox);
		yield return mnsRequest.Execute();
		if (mnsRequest.HasError) {
			Debug.LogWarning("Terrain manager failed to load global alti on area");
			Destroy(this.gameObject);
			yield break;
		}

		mnsRequest.TransformResultToMNS();
		this.MinHeight = mnsRequest.Min;
		this.MaxHeight = mnsRequest.Max;
	}

	public string TileID(int x, int z) => $"{x},{z}";
}

public class TileIDComparer : Comparer<(int x, int z)> {
	public override int Compare((int x, int z) a, (int x, int z) b) {
		if (a == b)
			return 0;
		int cmp = Mathf.Abs(a.x) + Mathf.Abs(a.z) - Mathf.Abs(b.x) - Mathf.Abs(b.z);
		if (cmp == 0) {
			cmp = a.x - b.x;
			return cmp == 0 ? a.z - b.z : cmp;
		}
		return cmp;
	}
}
