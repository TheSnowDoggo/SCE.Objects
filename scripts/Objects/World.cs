﻿namespace SCE
{
    /// <summary>
    /// Represents a world containing objects and components.
    /// </summary>
    public class World : SearchHash<SCEObject>, ICContainerHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <param name="cList">The initial cList of the world.</param>
        public World(CList cList)
        {
            CContainer = new(this, cList);
            OnAdd += World_OnAdd;
            OnRemove += World_OnRemove;
            SetupEveryObject();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        public World()
            : this(new CList())
        {
        }

        /// <inheritdoc/>
        public CContainer CContainer { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the world should update its own components before updating the contained objects components.
        /// </summary>
        public bool PrioritiseWorldComponentUpdates { get; set; } = false;

        /// <summary>
        /// Updates all the components in the world and all the components in every object.
        /// </summary>
        public void Update()
        {
            if (PrioritiseWorldComponentUpdates)
            {
                CContainer.Update();
                UpdateObjectComponents();
            }
            else
            {
                UpdateObjectComponents();
                CContainer.Update();
            }
        }

        /// <summary>
        /// Updates all the components in every object.
        /// </summary>
        public void UpdateObjectComponents()
        {
            foreach (var obj in this)
            {
                if (obj.IsActive)
                    obj.CContainer.Update();
            }
        }

        private void World_OnAdd(SCEObject obj)
        {
            obj.SetWorld(this);
        }

        private void World_OnRemove(SCEObject obj)
        {
            obj.SetWorld(null);
        }

        private void SetupEveryObject()
        {
            foreach (var obj in this)
                obj.SetWorld(this);
        }
    }
}
