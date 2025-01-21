namespace SCE
{
    public class SmartLayerHandler : ComponentBase<World>
    {
        private readonly List<SmartLayer> _smartLayerList = new();

        public SmartLayerHandler(string name = "smart_layer_handler")
            : base(name)
        {
        }

        public int InitialLayer { get; set; } = 0;

        public StackMode LayeringMode { get; set; } = StackMode.BottomUp;

        public override void Update()
        {
            _smartLayerList.Clear();

            PopulateSmartLayerList();

            if (_smartLayerList.Count == 0)
                return;

            SortSmartLayerList();

            UpdateSmartLayerList();
        }

        private void PopulateSmartLayerList()
        {
            foreach (SCEObject obj in Parent)
            {
                if (obj.IsActive)
                    UpdateObject(obj);
            }
        }

        private void UpdateObject(SCEObject obj)
        {
            foreach (IComponent component in obj.CContainer)
            {
                if (component.IsActive && component is SmartLayer smartLayer)
                    _smartLayerList.Add(smartLayer);
            }
        }

        private void SortSmartLayerList()
        {
            switch (LayeringMode)
            {
                case StackMode.BottomUp:
                    _smartLayerList.Sort((a, b) => b.RelativePosition.Y - a.RelativePosition.Y);
                    break;
                case StackMode.TopDown:
                    _smartLayerList.Sort((a, b) => a.RelativePosition.Y - b.RelativePosition.Y);
                    break;              
            }
        }

        private int GetIncremenet()
        {
            return LayeringMode switch
            {
                StackMode.BottomUp => +1,
                StackMode.TopDown => -1,
                _ => throw new NotImplementedException()
            };
        }

        private void UpdateSmartLayerList()
        {
            int increment = GetIncremenet();
            int layer = InitialLayer;
            int lastY = 0;
            for (int i = 0; i < _smartLayerList.Count; ++i)
            {
                int relY = _smartLayerList[i].RelativePosition.Y;
                if (i != 0 && relY != lastY)
                {
                    layer+=increment;
                    lastY = relY;
                }
                _smartLayerList[i].SmartLayerable.Layer = layer;
            }
        }
    }
}
