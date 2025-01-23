namespace SCE
{
    /// <summary>
    /// A wrapper class containing a <see cref="IComponent"/> <see cref="List{IComponent}"/>.
    /// </summary>
    public class CList : SearchHashTypeExt<IComponent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CList"/> class.
        /// </summary>
        public CList(IEnumerable<IComponent> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CList"/> class that is empty.
        /// </summary>
        public CList()
            : base()
        {
        }

        /// <summary>
        /// Calls <see cref="IComponent.Update"/> for every <see cref="IComponent"/> in this instance.
        /// </summary>
        public void Update()
        {
            foreach (IComponent c in this)
            {
                if (c.IsActive)
                    c.Update();
            }
        }
    }
}
