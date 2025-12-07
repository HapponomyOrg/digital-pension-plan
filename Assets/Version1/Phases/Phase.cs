using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Version1.Phases
{
    public class Phase : IPhase
    {
        public string Name { get; }

        public string Scene { get; }

        public Phase(string name, string scene)
        {
            Name = name;
            Scene = scene;
        }
    }
}
