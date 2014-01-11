﻿using System;
using System.Collections.Generic;
using System.Linq;
using Generator;
using Graphics;
using MathNet.Numerics;
using UnityEngine;
using Random = System.Random;

public class load : MonoBehaviour
{
    private Random _random = new Random();

    private VoronoiDiagramDrawer _drawer;
    private VoronoiDiagram _diagram;

    private bool _hasFailed = false;

	// Use this for initialization
	void Start ()
	{
	    var positions = new List<double[]>
	    {
	        VectorAt(0, 0),
	        VectorAt(45, -45),
	        VectorAt(45, 45),
	        VectorAt(45, 135),
            VectorAt(90, 0),
            VectorAt(90, 90)
	    };
        positions = Enumerable.Range(0, 1000).Select(i => CreateSphericalVector()).ToList();

        _diagram = new VoronoiDiagram(positions);
	    _drawer = new VoronoiDiagramDrawer(_diagram);
	}
	
	// Update is called once per frame
	void Update () {

        //if (!_hasFailed && (Input.GetKey(KeyCode.N) || Input.GetKeyDown(KeyCode.F)))
        if (!_hasFailed)
	    {
	        try
	        {
	            _diagram.ProcessNextEvent();
	        }
	        catch (Exception exception)
	        {
	            _hasFailed = true;
                Debug.Log(exception);
	        }
            _drawer.UpdateVoronoiDiagram();
	    }
	}

    private double[] CreateSphericalVector()
    {
        var z = (float)(-1 + 2*_random.NextDouble());
        var azimuth = (float)(2 * Mathf.PI * _random.NextDouble());

        var x = Mathf.Sqrt(1 - z*z) * Mathf.Cos(azimuth);
        var y = Mathf.Sqrt(1 - z*z) * Mathf.Sin(azimuth);

        return new double[] {x, y, z};
    }

    private double[] VectorAt(double colatitude, float azimuth)
    {
        colatitude = Mathf.Deg2Rad*colatitude;
        azimuth = Mathf.Deg2Rad*azimuth;

        var x = Trig.Sine(colatitude)*Trig.Cosine(azimuth);
        var y = Trig.Sine(colatitude)*Trig.Sine(azimuth);
        var z = Trig.Cosine(colatitude);

        return new double[] {x, y, z};
    }
}
