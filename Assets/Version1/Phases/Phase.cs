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
