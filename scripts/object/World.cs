namespace SCE
{
    /// <summary>
    /// Represents a world containing objects and components.
    /// </summary>
    public class World : SearchHash<SCEObject>, ICContainerHolder, IUpdate
    {
        private const string DEFAULT_NAME = "world";

        public World(string name, CGroup? components = null)
        {
            Name = name;
            Components = new(this, components);
        }

        public World(CGroup components)
            : this(DEFAULT_NAME, components)
        {
        }

        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public CContainer Components { get; }

        public bool PrioritiseWorldComponentUpdates { get; set; } = false;

        #region Caching
        public IObjectCacheable? IObjectCacheable { get; set; }
        #endregion

        public void Update()
        {
            if (PrioritiseWorldComponentUpdates)
            {
                Components.Update();
                UpdateObjectComponents();
            }
            else
            {
                UpdateObjectComponents();
                Components.Update();
            }
        }

        public void UpdateObjectComponents()
        {
            IEnumerable<SCEObject> collection = IObjectCacheable is null ? this : IObjectCacheable.ObjectCache;
            foreach (var obj in collection)
            {
                if (obj.IsActive)
                    obj.Components.Update();
            }
        }

        public override void Add(SCEObject obj)
        {
            obj.SetWorld(this);
            base.Add(obj);            
        }

        public override bool Remove(SCEObject obj)
        {
            obj.SetWorld(null);
            return base.Remove(obj);
        }

        public override void Clear()
        {
            foreach (var obj in this)
                obj.SetWorld(null);
            base.Clear();
        }
    }
}
