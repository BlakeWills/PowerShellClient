namespace PowerShellClient
{
    internal class PowerShellNamedParameter : PowerShellParameter
    {
        public PowerShellNamedParameter(string name, object value, ParameterQuoteOptions quoteOptions) : base(value, quoteOptions)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"-{Name} {base.ToString()}";
        }
    }
}
