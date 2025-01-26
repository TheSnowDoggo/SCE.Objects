namespace SCE
{
    /// <summary>
    /// Represents a world containing objects and components.
    /// </summary>
    public class World : SearchHash<SCEObject>, ICContainerHolder, IUpdate
    {
        private const string DEFAULT_NAME = "world";

        public World(string name, CGroup? components = null)
            : base()
        {
            Name = name;
            Components = new(this, components);
        }

        public World(CGroup? components = null)
            : this(DEFAULT_NAME, components)
        {
        }

        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public CContainer Components { get; }

        public SearchHash<SCEObject> EveryObject { get; } = new();

        public void Start()
        {
            foreach (var obj in this)
            {
                if (obj.IsActive)
                    obj.WorldStart();
            }
        }

        public void Update()
        {
            EveryObject.Clear();
            foreach (var obj in this)
            {
                if (obj.IsActive)
                    obj.WorldUpdate(EveryObject);
            }
            Components.Update();
        }

        public override bool Add(SCEObject obj)
        {
            obj.SetWorld(this);
            return base.Add(obj);
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
