﻿namespace SCE
{
    /// <summary>
    /// A class containing a CList and the ICContainerHolder.
    /// Used for storing components by a holder.
    /// </summary>
    public class CContainer : CGroup
    {
        public CContainer(ICContainerHolder holder, CGroup? components = null)
            : base()
        {
            Holder = holder;
            if (components is not null)
                AddRange(components);
        }

        public ICContainerHolder Holder { get; }

        public override bool Add(IComponent component)
        {
            bool result = base.Add(component);
            if (result)
                component.SetCContainer(this, Holder);
            return result;
        }

        public override bool Remove(IComponent component)
        {
            component.SetCContainer(null, Holder);
            return base.Remove(component);
        }

        public override void Clear()
        {
            foreach (var component in this)
                component.SetCContainer(null, Holder);
            base.Clear();
        }
    }
}
