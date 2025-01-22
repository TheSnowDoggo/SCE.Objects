namespace SCE
{
    public class ActiveCache : ComponentBase<World>, IObjectCacheable
    {
        private readonly List<SCEObject> _activeCache = new();

        private int framesPerUpdate = 1;

        private int updateCount = 0;

        public ActiveCache()
        {
        }

        public IList<SCEObject> ObjectCache { get => _activeCache.AsReadOnly(); }

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

        public override void Update()
        {
            if (++updateCount < FramesPerUpdate)
                return;
            updateCount = 0;

            _activeCache.Clear();
            foreach (var obj in Parent)
            {
                if (obj.IsActive)
                    _activeCache.Add(obj);
            }
        }
    }
}
