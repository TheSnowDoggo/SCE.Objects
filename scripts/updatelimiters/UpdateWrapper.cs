namespace SCE
{
    public class UpdateWrapper : IScene
    {
        private const string DEFAULT_NAME = "update_wrapper";

        public UpdateWrapper(string name, IScene scene)
        {
            Name = name;
            Scene = scene;
        }

        public UpdateWrapper(IScene scene)
            : this(DEFAULT_NAME, scene)
        {
        }

        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public IScene Scene { get; set; } 

        public IUpdateLimit? UpdateLimiter { get; set; }

        public void Start()
        {
            Scene.Start();
        }

        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
                return;
            Scene.Update();
        }
    }
}
