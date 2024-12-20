namespace SCE
{
    internal class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException()
            : base()
        {
        }
        public ComponentNotFoundException(string? message)
            : base(message)
        {
        }
    }
}
