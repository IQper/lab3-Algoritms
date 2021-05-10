using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace K_means
{
    class Cluster
    {
        public List<Dot> Dots { get; private set; }
        public Color Color
        {
            get
            {
                return Mean.Color;
            }
            set
            {
                Mean.Color = value;
            }
        }
        public Dot Mean = new Dot(100, 100);
        public double DotVariation
        {
            get
            {
                double variation = 0;
                foreach (var dot in Dots)
                {
                    variation += Mean.DistanceTo(dot) * Mean.DistanceTo(dot);
                }
                return variation;
            }
        }

        
        public Cluster()
        {
            Dots = new();
        }

        public Cluster(Dot mean, Color color)
        {
            Mean = mean;
            Color = color;
            Dots = new();
        }

        public void Add(Dot dot)
        {
            Dots.Add(new Dot(dot.X, dot.Y, Color));
        }

        public void Clear()
        {
            Dots.Clear();
        }

        public Dot RecalculateMean()
        {
            var mean = new Dot(0, 0, Mean.Color);
            if (Dots.Count == 0) return Mean;

            foreach(var dot in Dots)
            {
                mean += dot;
            }

            mean /= Dots.Count;
            Mean = mean;
            return Mean;
        }
    }
}
