namespace SCE
{
    public interface IRenderRule
    {
        bool IsActive { get; }

        bool ShouldRender(SCEObject obj);
    }
}
