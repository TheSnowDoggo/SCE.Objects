namespace SCE
{
    public class RangeOccluder : ComponentBase<World>, IUpdate, IObjectCacheable
    {
        private const string DEFAULT_NAME = "range_occluder";

        private readonly List<SCEObject> _objectList = new();

        private readonly HashSet<SCEObject> _objectCache = new();

        #region Constructors
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
        #endregion

        public HashSet<SCEObject> TargetSet { get; set; }

        public double Range { get; set; }

        public IList<SCEObject> ObjectCache { get => _objectList.AsReadOnly(); }

        public IUpdateLimit? UpdateLimiter { get; set; }

        public HashSet<SCEObject> ExclusionSet { get; set; } = new();

        public HashSet<SCEObject> PrioritySet { get; set; } = new();

        public bool ObjectCaching { get; set; } = false;

        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
            {
                UpdateIn(PrioritySet);
                return;
            }

            ClearCache();
            UpdateIn(Holder.EveryObject);
        }

        private void UpdateIn(IEnumerable<SCEObject> collection)
        {
            foreach (var obj in collection)
            {
                if (!TargetSet.Contains(obj) && !ExclusionSet.Contains(obj) && !obj.Components.Contains<RangeOccluderExcluder>())
                {
                    if (!IsObjectOccluded(obj))
                        obj.Components.Remove("occluded_tag");
                    else if(!obj.Components.Contains<OccludedTag>())
                        obj.Components.Add(new OccludedTag());
                }
                if (ObjectCaching && !obj.Components.IsEmpty && !_objectCache.Contains(obj) && !obj.Components.Contains<OccludedTag>())
                {
                    _objectCache.Add(obj);
                    _objectList.Add(obj);
                }
            }
        }

        private void ClearCache()
        {
            if (ObjectCaching)
            {
                _objectList.Clear();
                _objectCache.Clear();
            }
        }

        private bool IsObjectOccluded(SCEObject obj)
        {
            foreach (var target in TargetSet)
            {
                if (obj.WorldPosition.DistanceFrom(target.WorldPosition) <= Range)
                    return false;
            }
            return true;
        }
    }
}
