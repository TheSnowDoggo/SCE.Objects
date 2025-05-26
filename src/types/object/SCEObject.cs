using System.Diagnostics.CodeAnalysis;
namespace SCE
{
    public class SCEObject : ICContainerHolder, IScene
    {
        public SCEObject(params IComponent[] arr)
        {
            Components = new(this, arr);
            Children = new(this);
        }

        /// <summary>
        /// Gets the direct children of the object.
        /// </summary>
        public ChildSet Children { get; }

        /// <summary>
        /// Gets the components of the object.
        /// </summary>
        public CContainer Components { get; }

        internal void UpdateRecursiveProperties()
        {
            UpdateWorldIsActive();
            UpdateWorldPosition();
        }

        #region Active

        private bool isActive = true;

        /// <summary>
        /// Gets or sets a value indicating whether this object is active.
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                UpdateWorldIsActive();
            }
        }

        /// <summary>
        /// Gets a value indicating the combined active state of this object.
        /// </summary>
        public bool WorldIsActive { get; private set; }

        private void UpdateWorldIsActive()
        {
            WorldIsActive = RecursiveResolveActive() && IsActive;
            RecursiveUpdateCombinedIsActive();
        }

        #endregion

        #region Position

        private Vector2 position;

        /// <summary>
        /// Gets or sets the local position of this object.
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                UpdateWorldPosition();
            }
        }

        /// <summary>
        /// Gets the world position of this object.
        /// </summary>
        public Vector2 WorldPosition { get; private set; }

        /// <summary>
        /// Returns the rounded world position of this object.
        /// </summary>
        /// <returns>The rounded world position of this object.</returns>
        public Vector2Int WorldGridPosition()
        {
            return (Vector2Int)WorldPosition.Round();
        }

        private void UpdateWorldPosition()
        {
            WorldPosition = (Parent?.WorldPosition ?? Vector2.Zero) + position;
            RecursiveUpdateWorldPositions();
        }

        #endregion

        #region Children

        /// <summary>
        /// Recursively resolves every descendent of this object. 
        /// </summary>
        /// <returns>A list containing every descendent of this object. </returns>
        public List<SCEObject> RecursiveGetDescendents()
        {
            List<SCEObject> list = new();
            RecursiveResolveDescendents(list);
            return list;
        }

        private void RecursiveResolveDescendents(List<SCEObject> list)
        {
            if (Children.Count == 0)
                return;
            list.AddRange(Children);
            foreach (var child in Children)
            {
                child.RecursiveResolveDescendents(list);
            }
        }

        internal void RecursiveSetWorld(World? world)
        {
            SetWorld(world);
            foreach (var child in Children)
            {
                child.RecursiveSetWorld(world);
            }
        }

        private bool RecursiveResolveActive()
        {
            if (Parent is null)
                return true;
            if (!Parent.IsActive)
                return false;
            return Parent.RecursiveResolveActive();
        }

        private void RecursiveUpdateCombinedIsActive()
        {
            foreach (var child in Children)
            {
                child.WorldIsActive = WorldIsActive && child.IsActive;
                child.RecursiveUpdateCombinedIsActive();
            }
        }

        private void RecursiveUpdateWorldPositions()
        {
            foreach (var child in Children)
            {
                child.WorldPosition = WorldPosition + child.Position;
                child.RecursiveUpdateWorldPositions();
            }
        }

        #endregion

        #region Parent

        /// <summary>
        /// Gets the parent of the object.
        /// </summary>
        public SCEObject? Parent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object has a parent.
        /// </summary>
        public bool HasParent { get => Parent != null; }

        public bool TryGetParent([NotNullWhen(true)] out SCEObject? parent)
        {
            parent = Parent;
            return parent != null;
        }

        internal void SetParent(SCEObject? parent)
        {
            Parent = parent;
        }

        #endregion

        #region World

        /// <summary>
        /// Gets the world of the object.
        /// </summary>
        public World? World { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object has a world.
        /// </summary>
        public bool HasWorld { get => World != null; }

        public bool TryGetWorld([NotNullWhen(true)] out World? world)
        {
            world = World;
            return world != null;
        }

        internal void SetWorld(World? world)
        {
            World = world;
        }

        #endregion

        #region Scene

        /// <inheritdoc/>
        public virtual void Start()
        {
        }

        /// <inheritdoc/>
        public virtual void Update()
        {
        }

        internal void UpdateAll()
        {
            Components.Update();
            Update();
        }

        #endregion

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"SCEObject(Pos:({Position}), Active?:{IsActive})";
        }
    }
}
