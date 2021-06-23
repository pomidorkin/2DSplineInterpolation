using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpolation;

public class Test : MonoBehaviour
{
    SplineInterpolator spline;
    void Start()
    {
        List<float> xs = new List<float>();
        List<float> ys = new List<float>();

        xs.Add(1f);
        xs.Add(2f);
        xs.Add(3f);
        ys.Add(1f);
        ys.Add(4f);
        ys.Add(2f);


        SplineInterpolator mySpline =  SplineInterpolator.createMonotoneCubicSpline(xs, ys);
        Debug.Log(mySpline.interpolate(1.9f));
    }

}
