namespace SCE
{
    public class ActiveCache : ComponentBase<World>, IObjectCacheable
    {
        private readonly List<IObject> _activeCache = new();

        public ActiveCache()
        {
        }

        public IList<IObject> ObjectCache { get => _activeCache.AsReadOnly(); }

        public IUpdateLimit? UpdateLimiter { get; set; }

        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
                return;

            _activeCache.Clear();
            foreach (var obj in Holder)
            {
                if (obj.IsActive)
                    _activeCache.Add(obj);
            }
        }
    }
}
