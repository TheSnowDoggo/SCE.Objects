namespace SCE
{
    /// <summary>
    /// Represents a world containing objects and components.
    /// </summary>
    public class World : SearchHash<IObject>, ICContainerHolder, IUpdate
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

        public void Start()
        {
            StartObjects();
        }

        public void Update()
        {
            if (PrioritiseWorldComponentUpdates)
            {
                Components.Update();
                UpdateObjects();
            }
            else
            {
                UpdateObjects();
                Components.Update();
            }
        }

        public void StartObjects()
        {
            foreach (var obj in GetObjects())
            {
                if (obj.IsActive)
                    obj.Start();
            }
        }

        public void UpdateObjects()
        {
            foreach (var obj in GetObjects())
            {
                if (obj.IsActive)
                {
                    obj.Components.Update();
                    obj.Update();
                }
            }
        }

        public override void Add(IObject obj)
        {
            obj.SetWorld(this);
            base.Add(obj);            
        }

        public override bool Remove(IObject obj)
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

        private IEnumerable<IObject> GetObjects()
        {
            return IObjectCacheable is null ? this : IObjectCacheable.ObjectCache; 
        }
    }
}
