namespace SCE
{
    public class World : AliasHash<SCEObject>, ICContainerHolder, IScene
    {
        private readonly HashSet<SCEObject> _active = new();

        public World(params IComponent[] arr)
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
            UpdateActive();
            foreach (var obj in _active)
            {
                if (obj is IScene scene)
                {
                    scene.Start();
                }
            }
        }

        /// <inheritdoc/>
        public void Update()
        {
            UpdateActive();
            foreach (var obj in _active)
            {
                if (obj is IUpdate update)
                {
                    update.Update();
                }
                obj.Components.Update();
            }
            Components.Update();
        }

        #region Modify

        /// <inheritdoc/>
        public override bool Add(SCEObject item)
        {
            var res = base.Add(item);
            if (res)
                item.UpdateWorld(this);
            return res;
        }

        /// <inheritdoc/>
        public override bool Remove(SCEObject item)
        {
            var res = base.Remove(item);
            if (res)
                item.UpdateWorld(null);
            return res;
        }

        #endregion

        public IEnumerable<SCEObject> EnumerateActive()
        {
            return _active;
        }

        public void UpdateActive()
        {
            _active.Clear();
            foreach (var root in this)
            {
                foreach (var obj in root.EnumerateAllActive())
                {
                    _active.Add(obj);
                }
            }
        }
    }
}
