namespace SCECorePlus.Components.RHC
{
    using SCEComponents;

    using SCECorePlus.Objects;

    // WorldSpace Active Renderable Utility Component
    internal class WorldSpaceARUC : IComponent
    {
        private const bool DefaultActiveState = true;

        private readonly List<SCEObject> renderableObjectList = new();

        private CContainer? cContainer;

        public WorldSpaceARUC(string name, bool isActive = DefaultActiveState)
        {
            Name = name;
            IsActive = isActive;
        }

        public string Name { get; set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        public event EventHandler? ComponentModifyEvent;

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private World World { get => (World)CContainer.CContainerHolder; }

        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is World)
            {
                this.cContainer = cContainer;
                World.WorldModifyEvent += WorldSpaceARUC_ObjectModifyEvent;
            }
            else
            {
                throw new InvalidCContainerHolderException("CContainerHolder is not World.");
            }
        }

        private void WorldSpaceARUC_ObjectModifyEvent(object? sender, WorldModifyEventArgs e)
        {
            SCEObject obj = e.Object;

            if (obj.IsActive)
            {
                if (e.Type == WorldModifyEventArgs.ModifyType.Add)
                {
                }
            }
        }
    }
}
