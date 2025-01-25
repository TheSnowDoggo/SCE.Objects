﻿namespace SCE
{
    public class SmartLayerHandler : ComponentBase<World>, IUpdate
    {
        private const string DEFAULT_NAME = "smart_layer_handler";

        private readonly List<SmartLayer> _smartLayerList = new();

        public SmartLayerHandler(string name = DEFAULT_NAME)
            : base(name)
        {
        }

        public int InitialLayer { get; set; } = 0;

        public StackType LayeringMode { get; set; } = StackType.BottomUp;

        public IObjectCacheable? IObjectCacheable { get; set; }

        public IUpdateLimit? UpdateLimiter { get; set; }

        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
                return;

            _smartLayerList.Clear();

            PopulateSmartLayerList();

            if (_smartLayerList.Count == 0)
                return;

            SortSmartLayerList();

            UpdateSmartLayerList();
        }

        private void PopulateSmartLayerList()
        {
            IEnumerable<SCEObject> collection = IObjectCacheable is null ? Holder : IObjectCacheable.ObjectCache;
            foreach (SCEObject obj in collection)
            {
                if (obj.IsActive && obj.Components.Contains<SmartLayer>())
                    UpdateObject(obj);
            }
        }

        private void UpdateObject(SCEObject obj)
        {
            foreach (IComponent component in obj.Components)
            {
                if (component.IsActive && component is SmartLayer smartLayer)
                    _smartLayerList.Add(smartLayer);
            }
        }

        private void SortSmartLayerList()
        {
            switch (LayeringMode)
            {
                case StackType.BottomUp:
                    _smartLayerList.Sort((a, b) => b.RelativePosition.Y - a.RelativePosition.Y);
                    break;
                case StackType.TopDown:
                    _smartLayerList.Sort((a, b) => a.RelativePosition.Y - b.RelativePosition.Y);
                    break;              
            }
        }

        private int GetIncremenet()
        {
            return LayeringMode switch
            {
                StackType.BottomUp => +1,
                StackType.TopDown => -1,
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
