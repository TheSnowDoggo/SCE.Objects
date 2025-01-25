namespace SCE
{
    public class Painter : ComponentBase<SCEObject>, IUpdate
    {
        private const string DEFAULT_NAME = "painter";

        public Painter(string name, DisplayMap displayMap)
            : base(name)
        {
            DisplayMap = displayMap;
        }

        public Painter(DisplayMap displayMap)
            : this(DEFAULT_NAME, displayMap)
        {
        }

        public DisplayMap DisplayMap { get; set; }

        public void Update()
        {

        }
    }
}
