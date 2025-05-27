namespace SCE
{
    public class SmartLayerHandler : ComponentBase<World>, IUpdate
    {
        public int InitialLayer { get; set; } = 0;

        public StackType LayeringMode { get; set; } = StackType.BottomUp;

        public IUpdateLimit? RenderLimiter { get; set; }

        public void Update()
        {
            if (!RenderLimiter?.OnUpdate() ?? false)
            {
                return;
            }

            List<SmartLayer> smartLayers = new();
            foreach (var sl in EnumerateSmartLayers())
            {
                smartLayers.Add(sl);
            }

            smartLayers.Sort(LayeringMode == StackType.TopDown ? (a, b) => a.RelativePosition.Y - b.RelativePosition.Y :
                (a, b) => a.RelativePosition.Y - b.RelativePosition.Y);

            int increment = LayeringMode == StackType.TopDown ? -1 : +1;

            int layer = InitialLayer;

            int lastY = 0;

            for (int i = 0; i < smartLayers.Count; ++i)
            {
                int relY = smartLayers[i].RelativePosition.Y;
                if (i != 0 && relY != lastY)
                {
                    layer += increment;
                    lastY = relY;
                }
                smartLayers[i].SmartLayerable.Layer = layer;
            }
        }

        private IEnumerable<SmartLayer> EnumerateSmartLayers()
        {
            foreach (var obj in Holder.EnumerateActive())
            {
                foreach (var sl in obj.Components.EnumerateType<SmartLayer>())
                {
                    if (sl.IsActive)
                    {
                        yield return sl;
                    }
                }
            }
        } 
    }
}
