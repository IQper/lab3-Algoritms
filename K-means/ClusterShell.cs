using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K_means
{
    class ClusterShell
    {
        public List<Cluster> Clusters { get; private set; } = new();
        public double V
        {
            get
            {
                double v = 0;
                foreach (var cluster in Clusters)
                {
                    v += cluster.DotVariation;
                }
                return v;
            }
        }

        public void Add(Cluster cluster)
        {
            Clusters.Add(cluster);
        }

        public void ClearClusters()
        {
            foreach(var cluster in Clusters)
            {
                cluster.Clear();
            }
        }
    }
}
