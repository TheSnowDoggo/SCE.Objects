namespace SCE
{
    public class World : SCEObject, IScene
    {
        private readonly List<SCEObject> _activeCache = new();

        private readonly AliasHash<SCEObject> _everyObject = new();

        public World(params IComponent[] arr)
            : base(arr)
        {
            SetWorld(this);
        }

        /// <summary>
        /// Gets every descendent of the world.
        /// </summary>
        public IEnumerable<SCEObject> Objects { get => ObjectCaching ? _activeCache.AsReadOnly() : _everyObject; }

        #region Settings

        public IUpdateLimit? UpdateLimiter { get; set; }

        public IUpdateLimit? ComponentLimiter { get; set; }

        public AliasHash<IRenderRule> RenderRules { get; set; } = new();

        public bool ObjectCaching { get; set; } = true;

        #endregion

        #region Scene

        /// <inheritdoc/>
        public override void Start()
        {
            foreach (var obj in _everyObject)
                if (obj.WorldIsActive)
                    obj.Start();
        }

        /// <inheritdoc/>
        public override void Update()
        {
            bool updateCache = UpdateLimiter?.OnUpdate() ?? true;
            if (ObjectCaching && updateCache)
            {
                _activeCache.Clear();
            }
            foreach (var obj in _everyObject)
            {
                if (obj.WorldIsActive && (!updateCache || ShouldRender(obj)))
                {
                    obj.UpdateAll();
                    if (ObjectCaching && updateCache)
                    {
                        _activeCache.Add(obj);
                    }
                }
            }
            if (!ComponentLimiter?.OnUpdate() ?? true)
            {
                Components.Update();
            }
        }

        private bool ShouldRender(SCEObject obj)
        {
            foreach (var rule in RenderRules)
                if (rule.IsActive && !rule.ShouldRender(obj))
                    return false;
            return true;
        }

        #endregion

        #region Recursive

        internal void RecursiveAdd(SCEObject obj)
        {
            _everyObject.Add(obj);
            _everyObject.AddRange(obj.RecursiveGetDescendents());
        }

        internal void RecursiveRemove(SCEObject obj)
        {
            _everyObject.Remove(obj);
            _everyObject.RemoveRange(obj.RecursiveGetDescendents());
        }

        #endregion
    }
}
