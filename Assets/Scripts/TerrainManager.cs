using System.Collections;
using System.Collections.Generic;
using Common;
using Elements;
using UnityEngine;

public class TerrainManager : MonoBehaviour {
	[field: SerializeField] public int Size { get; private set; } = 128;
	[field: SerializeField] public int Resolution { get; private set; } = 1;
	[SerializeField] private Lambert93 coords; // Example: new Lambert93(652285.51, 100, 6861635.77)
	[SerializeField] private int minMaxEvaluationScale = 5;
	[SerializeField] private int minMaxEvaluationSampling = 16;

	public float MinHeight { get; private set; }
	public float MaxHeight { get; private set; }
	public float Height {
		get => this.MaxHeight - this.MinHeight;
	}

	public Dictionary<string, TerrainTile> Tiles { get; private set; } = new Dictionary<string, TerrainTile>();

	private IEnumerator Start() {
		yield return this.ComputeMinMax();
		for (int i = -2; i < 2; i++)
			for (int j = -2; j < 2; j++)
				this.AddTile(i, j);
	}

	private void AddTile(int x, int z) {
		string key = this.TileID(x, z);
		GameObject terrain = Terrain.CreateTerrainGameObject(new TerrainData());
		terrain.name = $"TerrainTile.{key}";
		terrain.transform.position = new Vector3(x * this.Size, (float) -this.coords.y, z * this.Size);
		//terrain.transform.localScale = new Vector3(1f / this.Resolution, 1, 1f / this.Resolution);
		TerrainTile tile = terrain.AddComponent<TerrainTile>();
		tile.x = x;
		tile.z = z;
		tile.manager = this;
		tile.realWorld = new Lambert93(this.coords.x + x * this.Size, 0, this.coords.z + z * this.Size);
		this.Tiles.Add(key, tile);
	}

	private IEnumerator ComputeMinMax() {
		int size = this.minMaxEvaluationSampling * this.minMaxEvaluationScale;
		int offsetValue = this.Size * this.minMaxEvaluationScale;
		Vector2 offset = new Vector2(offsetValue, offsetValue);
		string bbox = GeoDataUtils.BBOX(this.coords, offset, offset);

		MNSRequest mnsRequest = GeoDataUtils.MNSRequest(size, bbox);
		yield return mnsRequest.Execute();
		if (mnsRequest.HasError) {
			Destroy(this.gameObject);
			yield break;
		}

		mnsRequest.TransformResultToMNS();
		this.MinHeight = mnsRequest.Min;
		this.MaxHeight = mnsRequest.Max;
	}

	public string TileID(int x, int z) => $"{x},{z}";
}
