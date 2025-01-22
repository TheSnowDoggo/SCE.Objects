namespace SCE
{
    public class RangeOccluder : ComponentBase<World>, IObjectCacheable
    {
        private readonly List<SCEObject> _objectCache = new();

        private int framesPerUpdate = 1;

        private int updateCount = 0;

        public RangeOccluder(string name, SCEObject target, double range)
            : base(name)
        {
            Target = target;
            Range = range;
        }

        public RangeOccluder(SCEObject target, double range)
            : this("range_occluder", target, range)
        {
        }

        public SCEObject Target { get; set; }

        public double Range { get; set; }

        public IList<SCEObject> ObjectCache { get => _objectCache.AsReadOnly(); }

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

        public bool ObjectCaching { get; set; } = false;

        public override void Update()
        {
            if (++updateCount < FramesPerUpdate)
                return;
            updateCount = 0;

            if (ObjectCaching)
                _objectCache.Clear();

            foreach (var obj in Parent)
            {
                if (obj != Target && !ExclusionSet.Contains(obj) && !obj.CContainer.Contains<RangeOccluderExcluder>())
                    obj.IsActive = obj.Position.DistanceFrom(Target.Position) <= Range;
                if (ObjectCaching && obj.IsActive)
                    _objectCache.Add(obj);
            }
        }
    }
}
