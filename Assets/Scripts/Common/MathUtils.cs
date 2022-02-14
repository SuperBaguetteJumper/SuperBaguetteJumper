using System;
using UnityEngine;

namespace Common {
	public static class ExtensionVector3 {
		public static bool IsVector3Valid(this Vector3 vect) {
			return !float.IsNaN(vect.x) && !float.IsInfinity(vect.x)
					&& !float.IsNaN(vect.y) && !float.IsInfinity(vect.y)
					&& !float.IsNaN(vect.z) && !float.IsInfinity(vect.z);
		}
	}

	public class CoordSystem {
		public static Vector3 CylindricShiftedToCartesian(CylindricalShifted cylCoordinates) {
			return new Vector3(
					cylCoordinates.rho * Mathf.Cos(cylCoordinates.theta) + cylCoordinates.shift.x,
					cylCoordinates.rho * Mathf.Sin(cylCoordinates.theta) + cylCoordinates.shift.y,
					cylCoordinates.z
			);
		}

		public static Vector3 CylindricToCartesian(Cylindrical cylCoordinates) {
			return new Vector3(
					cylCoordinates.rho * Mathf.Cos(cylCoordinates.theta),
					cylCoordinates.z,
					cylCoordinates.rho * Mathf.Sin(cylCoordinates.theta)
			);
		}

		public static Cylindrical CartesianToCylindric(Vector3 position) {
			float rho = Mathf.Sqrt(position.x * position.x + position.z * position.z);
			float theta;

			if (position.x == 0) {
				if (position.z == 0)
					theta = 0;
				else if (position.z > 0)
					theta = Mathf.PI / 2f;
				else
					theta = 3 * Mathf.PI / 2f;
			} else if (position.x > 0)
				theta = Mathf.Atan(position.z / position.x);
			else
				theta = Mathf.PI + Mathf.Atan(position.z / position.x);

			while (theta < 0)
				theta += 2 * Mathf.PI;

			float z = position.y;

			return new Cylindrical(rho, theta, z);
		}

		public static Vector3 SphericalToCartesian(Spherical sphCoordinates) {
			return sphCoordinates.rho * new Vector3(
					Mathf.Sin(sphCoordinates.phi) * Mathf.Cos(sphCoordinates.theta),
					Mathf.Cos(sphCoordinates.phi),
					Mathf.Sin(sphCoordinates.phi) * Mathf.Sin(sphCoordinates.theta)
			);
		}

		public static Spherical CartesianToSpherical(Vector3 position) {
			float rho = position.magnitude;
			float theta;
			if (position.x == 0) {
				if (position.z == 0)
					theta = 0;
				else if (position.z > 0)
					theta = Mathf.PI / 2f;
				else
					theta = 3 * Mathf.PI / 2f;
			}
			else if (position.x > 0)
				theta = Mathf.Atan(position.z / position.x);
			else
				theta = Mathf.PI + Mathf.Atan(position.z / position.x);

			while (theta < 0)
				theta += 2 * Mathf.PI;

			float phi = Mathf.PI / 2f;
			if (position.y > 0)
				phi = Mathf.Atan(Mathf.Sqrt(position.x * position.x + position.z * position.z) / position.y);
			else if (position.y < 0)
				phi = Mathf.PI + Mathf.Atan(Mathf.Sqrt(position.x * position.x + position.z * position.z) / position.y);

			return new Spherical(rho, theta, phi);
		}

		[Serializable]
		public struct Cylindrical {
			public float rho;
			public float theta;
			public float z;

			public Cylindrical(Cylindrical cyl) {
				this.rho = cyl.rho;
				this.theta = cyl.theta;
				this.z = cyl.z;
			}

			public Cylindrical(float rho, float theta, float z) {
				this.rho = rho;
				this.theta = theta;
				this.z = z;
			}

			public static Cylindrical Lerp(Cylindrical cyl1, Cylindrical cyl2, float k) {
				return new Cylindrical(
						Mathf.Lerp(cyl1.rho, cyl2.rho, k),
						Mathf.Lerp(cyl1.theta, cyl2.theta, k),
						Mathf.Lerp(cyl1.z, cyl2.z, k)
				);
			}

			public void ConvertThetaToRad() {
				this.theta *= Mathf.Deg2Rad;
			}

			public void ConvertThetaToDeg() {
				this.theta *= Mathf.Rad2Deg;
			}

			public override bool Equals(object obj) {
				if (!(obj is Cylindrical))
					return false;
				Cylindrical cylObj = (Cylindrical) obj;
				return cylObj.rho == this.rho && cylObj.theta == this.theta && cylObj.z == this.z;
			}
		}

		[Serializable]
		public struct Spherical {
			public float rho;
			public float theta;
			public float phi;

			public Spherical(Spherical sph) {
				this.rho = sph.rho;
				this.theta = sph.theta;
				this.phi = sph.phi;
			}

			public Spherical(float rho, float theta, float phi) {
				this.rho = rho;
				this.theta = theta;
				this.phi = phi;
			}

			public static Spherical Lerp(Spherical sph1, Spherical sph2, float k) {
				return new Spherical(
						Mathf.Lerp(sph1.rho, sph2.rho, k),
						Mathf.Lerp(sph1.theta, sph2.theta, k),
						Mathf.Lerp(sph1.phi, sph2.phi, k)
				);
			}

			public void ConvertThetaPhiToRad() {
				this.theta *= Mathf.Deg2Rad;
				this.phi *= Mathf.Deg2Rad;
			}

			public void ConvertThetaPhiToDeg() {
				this.theta *= Mathf.Rad2Deg;
				this.phi *= Mathf.Rad2Deg;
			}

			public override bool Equals(object obj) {
				if (!(obj is Spherical))
					return false;
				Spherical sphObj = (Spherical) obj;
				return sphObj.rho == this.rho && sphObj.theta == this.theta && sphObj.phi == this.phi;
			}
		}

		public struct CylindricalShifted {
			public CylindricalShifted(float rho, Vector2 shift, float theta, float z) {
				this.rho = rho;
				this.shift = shift;
				this.theta = theta;
				this.z = z;
			}

			public float rho;
			public Vector2 shift;
			public float theta;
			public float z;
		}
	}

	public class Funch {
		public static float sh(float t) {
			return (Mathf.Exp(t) - Mathf.Exp(-t)) * .5f;
		}

		public static float ch(float t) {
			return (Mathf.Exp(t) + Mathf.Exp(-t)) * .5f;
		}

		public static float th(float t) {
			return sh(t) / ch(t);
		}

		public static float sech(float t) {
			return 1f / ch(t);
		}

		public static float cosech(float t) {
			return 1f / sh(t);
		}

		public static float argsh(float t) {
			return Mathf.Log(t + Mathf.Sqrt(t * t + 1), 2f);
		}

		public static float argch(float t) {
			if (t >= 1)
				return Mathf.Log(t + Mathf.Sqrt(t * t - 1), 2f);
			return float.NaN;
		}

		public static float argth(float t) {
			if (Mathf.Abs(t) < 1)
				return .5f * Mathf.Log((1 + t) / (1 - t), 2f);
			return float.NaN;
		}

		public static float argcoth(float t) {
			if (Mathf.Abs(t) > 1)
				return .5f * Mathf.Log((1 + t) / (t - 1), 2f);
			return float.NaN;
		}
	}
}
