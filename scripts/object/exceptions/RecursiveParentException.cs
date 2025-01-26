namespace SCE
{
    public class RecursiveParentException : Exception
    {
        public RecursiveParentException()
            : base()
        {
        }
        public RecursiveParentException(string? message)
            : base(message)
        {
        }
    }
}
