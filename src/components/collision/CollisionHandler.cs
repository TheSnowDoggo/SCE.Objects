namespace SCE
{
    public class CollisionHandler : ComponentBase<World>, IUpdate
    {
        public IUpdateLimit? RenderLimiter { get; set; }

        /// <inheritdoc/>
        public void Update()
        {
            if (!RenderLimiter?.OnUpdate() ?? false)
            {
                return;
            }

            KeyHash<int, Collider> masks = new();
            KeyHash<int, Collider> layers = new();

            foreach (var c in EnumerateColliders())
            {
                foreach (var bit in LData.EnumerateMasks(c.LayerData))
                {
                    masks[bit].Add(c);
                }

                foreach (var bit in LData.EnumerateLayers(c.LayerData))
                {
                    layers[bit].Add(c);
                }
            }

            foreach ((var mask, var maskSet) in masks)
            {
                if (!layers.TryGetValue(mask, out var layerSet))
                {
                    continue;
                }

                foreach (var c1 in maskSet)
                {
                    foreach (var c2 in layerSet)
                    {
                        if (c1 == c2)
                        {
                            continue;
                        }

                        if (c1.CollidesWith(c2) || c2.CollidesWith(c1))
                        {
                            c2.OnCollision?.Invoke(new CollisionDetails(this, c2, c1));
                        }
                    }
                }
            }
        }

        private IEnumerable<Collider> EnumerateColliders()
        {
            foreach (var obj in Holder.EnumerateActive())
            {
                foreach (var c in obj.Components.EnumerateType<Collider>())
                {
                    if (c.IsActive)
                    {
                        yield return c;
                    }
                }
            }
        }
    }
}
