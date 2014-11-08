using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pocal.ViewModel;

namespace Pocal.Helper
{
    class Cluster
    {
        public List<PocalAppointment> members;
        public int maxConflicts { get; set; }

        public Cluster (PocalAppointment pa)
        {
            members = new List<PocalAppointment>(); 
            members.Add(pa);
        }

    }
}
