using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Interpolation
{
    public class SplineInterpolator
    {
		private  List<float> mX;
		private  List<float> mY;
		private  float[] mM;
		private SplineInterpolator(List<float> x, List<float> y, float[] m)
		{
			mX = x;
			mY = y;
			mM = m;
		}

		/**
		 * Creates a monotone cubic spline from a given set of control points.
		 * 
		 * The spline is guaranteed to pass through each control point exactly. Moreover, assuming the control points are
		 * monotonic (Y is non-decreasing or non-increasing) then the interpolated values will also be monotonic.
		 * 
		 * This function uses the Fritsch-Carlson method for computing the spline parameters.
		 * http://en.wikipedia.org/wiki/Monotone_cubic_interpolation
		 * 
		 * @param x
		 *            The X component of the control points, strictly increasing.
		 * @param y
		 *            The Y component of the control points
		 * @return
		 * 
		 * @throws IllegalArgumentException
		 *             if the X or Y arrays are null, have different lengths or have fewer than 2 values.
		 */
		public static SplineInterpolator createMonotoneCubicSpline(List<float> x, List<float> y)
		{
			if (x == null || y == null || x.Count() != y.Count() || x.Count() < 2)
			{
				Debug.Log("There must be at least two control "
						+ "points and the arrays must be of equal length.");
			}

			int n = x.Count();
			float[] d = new float[n - 1]; // could optimize this out
			float[] m = new float[n];

			// Compute slopes of secant lines between successive points.
			for (int i = 0; i < n - 1; i++)
			{
				float h = x[i + 1] - x[i];
				if (h <= 0f)
				{
					Debug.Log("The control points must all "
							+ "have strictly increasing X values.");
				}
				d[i] = (y[i + 1] - y[i]) / h;
			}

			// Initialize the tangents as the average of the secants.
			m[0] = d[0];
			for (int i = 1; i < n - 1; i++)
			{
				m[i] = (d[i - 1] + d[i]) * 0.5f;
			}
			m[n - 1] = d[n - 2];

			// Update the tangents to preserve monotonicity.
			for (int i = 0; i < n - 1; i++)
			{
				if (d[i] == 0f)
				{ // successive Y values are equal
					m[i] = 0f;
					m[i + 1] = 0f;
				}
				else
				{
					float a = m[i] / d[i];
					float b = m[i + 1] / d[i];
					float h = (float)HypotenuseLength(a, b);
					if (h > 9f)
					{
						float t = 3f / h;
						m[i] = t * a * d[i];
						m[i + 1] = t * b * d[i];
					}
				}
			}
			return new SplineInterpolator(x, y, m);
		}

		/**
		 * Interpolates the value of Y = f(X) for given X. Clamps X to the domain of the spline.
		 * 
		 * @param x
		 *            The X value.
		 * @return The interpolated Y = f(X) value.
		 */
		public float interpolate(float x)
		{
			// Handle the boundary cases.
		    int n = mX.Count();
			if (float.IsNaN(x))
			{
				return x;
			}
			if (x <= mX[0])
			{
				return mY[0];
			}
			if (x >= mX[n - 1])
			{
				return mY[n - 1];
			}

			// Find the index 'i' of the last point with smaller X.
			// We know this will be within the spline due to the boundary tests.
			int i = 0;
			while (x >= mX[i + 1])
			{
				i += 1;
				if (x == mX[i])
				{
					return mY[i];
				}
			}

			// Perform cubic Hermite spline interpolation.
			float h = mX[i + 1] - mX[i];
			float t = (x - mX[i]) / h;
			return (mY[i] * (1 + 2 * t) + h * mM[i] * t) * (1 - t) * (1 - t)
					+ (mY[i + 1] * (3 - 2 * t) + h * mM[i + 1] * (t - 1)) * t * t;
		}


		private static float HypotenuseLength(float sideALength, float sideBLength)
		{
			return Mathf.Sqrt(sideALength * sideALength + sideBLength * sideBLength);
		}

	}

}