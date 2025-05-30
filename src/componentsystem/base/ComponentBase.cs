﻿namespace SCE
{
    public class ComponentBase<T> : IComponent
        where T : ICContainerHolder
    {
        private CContainer? container;

        public CContainer Container { get => container ?? throw new NullReferenceException("CContainer is null."); }

        public T Holder { get => (T)Container.Holder; }

        public bool HasContainer { get => container != null; }

        public bool IsActive { get; set; } = true;

        public void SetCContainer(CContainer? container, ICContainerHolder holder)
        {
            if (holder is not T)
            {
                throw new InvalidCContainerHolderException();
            }
            this.container = container;
        }
    }
}
