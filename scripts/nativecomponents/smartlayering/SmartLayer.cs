namespace SCE
{
    public class SmartLayer : ComponentBase<SCEObject>
    {
        public SmartLayer(ISmartLayerable smartLayerable)
            : base()
        {
            SmartLayerable = smartLayerable;
        }

        public ISmartLayerable SmartLayerable { get; set; }

        public Vector2Int Position { get; set; }

        public Vector2Int RelativePosition { get => Parent.GridPosition + Position; }
    }
}
