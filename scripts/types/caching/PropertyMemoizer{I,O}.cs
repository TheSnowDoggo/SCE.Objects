namespace SCE
{
    public class PropertyMemoizer<I, O>
    {
        private I? _lastInput;
        private O? _lastOutput;

        public PropertyMemoizer(Func<I, O> getFunc)
        {
            GetFunc = getFunc;
        }

        public Func<I, O> GetFunc { get; }

        public O Get(I i)
        {
            if ((i?.Equals(_lastInput) ?? false) && _lastOutput is not null)
                return _lastOutput;
            return _lastOutput = GetFunc.Invoke(_lastInput = i);
        }
    }
}
