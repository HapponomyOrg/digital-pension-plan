using System;

namespace Version1.Phases.tmp
{
    public abstract class Phase
    {
        public abstract event EventHandler<EventArgs> InitFinished;
        public abstract bool InitComplete { get; protected set; }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
    }
}
