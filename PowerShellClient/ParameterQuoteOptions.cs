namespace PowerShellClient
{
    public enum ParameterQuoteOptions
    {
        /// <summary>
        /// Wrap the value in double quotes before running the command.
        /// </summary>
        Quote = 0,
        
        /// <summary>
        /// Do not wrap the value in quotes. If you are wrapping the value in double quotes yourself, use three quotes.
        /// Caution: This may cause issues if your input contains special characters, such as commas.
        /// </summary>
        NoQuotes = 1
    }
}
