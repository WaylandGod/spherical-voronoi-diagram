﻿using SphericalVoronoiDiagramTests.DataAttributes;
using UnityEngine;
using Xunit;
using Xunit.Extensions;
using Debug = System.Diagnostics.Debug; 

namespace SphericalVoronoiDiagramTests
{
    public class ArcTests
    {
        private const int Tolerance = 3;

        [Theory]
        [SphericalVectorAndSweeplineData]
        public void AzimuthOfLeftIntersection_WhenLeftNeighbourSiteIsTheSame_ShouldReturnAzimuthOfTheSite
            (Site siteA, Sweepline sweepline)
        {
            // Fixture setup
            var sut = new Arc(siteA, sweepline);

            var expectedResult = sut.Site.Azimuth;

            // Exercise system
            var result = sut.AzimuthOfLeftIntersection();

            // Verify outcome
            Assert.Equal(result, expectedResult);

            // Teardown
        }

        [Theory]
        [SphericalVectorAndSweeplineData]
        public void AzimuthOfLeftIntersection_WhenBothSitesAreDefined_ShouldBeEquidistantFromBothSitesAndTheSweeplineWhenConvertedToAPointOnTheEllipse
            (Site siteA, Site siteB, Sweepline sweepline)
        {
            // Fixture setup
            var sut = new Arc(siteA, sweepline) {LeftNeighbour = siteB, RightNeighbour = siteB};

            // Exercise system
            var result = sut.AzimuthOfLeftIntersection();

            // Verify outcome
            var pointOnEllipse = EllipseDrawer.PointOnEllipse(sut.Site.Position, sweepline.Height, result);

            var distanceToLeftNeighbour = Mathf.Acos(Vector3.Dot(pointOnEllipse, sut.LeftNeighbour.Position));
            var distanceToSite = Mathf.Acos(Vector3.Dot(pointOnEllipse, sut.Site.Position));
            var distanceToSweepline = Mathf.Abs(Mathf.Acos(sweepline.Height) - Mathf.Acos(pointOnEllipse.z));

            Assert.Equal(distanceToLeftNeighbour, distanceToSite, Tolerance);
            Assert.Equal(distanceToSite, distanceToSweepline, Tolerance);
            Assert.Equal(distanceToSweepline, distanceToLeftNeighbour, Tolerance);

            // Teardown
        }


    }
}
