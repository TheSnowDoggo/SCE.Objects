namespace SCE
{
    public class RangeOccluder : ComponentBase<World>
    {
        private int framesPerUpdate = 1;

        private int updateCount = 0;

        public RangeOccluder(string name, SCEObject obj, double range)
            : base(name)
        {
            Object = obj;
            Range = range;
        }

        public RangeOccluder(SCEObject obj, double range)
            : this("range_occluder", obj, range)
        {
        }

        public SCEObject Object { get; set; }

        public double Range { get; set; }

        public int FramesPerUpdate
        {
            get => framesPerUpdate;
            set
            {
                if (value < 1)
                    throw new ArgumentException("Update frequency must be greater than 0.");
                framesPerUpdate = value;
                updateCount = FramesPerUpdate;
            }
        }

        public HashSet<SCEObject> ExclusionSet { get; set; } = new();

        public override void Update()
        {
            if (++updateCount < FramesPerUpdate)
                return;
            updateCount = 0;

            foreach (var obj in Parent)
            {
                if (obj != Object && !ExclusionSet.Contains(obj))
                    obj.IsActive = obj.Position.DistanceFrom(Object.Position) <= Range;
            }
        }
    }
}
