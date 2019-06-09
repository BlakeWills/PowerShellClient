using NUnit.Framework;
using System;

namespace PowerShellClient.Tests
{
    [TestFixture]
    public class PowerShellCommandTests
    {
        [Test]
        public void ExecuteScalar_ReturnsSingleValue()
        {
            const string expectedResult = "Hello, World!";

            var command = new PowerShellCommand($"Write-Output '{expectedResult}'");

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\t")]
        [TestCase(null)]
        public void GivenNullOrEmptyCommand_ThrowsArgumentNullException(string commandText)
        {
            Assert.Throws<ArgumentNullException>(() => new PowerShellCommand(commandText));
        }

        [Test]
        public void ExecuteScalar_WhenCommandReturnsNoOutput_ReturnsEmptyString()
        {
            var command = new PowerShellCommand("function Get-HostsInClusterTest {} Get-HostsInClusterTest");

            var result = command.ExecuteScalar();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ExecuteScalar_WhenCommandReturnsNull_ReturnsEmptyString()
        {
            var command = new PowerShellCommand("$null");

            var result = command.ExecuteScalar();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void AddArgument_GivenArgument_WhenCommandIsRunArgumentValueIsAppliedToScript()
        {
            const string script = "function MyTestFunc { param([int]$arg) Write-Output $arg } MyTestFunc";
            const int expectedResult = 10;

            var command = new PowerShellCommand(script);
            command.AddArgument(expectedResult);

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult.ToString(), result);
        }

        [Test]
        public void AddArgument_GivenStringArgument_WhenCommandIsRunArgumentValueIsQuoted()
        {
            const string script = "function MyTestFunc { param([string]$arg) Write-Output $arg } MyTestFunc";
            const string expectedResult = "Hello, World"; // Would fail due to the comma if not quoted.

            var command = new PowerShellCommand(script);
            command.AddArgument(expectedResult);

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddParameter_GivenParameter_WhenCommandIsRunParameterIsAppliedToScript()
        {
            const string script = "function MyTestFunc { param([int]$arg) Write-Output $arg } MyTestFunc";
            const int expectedResult = 10;

            var command = new PowerShellCommand(script);
            command.AddParameter(expectedResult, "arg");

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult.ToString(), result);
        }

        [Test]
        public void ExecuteScalar_WhenCommandExecutionReturnsNonZeroExitCode_PowerShellExceptionIsThrown()
        {
            var command = new PowerShellCommand("ThisCmdletDoesNotExist");

            Assert.Throws<PowerShellException>(() => command.ExecuteScalar());
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\t")]
        [TestCase(null)]
        public void AddParameter_GivenNullOrEmptyValue_ThrowsArgumentNullException(string value)
        {
            var command = new PowerShellCommand("Script");
            Assert.Throws<ArgumentNullException>(() => command.AddParameter(value, "paramName"));
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\t")]
        [TestCase(null)]
        public void AddArgument_GivenNullOrEmptyValue_ThrowsArgumentNullException(string value)
        {
            var command = new PowerShellCommand("Script");
            Assert.Throws<ArgumentNullException>(() => command.AddArgument(value));
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\t")]
        [TestCase(null)]
        public void AddParameter_GivenNullOrEmptyParamName_ThrowsArgumentNullException(string paramName)
        {
            var command = new PowerShellCommand("Script");
            Assert.Throws<ArgumentNullException>(() => command.AddParameter("value", paramName));
        }
    }
}
