namespace SCE
{
    public class RangeOccluder : ComponentBase<World>, IUpdate, IObjectCacheable
    {
        private const string DEFAULT_NAME = "range_occluder";

        private readonly List<SCEObject> _objectCache = new();

        public RangeOccluder(string name, IEnumerable<SCEObject> targets, double range)
            : base(name)
        {
            TargetSet = new(targets);
            Range = range;
        }

        public RangeOccluder(IEnumerable<SCEObject> targets, double range)
            : this(DEFAULT_NAME, targets, range)
        {
        }

        public RangeOccluder(string name, double range)
            : base(name)
        {
            TargetSet = new();
            Range = range;
        }

        public RangeOccluder(double range)
            : this(DEFAULT_NAME, range)
        {
        }

        public HashSet<SCEObject> TargetSet { get; set; }

        public double Range { get; set; }

        public IList<SCEObject> ObjectCache { get => _objectCache.AsReadOnly(); }

        public IUpdateLimit? UpdateLimiter { get; set; }

        public HashSet<SCEObject> ExclusionSet { get; set; } = new();

        public bool ObjectCaching { get; set; } = false;

        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
                return;

            if (ObjectCaching)
                _objectCache.Clear();

            foreach (var obj in Holder)
            {
                if (!TargetSet.Contains(obj) && !ExclusionSet.Contains(obj) && !obj.Components.Contains<RangeOccluderExcluder>())
                    obj.IsActive = IsObjectOccluded(obj);
                if (ObjectCaching && obj.IsActive && !obj.Components.IsEmpty)
                    _objectCache.Add(obj);
            }
        }

        public bool IsObjectOccluded(SCEObject obj)
        {
            foreach (var target in TargetSet)
            {
                if (obj.Position.DistanceFrom(target.Position) <= Range)
                    return true;
            }
            return false;
        }
    }
}
