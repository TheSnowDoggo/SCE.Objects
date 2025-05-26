namespace SCE
{
    /// <summary>
    /// A class containing components and the ICContainerHolder.
    /// Used for storing components by a holder.
    /// </summary>
    public class CContainer : AliasHashTExt<IComponent>
    {
        /// <summary>
        /// A set containing cached components types.
        /// </summary>
        public static HashSet<Type> AssignableTypeSet { get; } = new()
        {
            typeof(IUpdate),
            typeof(IRenderable),
        };

        public CContainer(ICContainerHolder holder)
            : base()
        {
            Holder = holder;
        }

        public CContainer(ICContainerHolder holder, params IComponent[] arr)
            : this(holder)
        {
            AddRange(arr);
        }

        public ICContainerHolder Holder { get; }

        /// <summary>
        /// Calls update for every <see cref="IComponent"/> in this instance.
        /// </summary>
        public void Update()
        {
            if (!Contains<IUpdate>())
                return;
            foreach (var c in this)
                if (c.IsActive && c is IUpdate update)
                    update.Update();
        }

        #region Modify

        public override bool Add(IComponent component)
        {
            var res = base.Add(component);
            if (res)
            {
                component.SetCContainer(this, Holder);
                AddCacheTypes(component);
            }
            return res;
        }

        public override bool Remove(IComponent component)
        {
            var res = base.Remove(component);
            if (res)
            {
                component.SetCContainer(null, Holder);
                RemoveCacheTypes(component);
            }
            return res;
        }

        public override void Clear()
        {
            foreach (var c in this)
                c.SetCContainer(null, Holder);
            base.Clear();
        }

        #endregion

        #region CacheTypes

        private void AddCacheTypes(IComponent component)
        {
            foreach (var type in AssignableTypeSet)
                AddType(component, type);
        }

        private void RemoveCacheTypes(IComponent component)
        {
            foreach (var type in AssignableTypeSet)
                RemoveType(component, type);
        }

        #endregion
    }
}
