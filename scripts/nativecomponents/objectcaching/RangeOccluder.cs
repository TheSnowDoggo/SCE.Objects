namespace SCE
{
    public class RangeOccluder : ComponentBase<World>, IUpdate, IObjectCacheable
    {
        private const string DEFAULT_NAME = "range_occluder";

        private readonly List<IObject> _objectList = new();

        private readonly HashSet<IObject> _objectCache = new();

        public RangeOccluder(string name, IEnumerable<IObject> targets, double range)
            : base(name)
        {
            TargetSet = new(targets);
            Range = range;
        }

        public RangeOccluder(IEnumerable<IObject> targets, double range)
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

        public HashSet<IObject> TargetSet { get; set; }

        public double Range { get; set; }

        public IList<IObject> ObjectCache { get => _objectList.AsReadOnly(); }

        public IUpdateLimit? UpdateLimiter { get; set; }

        public HashSet<IObject> ExclusionSet { get; set; } = new();

        public HashSet<IObject> PrioritySet { get; set; } = new();

        public bool ObjectCaching { get; set; } = false;

        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
            {
                UpdateIn(PrioritySet);
                return;
            }

            Clear();
            UpdateIn(Holder);
        }

        private void UpdateIn(IEnumerable<IObject> collection)
        {
            foreach (var obj in collection)
            {
                if (!TargetSet.Contains(obj) && !ExclusionSet.Contains(obj) && !obj.Components.Contains<RangeOccluderExcluder>())
                    obj.IsActive = IsObjectOccluded(obj);
                if (ObjectCaching && obj.IsActive && !obj.Components.IsEmpty && !_objectCache.Contains(obj))
                {
                    _objectCache.Add(obj);
                    _objectList.Add(obj);
                }
            }
        }

        private void Clear()
        {
            if (ObjectCaching)
            {
                _objectList.Clear();
                _objectCache.Clear();
            }
        }

        private bool IsObjectOccluded(IObject obj)
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
