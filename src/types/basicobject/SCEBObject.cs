namespace SCE
{
    public class SCEBObject : SceneBase, ICContainerHolder
    {
        private bool isActive = true;

        public SCEBObject(params IComponent[] arr)
        {
            Components = new(this, arr);
            Children = new(this);
        }

        public CContainer Components { get; }

        public ChildBSet Children { get; }

        public BWorld? World { get; private set; }

        public SCEBObject? Parent { get; private set; }

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                if (HasWorld)
                    World.UpdateActive(this);
            }
        }

        public Vector2 Position { get; set; }

        internal void SetWorld(BWorld? world)
        {
            this.world = world;
        }

        internal void SetParent(SCEBObject? obj)
        {

        }
    }
}
