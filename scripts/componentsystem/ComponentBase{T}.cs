namespace SCE
{
    public class ComponentBase<T> : IComponent
        where T : ICContainerHolder
    {
        private CContainer? container;

        public ComponentBase(string name)
        {
            Name = name;
        }

        public ComponentBase()
            : this(string.Empty)
        {
        }

        public CContainer Container { get => container ?? throw new NullReferenceException("CContainer is null."); }

        public T Holder { get => (T)Container.Holder; }

        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public void SetCContainer(CContainer? container, ICContainerHolder holder)
        {
            if (holder is not T)
                throw new InvalidCContainerHolderException();
            this.container = container;
        }
    }
}
