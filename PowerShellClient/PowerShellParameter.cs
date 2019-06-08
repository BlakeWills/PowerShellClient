namespace PowerShellClient
{
    internal class PowerShellParameter
    {
        public PowerShellParameter(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override string ToString()
        {
            if(Value.GetType() == typeof(string))
            {
                return $"'{Value.ToString()}'";
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}
