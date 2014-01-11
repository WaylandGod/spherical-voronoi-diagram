﻿using System;

namespace Generator
{
    public class Arc : IArc
    {
        public SiteEvent Site { get; set; }
        public SiteEvent LeftNeighbour { get; set; }

        public override string ToString()
        {
            return Site.ToString();
        }
    }
}
