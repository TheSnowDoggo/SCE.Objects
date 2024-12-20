namespace SCE
{
    /// <summary>
    /// A class containing a CList and the ICContainerHolder.
    /// Used for storing components by a holder.
    /// </summary>
    public class CContainer : CList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CContainer"/> class.
        /// Takes the CContainerHolder and a default CList.
        /// </summary>
        /// <param name="holder">The reference to the CContainerHolder storing this instance.</param>
        /// <param name="cList">The default CList for this instance.</param>
        public CContainer(ICContainerHolder holder, CList cList)
            : base(cList)
        {
            CContainerHolder = holder;
            SetEveryCContainer(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CContainer"/> class with an empty CList.
        /// Takes the CContainerHolder.
        /// </summary>
        /// <param name="holder">The reference to the CContainerHolder storing this instance.</param>
        public CContainer(ICContainerHolder holder)
            : this(holder, new CList())
        {
        }

        /// <inheritdoc cref="SCEComponents.ICContainerHolder"/>
        public ICContainerHolder CContainerHolder { get; }

        /// <summary>
        /// Sets the <see cref="CContainerHolder"/> of every <see cref="IComponent"/> in <see cref="CList"/>.
        /// </summary>
        private void SetEveryCContainer(CContainer? cContainer)
        {
            foreach (IComponent component in this)
                component.SetCContainer(cContainer, CContainerHolder);
        }
    }
}
