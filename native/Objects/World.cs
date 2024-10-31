namespace SCECorePlus.Objects
{
    using System;
    using System.Collections;

    using SCECore.ComponentSystem;

    /// <summary>
    /// Represents a world containing objects and components.
    /// </summary>
    public class World : IEnumerable<SCEObject>, ICContainerHolder
    {
        private readonly List<SCEObject> objectList;

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <param name="objectList">The initial object list of the world.</param>
        /// <param name="cList">The initial cList of the world.</param>
        public World(List<SCEObject> objectList, CList cList)
        {
            this.objectList = objectList;

            CContainer = new(this, cList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <param name="objectList">The initial object list of the world.</param>
        public World(List<SCEObject> objectList)
            : this(objectList, new CList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <param name="cList">The initial cList of the world.</param>
        public World(CList cList)
            : this(new List<SCEObject>(), cList)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        public World()
            : this(new CList())
        {
        }

        /// <summary>
        /// Gets the object list.
        /// </summary>
        public List<SCEObject> ObjectList { get => objectList; }

        /// <inheritdoc/>
        public CContainer CContainer { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the world should update its own components before updating the contained objects components.
        /// </summary>
        public bool PrioritiseWorldComponentUpdates { get; set; } = false;

        /// <summary>
        /// Gets the number of objects in the world.
        /// </summary>
        public int Objects { get => objectList.Count; }

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the object to get or set.</param>
        /// <returns>The object at the specified index.</returns>
        public SCEObject this[int index]
        {
            get => objectList[index];
            set => objectList[index] = value;
        }

        /// <summary>
        /// Gets or sets the first occurance of an object with a name matching the specified name.
        /// </summary>
        /// <param name="name">The name of the object to get or set.</param>
        /// <returns>The first occurance of an object with a name matching the specified name.</returns>
        public SCEObject this[string name]
        {
            get => this[IndexOf(name)];
            set => this[IndexOf(name)] = value;
        }

        /// <inheritdoc/>
        public IEnumerator<SCEObject> GetEnumerator() => objectList.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Updates all the components in the world and all the components in every object.
        /// </summary>
        public void UpdateAll()
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
            foreach (SCEObject obj in this)
            {
                obj.CContainer.Update();
            }
        }

        /// <summary>
        /// Adds an object to the world.
        /// </summary>
        /// <param name="obj">The object to be added to the world.</param>
        public void Add(SCEObject obj)
        {
            objectList.Add(obj);
        }

        /// <summary>
        /// Adds a every object in an object array to the world.
        /// </summary>
        /// <param name="objArray">The object array containing every object to be added to the world.</param>
        public void Add(SCEObject[] objArray)
        {
            foreach (SCEObject obj in objArray)
            {
                Add(obj);
            }
        }

        /// <summary>
        /// Removes the first occurance of a specified object from the world.
        /// </summary>
        /// <param name="obj">The object to remove from the world.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(SCEObject obj)
        {
            return objectList.Remove(obj);
        }

        /// <summary>
        /// Removes the first occurance of an object with a name matching specified name.
        /// </summary>
        /// <param name="name">The name of the object to remove from the world.</param>
        /// <returns><see langword="true"/> if an object with a matching <paramref name="name"/> is successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(string name)
        {
            bool found = Contains(name, out int index);

            if (found)
            {
                RemoveAt(index);
            }

            return found;
        }

        /// <summary>
        /// Removes the object at the specified index of the world.
        /// </summary>
        /// <param name="index">The zero-based index of the object to remove.</param>
        public void RemoveAt(int index)
        {
            objectList.RemoveAt(index);
        }

        /// <summary>
        /// Indicates whether an object with a name matching the specified name is found in the world.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <param name="index">Outputs the zero-based index of the object if found; otherwise, -1.</param>
        /// <returns><see langword="true"/> if an object with a name matching the specified name is found in the world is found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(string name, out int index)
        {
            bool found = false;
            int i = 0;
            do
            {
                if (this[i].Name == name)
                {
                    found = true;
                }
                else
                {
                    i++;
                }
            }
            while (i < Objects && !found);

            index = found ? i : -1;

            return found;
        }

        /// <summary>
        /// Indicates whether an object with a name matching the specified name is found in the world.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns><see langword="true"/> if an object with a name matching the specified name is found in the world is found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(string name)
        {
            return Contains(name, out _);
        }

        /// <summary>
        /// Returns the first occurance of an object with a name matching the specified name in the world.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>The first occurance of an object with a name matching the specified <paramref name="name"/> in the world.</returns>
        /// <exception cref="ArgumentException">Thrown if no object with the given <paramref name="name"/> is found in the world.</exception>
        public SCEObject Find(string name)
        {
            bool found = Contains(name, out int index);

            if(!found)
            {
                throw new ArgumentException("No object with specified name found.");
            }

            return this[index];
        }

        /// <summary>
        /// Searches for an object with a name matcing the specified name and returns the zero-based index of the first occurance within the entire world.
        /// </summary>
        /// <param name="name">The name of the object to locate in the world.</param>
        /// <returns>The zero-based index of the first occurance within the entire world, if found; othewise, -1.</returns>
        public int IndexOf(string name)
        {
            Contains(name, out int index);

            return index;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurance within the entire world.
        /// </summary>
        /// <param name="obj">The object to locate in the world.</param>
        /// <returns>The zero-based index of the first occurance within the entire world, if found; othewise, -1.</returns>
        public int IndexOf(SCEObject obj)
        {
            return objectList.IndexOf(obj);
        }
    }
}
