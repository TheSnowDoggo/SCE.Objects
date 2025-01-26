namespace SCE
{
    /// <summary>
    /// A wrapper class containing a <see cref="IComponent"/> <see cref="List{IComponent}"/>.
    /// </summary>
    public class CGroup : SearchHashTypeExt<IComponent>
    {
        public static HashSet<Type> AssignableTypeSet { get; } = new() { typeof(IUpdate), typeof(IRenderable), typeof(ICollidable) };

        /// <summary>
        /// Initializes a new instance of the <see cref="CGroup"/> class.
        /// </summary>
        public CGroup(IEnumerable<IComponent> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CGroup"/> class that is empty.
        /// </summary>
        public CGroup()
            : base()
        {
        }

        public override bool Add(IComponent t)
        {
            AddAssignableTypes(t, AssignableTypeSet);
            return base.Add(t);
        }

        public override bool Remove(IComponent t)
        {
            RemoveAssignableTypes(t, AssignableTypeSet);
            return base.Remove(t);
        }


        /// <summary>
        /// Calls <see cref="IComponent.Update"/> for every <see cref="IComponent"/> in this instance.
        /// </summary>
        public void Update()
        {
            if (!Contains<IUpdate>())
                return;
            foreach (var component in this)
            {
                if (component.IsActive && component is IUpdate update)
                    update.Update();
            }
        }

        #region AssignableTypes
        private static void ForAssignableTypes(IComponent t, HashSet<Type> typeSet, Action<Type> action)
        {
            foreach (var type in typeSet)
            {
                if (type.IsAssignableFrom(t.GetType()))
                    action.Invoke(type);
            }
        }

        private void AddAssignableTypes(IComponent t, HashSet<Type> typeSet)
        {
            ForAssignableTypes(t, typeSet, AddType);
        }

        private void RemoveAssignableTypes(IComponent t, HashSet<Type> typeSet)
        {
            ForAssignableTypes(t, typeSet, RemoveType);
        }
        #endregion
    }
}
