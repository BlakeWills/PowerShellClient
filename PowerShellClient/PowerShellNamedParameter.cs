namespace PowerShellClient
{
    internal class PowerShellNamedParameter : PowerShellParameter
    {
        public PowerShellNamedParameter(string name, object value) : base(value)
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
