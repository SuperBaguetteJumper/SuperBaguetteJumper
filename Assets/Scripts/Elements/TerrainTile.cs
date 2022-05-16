using System.Collections;
using Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Elements {
	public class TerrainTile : MonoBehaviour {
		[SerializeField] public int x, z;
		[SerializeField] public TerrainManager manager;
		[SerializeField] public Lambert93 realWorld;

		private Terrain terrain;
		public bool HasGenerated { get; private set; }

		private void Awake() {
			this.terrain = this.gameObject.GetComponent<Terrain>();
		}

		private IEnumerator Start() {
			// Prepare terrain
			this.terrain.enabled = false;
			this.terrain.terrainData.heightmapResolution = this.manager.Size * this.manager.Resolution + 1;
			this.terrain.terrainData.size = new Vector3(this.manager.Size + 1, this.manager.Height, this.manager.Size + 1);

			// Parameters
			int size = this.manager.Size * this.manager.Resolution;
			string bbox = GeoDataUtils.BBOX(this.realWorld, Vector2.zero, new Vector2(this.manager.Size + 1, this.manager.Size + 1));

			// MNS
			MNSRequest mnsRequest = GeoDataUtils.MNSRequest(size, bbox);
			yield return mnsRequest.Execute();
			if (mnsRequest.HasError) {
				Debug.LogWarning($"Terrain tile {this.manager.TileID(this.x, this.z)} failed to load alti on area");
				Destroy(this.gameObject);
				yield break;
			}

			// Transform MNS to heightmap
			mnsRequest.TransformResultToMNS();
			mnsRequest.ScaleMNS(this.manager.MinHeight, this.manager.MaxHeight);

			// Texture
			UnityWebRequest textureRequest = GeoDataUtils.OrthoRequest(size, bbox);
			yield return textureRequest.SendWebRequest();
			if (textureRequest.result != UnityWebRequest.Result.Success) {
				Debug.LogWarning($"Terrain tile {this.manager.TileID(this.x, this.z)} failed to load ortho on area");
				Destroy(this.gameObject);
				yield break;
			}

			// Transform texture to material
			Texture ortho = DownloadHandlerTexture.GetContent(textureRequest);
			Material material = new Material(Shader.Find("Unlit/Texture"));
			material.mainTexture = ortho;
 
			// Apply to terrain
			this.terrain.terrainData.SetHeights(0, 0, mnsRequest.MNS);
			this.terrain.materialTemplate = material;
			this.terrain.enabled = true;
			this.HasGenerated = true;

			// Connect tile to others
			Terrain left = this.GetNeighborIfNull(this.terrain.leftNeighbor, this.x - 1, this.z);
			Terrain top = this.GetNeighborIfNull(this.terrain.topNeighbor, this.x, this.z + 1);
			Terrain right = this.GetNeighborIfNull(this.terrain.rightNeighbor, this.x + 1, this.z);
			Terrain bottom = this.GetNeighborIfNull(this.terrain.bottomNeighbor, this.x, this.z - 1);
			this.terrain.SetNeighbors(left, top, right, bottom);
		}

		private Terrain GetNeighborIfNull(Terrain neighbor, int neighborX, int neighborZ) {
			if (neighbor != null)
				return neighbor;
			TerrainTile tile;
			if (this.manager.Tiles.TryGetValue(this.manager.TileID(neighborX, neighborZ), out tile))
				return tile.GetComponent<Terrain>();
			return null;
		}
	}
}
