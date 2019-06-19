using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PowerShellClient.Tests
{
    [TestFixture]
    public class ExecuteStreamTests
    {
        [Test]
        public async Task GivenLongRunningScript_AllOutputIsCaptured()
        {
            var script = @"
                Write-Output 'Started';
                Start-Sleep -Milliseconds 500;
                Write-Output 'Finished';";

            var output = await ExecuteStream(script);

            Assert.AreEqual($"Started{Environment.NewLine}Finished", output);
        }

        [TestCase("Write-Output 'Output'", "Output")]
        [TestCase("Write-Warning 'Warning!'", "WARNING: Warning!")]
        [TestCase("Write-Debug 'Some Debug Data!'", "DEBUG: Some Debug Data!")]
        [TestCase("Write-Information 'Info!'", "Info!")]
        [TestCase("Write-Verbose 'Verbose!'", "VERBOSE: Verbose!")]
        public async Task AllOutputStreamsAreStreamedToClient(string script, string expectedOutput)
        {
            var debugPref = $"$DebugPreference = \"\"\"Continue\"\"\";{Environment.NewLine}";
            var infoPref = $"$InformationPreference = \"\"\"Continue\"\"\";{Environment.NewLine}";
            var verbosePref = $"$VerbosePreference = \"\"\"Continue\"\"\";{Environment.NewLine}";

            var scriptWithPrefs = $"{debugPref}{infoPref}{verbosePref}{Environment.NewLine}{script}";

            var output = await ExecuteStream(scriptWithPrefs);

            Assert.AreEqual(expectedOutput, output, message: $"Input: {script}");
        }

        [Test]
        public async Task StandardErrorIsStreamedToClient()
        {
            const string script = "Write-Error 'FatalError!'";

            var output = await ExecuteStream(script);

            // We don't filter the output.
            var expectedOutput = @"Write-Error 'FatalError!' : FatalError!
    + CategoryInfo          : NotSpecified: (:) [Write-Error], WriteErrorException
    + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorException";

            Assert.AreEqual(expectedOutput, output);
        }

        private async Task<string> ExecuteStream(string script)
        {
            var command = new PowerShellCommand(script);
            var output = new StringBuilder();

            await command.ExecuteStreamAsync((sender, args) =>
            {
                output.AppendLine(args.Data);
            });

            return output.ToString().Trim();
        }
    }
}
