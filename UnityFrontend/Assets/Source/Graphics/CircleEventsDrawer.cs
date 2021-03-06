﻿using System.Collections.Generic;
using System.Linq;
using C5;
using Generator;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Graphics
{
    public class CircleEventsDrawer
    {
        public static int NumberOfVerticesPerCircle = 50;

        private readonly GameObject _parentObject;
        private readonly Dictionary<CircleEvent, GameObject> _gameObjects;
        private readonly CircleEventQueue _circleEvents;

        public CircleEventsDrawer(CircleEventQueue circleEvents)
        {
            _circleEvents = circleEvents;
            _parentObject = new GameObject("Circle Events");
            _gameObjects = new Dictionary<CircleEvent, GameObject>();
        }

        public void Update()
        {
            var newCircleEvents = _circleEvents.ToArray().Except(_gameObjects.Keys);

            foreach (var circleEvent in newCircleEvents)
            {
                var vertices = CircleEventVertices(circleEvent);
                var gameObject = DrawingUtilities.CreateLineObject("Circle " + circleEvent, vertices, "CircleEventMaterial");
                gameObject.transform.parent = _parentObject.transform;

                _gameObjects.Add(circleEvent, gameObject);
            }

            var removedCircles = _gameObjects.Keys.Except(_circleEvents.ToArray()).ToList();

            foreach (var circleEvent in removedCircles)
            {
                var gameObject = _gameObjects[circleEvent];
                _gameObjects.Remove(circleEvent);
                Object.Destroy(gameObject);
            }
        }

        private Vector3[] CircleEventVertices(CircleEvent circle)
        {
            var angles = DrawingUtilities.AnglesInRange(0, 2 * Mathf.PI, NumberOfVerticesPerCircle);

            var points = angles.Select(angle => VertexOfCircleEvent(circle, angle)).ToArray();

            return points;
        }

        private Vector3 VertexOfCircleEvent(CircleEvent circle, float angle)
        {
            var a = circle.LeftArc.Site.Position.ToUnityVector3();
            var b = circle.MiddleArc.Site.Position.ToUnityVector3();
            var c = circle.RightArc.Site.Position.ToUnityVector3();

            var n = Vector3.Cross(a - b, c - b).normalized;

            var s = a - Vector3.Dot(a, n) * n;
            var t = Vector3.Cross(s, n);

            var vertex = Vector3.Dot(a, n) * n + Mathf.Cos(angle) * s + Mathf.Sin(angle) * t;

            return vertex;
        }
    }
}
