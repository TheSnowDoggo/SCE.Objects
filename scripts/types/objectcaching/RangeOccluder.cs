namespace SCE
{
    public class RangeOccluder : IRenderRule
    {
        private const string DEFAULT_NAME = "range_occluder";

        #region Constructors
        public RangeOccluder(string name, double range, IEnumerable<SCEObject>? targets = null)
        {
            Name = name;
            TargetSet = targets is not null ? new(targets) : new();
            Range = range;
        }

        public RangeOccluder(double range, IEnumerable<SCEObject>? targets = null)
            : this(DEFAULT_NAME, range, targets)
        {
        }
        #endregion

        public string Name { get; set; }

        #region Settings

        public bool IsActive { get; set; } = true;

        public double Range { get; set; }

        public HashSet<SCEObject> TargetSet { get; set; }

        public HashSet<SCEObject> ExclusionSet { get; set; } = new();

        public HashSet<SCEObject> PrioritySet { get; set; } = new();

        #endregion

        public bool ShouldRender(SCEObject obj)
        {
            if (TargetSet.Contains(obj) || ExclusionSet.Contains(obj) || obj.Components.Contains<RangeOccluderExcluder>())
                return true;
            foreach (var target in TargetSet)
            {
                if (obj.WorldPosition.DistanceFrom(target.WorldPosition) <= Range)
                    return true;
            }
            return false;
        }
    }
}
