namespace SCE
{
    /// <summary>
    /// Represents a world containing objects and components.
    /// </summary>
    public class World : SCEObject, IUpdate
    {
        private const string DEFAULT_NAME = "world";

        private readonly List<SCEObject> _activeCache = new();

        private readonly SearchHash<SCEObject> _everyObject = new();

        public World(string name, CGroup? components = null)
            : base(name, components)
        {
            SetWorld(this);
        }

        public World(CGroup? components = null)
            : this(DEFAULT_NAME, components)
        {
        }

        public IUpdateLimit? UpdateLimiter { get; set; }

        public SearchHash<IRenderRule> RenderRules { get; set; } = new();

        public IEnumerable<SCEObject> Objects { get => ObjectCaching ? _activeCache.AsReadOnly() : _everyObject; }

        public bool ObjectCaching { get; set; } = true;

        public override void Start()
        {
            foreach (var obj in _everyObject)
            {
                if (obj.CombinedIsActive)
                    obj.Start();
            }
        }

        public override void Update()
        {
            bool updateCache = UpdateLimiter?.OnUpdate() ?? true;
            if (ObjectCaching && updateCache)
                _activeCache.Clear();
            foreach (var obj in _everyObject)
            {
                if (obj.CombinedIsActive && (!updateCache || ShouldRender(obj)))
                {
                    obj.UpdateAll();
                    if (ObjectCaching && updateCache)
                        _activeCache.Add(obj);
                }
            }
            Components.Update();
        }

        private bool ShouldRender(SCEObject obj)
        {
            foreach (var rule in RenderRules)
            {
                if (rule.IsActive && !rule.ShouldRender(obj))
                    return false;
            }
            return true;
        }

        #region Recursive
        internal void RecursiveAdd(SCEObject obj)
        {
            _everyObject.Add(obj);
            _everyObject.AddRange(obj.RecursiveGetChildren());
        }

        internal void RecursiveRemove(SCEObject obj)
        {
            _everyObject.Remove(obj);
            _everyObject.RemoveRange(obj.RecursiveGetChildren());
        }
        #endregion
    }
}
