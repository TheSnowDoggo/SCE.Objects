namespace SCE
{
    public class BWorld : AliasHash<SCEBObject>, ICContainerHolder, IScene
    {
        private readonly HashSet<SCEBObject> _active = new();

        public BWorld(params IComponent[] arr)
        {
            Components = new(this, arr);
        }

        /// <inheritdoc/>
        public CContainer Components { get; }

        /// <inheritdoc/>
        public bool IsActive { get; set; } = true;

        /// <inheritdoc/>
        public void Start()
        {
            foreach (var obj in ActiveObjects())
                obj.Start();
        }

        /// <inheritdoc/>
        public void Update()
        {
            foreach (var obj in ActiveObjects())
                obj.Update();
        }

        /// <inheritdoc/>
        public override bool Add(SCEBObject item)
        {
            var res = base.Add(item);
            if (res)
            {
                item.SetWorld(this);
                if (item.IsActive)
                {
                    _active.Add(item);
                }
            }
            return res;
        }

        /// <inheritdoc/>
        public override bool Remove(SCEBObject item)
        {
            var res = base.Remove(item);
            if (res)
            {
                item.SetWorld(null);
                if (item.IsActive)
                {
                    _active.Remove(item);
                }
            }
            return res;
        }

        public IEnumerable<SCEBObject> ActiveObjects()
        {
            foreach (var obj in _active)
                yield return obj;
        }

        internal void UpdateActive(SCEBObject obj)
        {
            if (obj.IsActive)
            {
                _active.Add(obj);
            }
            else
            {
                _active.Remove(obj);
            }
        }
    }
}
