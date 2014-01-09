﻿using System;

namespace Generator
{
    public class CircleEvent : IComparable<CircleEvent>
    {
        public readonly IArc LeftArc;
        public readonly IArc MiddleArc;
        public readonly IArc RightArc;

        public double Priority { get; private set; }

        public CircleEvent(IArc leftArc, IArc middleArc, IArc rightArc)
        {
            LeftArc = leftArc;
            MiddleArc = middleArc;
            RightArc = rightArc;

            Priority = CalculatePriority(leftArc.Site.Position, middleArc.Site.Position, rightArc.Site.Position);
        }

        private static double CalculatePriority(Vector3 a, Vector3 b, Vector3 c)
        {
            var v = (a - b).CrossMultiply(c - b);

            var vz = v[2];
            var va = v.ScalarMultiply(a);

            var z = va*vz - Math.Sqrt((1 - vz*vz)*(1 - va*va));
            var sign = va > -vz ? 1 : -1;

            return sign*(1 + z);
        }

        public int CompareTo(CircleEvent other)
        {
            return this.Priority.CompareTo(other.Priority);
        }
    }
}
