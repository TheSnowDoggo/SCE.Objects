namespace SCE
{
    public abstract class HandlerBase<T> : ComponentBase<World>
    {
        public HandlerBase()
            : base()
        {
        }

        protected Func<T, bool>? ComponentRule;

        protected Func<SCEObject, bool>? ObjectRule;

        protected abstract void OnLoad(SCEObject obj, T t);

        protected virtual void LoadComponents(SCEObject obj)
        {
            foreach (var component in obj.Components)
                if (component.IsActive && component is T t && (ComponentRule?.Invoke(t) ?? true))
                    OnLoad(obj, t);
        }

        protected virtual void LoadObjects()
        {
            foreach (var obj in Holder.Objects)
                if (obj.IsActive && obj.Components.Contains<T>() && (ObjectRule?.Invoke(obj) ?? true))
                    LoadComponents(obj);
        }
    }
}
