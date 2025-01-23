namespace SCE
{
    public class Sprite2D : ComponentBase<SCEObject>, IRenderable, ISmartLayerable
    {
        public Sprite2D(string name, DisplayMap dpMap)
            : base(name)
        {
            DisplayMap = dpMap;
        }

        public Sprite2D(DisplayMap dpMap)
            : this("sprite", dpMap)
        {
        }

        public DisplayMap DisplayMap { get; set; }

        public Vector2Int Position { get; set; }

        public int Layer { get; set; }

        public Anchor Anchor { get; set; }

        public DisplayMap GetMap()
        {
            return DisplayMap;
        }
    }
}
