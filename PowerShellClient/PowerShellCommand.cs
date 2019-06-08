using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PowerShellClient
{
    public class PowerShellCommand
    {
        private readonly string _script;
        private readonly List<PowerShellParameter> _parameters;

        public PowerShellCommand(string script)
        {
            _script = script;
            _parameters = new List<PowerShellParameter>();
        }

        public int CommandTimeoutSeconds { get; set; } = 30;

        public void AddArgument(object value)
        {
            _parameters.Add(new PowerShellParameter(value));
        }

        public void AddParameter(object value, string name)
        {
            _parameters.Add(new PowerShellNamedParameter(name, value));
        }

        public string ExecuteScalar()
        {
            return Execute().ReadToEnd().Trim();
        }

        public PowerShellDataReader ExecuteDataReader()
        {
            return new PowerShellDataReader(Execute());
        }

        private StreamReader Execute()
        {
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = "powershell",
                Arguments = GetCommandString(),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var ps = new Process())
            {
                ps.StartInfo = processStartInfo;

                ps.Start();
                ps.WaitForExit(CommandTimeoutSeconds * 1000);

                if (ps.ExitCode != 0)
                {
                    var errorMessage = ps.StandardError.ReadToEnd();
                    throw new PowerShellException(errorMessage);
                }

                return ps.StandardOutput;
            }
        }

        private string GetCommandString()
        {
            var builder = new StringBuilder(_script);

            foreach(var param in _parameters)
            {
                builder.Append($" {param.ToString()} ");
            }

            return builder.ToString();
        }
    }
}
