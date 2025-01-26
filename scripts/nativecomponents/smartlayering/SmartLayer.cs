namespace SCE
{
    public class SmartLayer : ComponentBase<SCEObject>
    {
        public SmartLayer(string name,ISmartLayerable smartLayerable)
            : base(name)
        {
            SmartLayerable = smartLayerable;
        }

        public SmartLayer(ISmartLayerable smartLayerable)
            : this("smart_layer", smartLayerable)
        {
        }

        public ISmartLayerable SmartLayerable { get; set; }

        public Vector2Int Position { get; set; }

        public Vector2Int RelativePosition { get => Holder.WorldGridPosition() + Position; }
    }
}
