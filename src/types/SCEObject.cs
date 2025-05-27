namespace SCE
{
    public class SCEObject : ICContainerHolder
    {
        private World? world;

        private SCEObject? parent;

        private bool isActive = true;

        private bool globalIsActive = true;

        private Vector2 position;

        private Vector2 globalPosition;

        public SCEObject(params IComponent[] arr)
        {
            Components = new(this, arr);
            Children = new(this);
        }

        public CContainer Components { get; }

        public ChildSet Children { get; }

        public World? World { get => world; }

        public SCEObject? Parent { get => parent; }

        public bool DisableUpdates { get; set; } = false;

        #region Properties

        /// <summary>
        /// Gets or sets the local active state.
        /// </summary>
        /// <remarks>
        /// Note: Not recommended to set frequently as it will update all descents.
        /// </remarks>
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                UpdateActive();
            }
        }

        /// <summary>
        /// Gets the active state relative to its parents.
        /// </summary>
        public bool GlobalIsActive { get => globalIsActive; }

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        /// <remarks>
        /// Note: Not recommended to set frequently as it will update all descents.
        /// </remarks>
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Gets the position relative to its parents.
        /// </summary>
        public Vector2 GlobalPosition { get => globalPosition; }

        public Vector2Int GridPosition() { return (Vector2Int)Position.Round(); }

        public Vector2Int GlobalGridPosition() { return (Vector2Int)GlobalPosition.Round(); }

        #endregion

        /// <summary>
        /// Enumerates all descendents.
        /// </summary>
        public IEnumerable<SCEObject> EnumerateDescendents()
        {
            Stack<SCEObject> stack = new();
            foreach (var child in Children)
            {
                stack.Push(child);
            }

            while (stack.Count > 0)
            {
                var obj = stack.Pop();
                yield return obj;
                foreach (var child in obj.Children)
                {
                    stack.Push(child);
                }
            }
        }

        /// <summary>
        /// Enumerates all active descendents including this.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SCEObject> EnumerateAllActive()
        {
            Stack<SCEObject> stack = new();

            if (GlobalIsActive)
            {
                stack.Push(this);
            }

            while (stack.Count > 0)
            {
                var obj = stack.Pop();
                yield return obj;
                foreach (var child in obj.Children)
                {
                    if (child.GlobalIsActive)
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        internal void UpdateWorld(World? world)
        {
            this.world = world;
            foreach (var obj in EnumerateDescendents())
                obj.world = world;
        }

        internal void InitializeChild(SCEObject? parent)
        {
            this.parent = parent;
            world = parent?.world;
            UpdateActive();
            UpdatePosition();
        }

        private void UpdateActive()
        {
            Stack<SCEObject> stack = new();
            stack.Push(this);

            while (stack.Count > 0)
            {
                var obj = stack.Pop();

                var last = obj.globalIsActive;
                if (obj.Parent == null)
                {
                    if (obj != this)
                    {
                        throw new NullReferenceException("Parent is null.");
                    }

                    globalIsActive = Parent != null ? isActive && Parent.globalIsActive : isActive;
                }
                else
                {
                    obj.globalIsActive = obj.isActive && obj.Parent.isActive;
                }

                if (obj.globalIsActive == last)
                {
                    continue;
                }

                foreach (var child in obj.Children)
                {
                    stack.Push(child);
                }
            }
        }

        private void UpdatePosition()
        {
            Stack<SCEObject> stack = new();
            stack.Push(this);

            while (stack.Count > 0)
            {
                var obj = stack.Pop();

                var last = obj.GlobalPosition;
                if (obj.Parent == null)
                {
                    if (obj != this)
                    {
                        throw new NullReferenceException("Parent is null.");
                    }

                    globalPosition = Parent != null ? Position + Parent.GlobalPosition : Position;
                }
                else
                {
                    obj.globalPosition = obj.position + obj.Parent.globalPosition;
                }

                if (obj.globalPosition == last)
                {
                    continue;
                }

                foreach (var child in obj.Children)
                {
                    stack.Push(child);
                }
            }
        }
    }
}
