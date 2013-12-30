﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BeachlineDrawer
{
    public static int PointsPerArc = 100;

    public static void DrawBeachline(Beachline beachline)
    {
        foreach (var arc in beachline)
        {
            var arcObject = new GameObject("Arc" + arc);
            var arcMeshFilter = arcObject.AddComponent<MeshFilter>();
            arcObject.AddComponent<MeshRenderer>();

            var arcMesh = arcMeshFilter.mesh;

            var azimuths = AzimuthsInRange(arc.AzimuthOfLeftIntersection(), arc.AzimuthOfRightIntersection());
            var pointsOnArc = PointsOnArc(arc, azimuths).ToList();

            arcMesh.vertices = pointsOnArc.ToArray();
            arcMesh.SetIndices(
                Enumerable.Range(0, pointsOnArc.Count).ToArray(), 
                MeshTopology.LineStrip, 
                0);
        }
    }

    private static IEnumerable<Vector3> PointsOnArc(Arc arc, IEnumerable<float> azimuths)
    {
        var points = azimuths.Select(azimuth => PointOnEllipse(arc, azimuth)).ToList();

        return points;
    }

    private static IEnumerable<float> AzimuthsInRange(float leftAzimuth, float rightAzimuth)
    {
        if (leftAzimuth <= rightAzimuth)
        {
            var distance = rightAzimuth - leftAzimuth;
            return Enumerable.Range(0, PointsPerArc+1).Select(i => leftAzimuth + i*distance/PointsPerArc);
        }
        else
        {
            var distanceFromLeftToZero = Mathf.Abs(2*Mathf.PI - leftAzimuth);
            var distanceFromRightTo2Pi = Mathf.Abs(rightAzimuth);
            var distance = distanceFromLeftToZero + distanceFromRightTo2Pi;

            var numberOfpointsFromLeft = Mathf.FloorToInt(PointsPerArc * distanceFromLeftToZero / distance);
            var numberOfpointsFromRight = Mathf.CeilToInt(PointsPerArc * distanceFromRightTo2Pi / distance);

            var pointsFromLeft =
                Enumerable.Range(0, numberOfpointsFromLeft+1)
                    .Select(i => leftAzimuth + i*distanceFromLeftToZero/numberOfpointsFromLeft);

            var pointsFromRight =
                Enumerable.Range(0, numberOfpointsFromRight+1)
                    .Select(i => i * distanceFromRightTo2Pi / numberOfpointsFromRight);

            return pointsFromLeft.Concat(pointsFromRight);
        }
    }

    public static Vector3 PointOnEllipse(Arc arc, float azimuth)
    {
        var p = arc.SiteEvent.Position;
        var Z = arc.Sweepline.Z;

        var tx = Mathf.Cos(azimuth);
        var ty = -Mathf.Sin(azimuth);

        //var Y = p.z - Z;
        //var X = Mathf.Sqrt(1 - Z * Z) - (p.x * tx - p.y * ty);

        //var x = Y / Mathf.Sqrt(X * X + Y * Y) * Mathf.Cos(azimuth);
        //var y = Y / Mathf.Sqrt(X * X + Y * Y) * -Mathf.Sin(azimuth);
        //var z = X / Mathf.Sqrt(X * X + Y * Y);

        var tz = (Z - p.z) / (p.x * tx + p.y * ty - Mathf.Sqrt(1 - Z * Z));

        var x = 1/Mathf.Sqrt(1 + 1/(tz*tz))*tx;
        var y = 1/Mathf.Sqrt(1 + 1/(tz*tz))*ty;
        var z = Mathf.Sign(tz)*1/Mathf.Sqrt(1 + tz*tz);

        ////var z = Mathf.Sqrt(Mathf.Pow(p.x*tx - p.y*ty - Mathf.Sqrt(1-Z*Z), 2) + Mathf.Pow(p.z - Z, 2)) / (p.z - Z);
        //var tz = (Z - p.z) / (p.x * tx - p.y * ty - Mathf.Sqrt(1 - Z * Z));

        //var z = 1 / Mathf.Sqrt(tz * tz - 1);
        //var x = tx * Mathf.Sqrt(1 - z * z);
        //var y = -ty * Mathf.Sqrt(1 - z * z);

        return new Vector3(x, y, z);
    }
}