using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platforms
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private bool useCurrentPosAsFirstCoord = true;
        [SerializeField] private List<Vector3> coordinates;
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float initialDelay;
        [SerializeField] private float waitDelay;
        [SerializeField] private AnimationCurve curve;
        
        private void Awake()
        {
            if (this.useCurrentPosAsFirstCoord)
            {
                this.coordinates.Insert(0,this.transform.position);
            }
            if (this.coordinates.Count < 2)
            {
                Debug.LogWarning($"not enough coordinates: {this.name} destroyed.");
                Destroy(this.gameObject);
                return;
            }

            if (!this.useCurrentPosAsFirstCoord)
            {
                this.transform.position = this.coordinates[0];
            }
            
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(this.initialDelay);
            while (this.enabled)
            {
                for (int i = 0; i < this.coordinates.Count; i++)
                {
                    Vector3 from = this.coordinates[i];
                    Vector3 to = this.coordinates[i + 1 == this.coordinates.Count ? 0 : i + 1];
                    yield return Translation(from, to);
                    yield return new WaitForSeconds(this.waitDelay);
                }
            }
        }

        private IEnumerator Translation(Vector3 from, Vector3 to)
        {
            float start = Time.time;
            float length = Vector3.Distance(from,to) / this.speed;
            float duration;
            while ((duration = Time.time - start) < length) {
                float progress = duration / length;
                this.transform.position = Vector3.Lerp(from,to,this.curve.Evaluate(progress));
                yield return null;
            }
            this.transform.position = to;
        }
    }
}