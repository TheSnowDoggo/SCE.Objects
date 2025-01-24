namespace SCE
{
    /// <summary>
    /// Allows <see cref="IComponent"/> classes to modularly interface with their holder.
    /// </summary>
    public interface IComponent : ISearcheable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IComponent"/> is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Used to set the parent <see cref="CContainer"/> of the <see cref="IComponent"/> if the <see cref="ICContainerHolder"/> is valid.
        /// </summary>
        /// <param name="cContainer">The <see cref="CContainer"/> to set to the <see cref="IComponent"/>.</param>
        /// <param name="holder">The <see cref="ICContainerHolder"/> to check if it's valid in the <see cref="IComponent"/>.</param>
        void SetCContainer(CContainer? cContainer, ICContainerHolder holder);
    }
}
