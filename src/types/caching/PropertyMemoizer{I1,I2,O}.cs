namespace SCE
{
    public class PropertyMemoizer<I1,I2,O>
    {
        private I1? _lastInput1;
        private I2? _lastInput2;
        private O? _lastOutput;

        public PropertyMemoizer(Func<I1, I2, O> getFunc)
        {
            GetFunc = getFunc;
        }

        public Func<I1, I2, O> GetFunc { get; }

        public O Get(I1 i1, I2 i2)
        {
            if ((i1?.Equals(_lastInput1) ?? false) && (i2?.Equals(_lastInput2) ?? false) && _lastOutput is not null)
                return _lastOutput;
            return _lastOutput = GetFunc.Invoke(_lastInput1 = i1, _lastInput2 = i2);
        }
    }
}
