namespace SCE
{
    public class ComponentBase<T> : IComponent
        where T : ICContainerHolder
    {
        private CContainer? cContainer;

        public ComponentBase(string name)
        {
            Name = name;
        }

        public ComponentBase()
            : this(string.Empty)
        {
        }

        public CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        public T Parent { get => (T)CContainer.CContainerHolder; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is not T)
                throw new InvalidCContainerHolderException("Holder is invalid.");
            this.cContainer = cContainer;
        }

        public virtual void Update()
        {
        }
    }
}
