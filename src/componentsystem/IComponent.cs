namespace SCE
{
    /// <summary>
    /// Allows <see cref="IComponent"/> classes to modularly interface with their holder.
    /// </summary>
    public interface IComponent
    {
        bool IsActive { get; }

        void SetCContainer(CContainer? cContainer, ICContainerHolder holder);
    }
}
