namespace PowerShellClient
{
    internal class PowerShellParameter
    {
        public PowerShellParameter(object value, ParameterQuoteOptions quoteOptions)
        {
            Value = value;
            QuoteOptions = quoteOptions;
        }

        public ParameterQuoteOptions QuoteOptions { get; }

        public object Value { get; }

        public override string ToString()
        {
            if(Value.GetType() == typeof(string))
            {
                return GetFormattedValue();
            }
            else
            {
                return Value.ToString();
            }
        }

        private string GetFormattedValue()
        {
            if(QuoteOptions == ParameterQuoteOptions.Quote)
            {
                return $"\"\"\"{Value.ToString()}\"\"\"";
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}
