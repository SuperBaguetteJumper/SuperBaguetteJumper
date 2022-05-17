using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Common {
	public class GeoDataUtils {
		public static string BBOX(Lambert93 coords, Vector2 offsetMin, Vector2 offsetMax) {
			CsharpUtils.FixCsharpBadDecimalSeparator();
			return $"{coords.x - offsetMin.x},{coords.z - offsetMin.y},{coords.x + offsetMax.x},{coords.z + offsetMax.y}";
		}

		public static MNSRequest MNSRequest(int size, string bbox) {
			return new MNSRequest(WXSRequestURL("ELEVATION.ELEVATIONGRIDCOVERAGE.HIGHRES.MNS", "altimetrie", size, bbox, "image/x-bil;bits=32"));
		}

		public static UnityWebRequest OrthoRequest(int size, string bbox) {
			return UnityWebRequestTexture.GetTexture(WXSRequestURL("ORTHOIMAGERY.ORTHOPHOTOS.BDORTHO", "ortho", size, bbox, "image/png"));
		}

		public static string WXSRequestURL(string layer, string key, int size, string bbox, string format) {
			return $"https://wxs.ign.fr/{key}/geoportail/r/wms?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap" +
			       $"&layers={layer}&CRS=EPSG:2154&BBOX={bbox}&WIDTH={size}&HEIGHT={size}&FORMAT={format}&STYLES=";
		}
	}

	public class MNSRequest {
		private string url;
		private byte[] result;
		public float[,] MNS { get; private set; }
		public bool HasError { get; private set; }
		public int Size { get; private set; }
		public float Min { get; private set; }
		public float Max { get; private set; }

		public MNSRequest(string url) {
			this.url = url;
		}

		public IEnumerator Execute() {
			UnityWebRequest mnsRequest = UnityWebRequest.Get(this.url);
			yield return mnsRequest.SendWebRequest();
			if (mnsRequest.result == UnityWebRequest.Result.Success) {
				this.result = mnsRequest.downloadHandler.data;
				this.Size = Mathf.FloorToInt(Mathf.Sqrt(this.result.Length / 4f));
			} else {
				this.HasError = true;
			}
		}

		public void TransformResultToMNS() {
            this.MNS = new float[this.Size, this.Size];
            this.Min = float.MaxValue;
			this.Max = float.MinValue;
            for (int i = 0; i < this.Size; i++) {
                for (int j = 0; j < this.Size; j++) {
                    float height = BitConverter.ToSingle(this.result, i * this.Size * 4 + j * 4);
                    this.MNS[this.Size - i - 1, j] = height;
                    this.Min = Mathf.Min(this.Min, height);
                    this.Max = Mathf.Max(this.Max, height);
                }
            }
		}

		public void ScaleMNS(float min, float max) {
			float diff = max - min;
            for (int i = 0; i < this.Size; i++)
                for (int j = 0; j < this.Size; j++)
                    this.MNS[i, j] = (this.MNS[i, j] - min) / diff;
		}
	}

	[Serializable]
	public struct Lambert93 {
		public double x;
		public double y;
		public double z;

		public Lambert93(double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
}
