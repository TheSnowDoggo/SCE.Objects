namespace SCE
{
    public abstract class HandlerBase<T> : ComponentBase<World>
    {
        protected Func<T, bool>? ComponentRule;

        protected Func<SCEObject, bool>? ObjectRule;

        protected abstract void OnLoad(SCEObject obj, T t);

        protected virtual void LoadComponents(SCEObject obj)
        {
            foreach (var c in obj.Components)
            {
                if (c.IsActive && c is T t && (ComponentRule?.Invoke(t) ?? true))
                {
                    OnLoad(obj, t);
                }
            }
        }

        protected virtual void LoadObjects()
        {
            foreach (var obj in Holder.EnumerateActive())
            {
                if (obj.Components.Contains<T>() && (ObjectRule?.Invoke(obj) ?? true))
                {
                    LoadComponents(obj);
                }
            }
        }
    }
}
