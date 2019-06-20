using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

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
        public void AddArgument_GivenStringArgumentWithDefaultQuoteOptions_WhenCommandIsRunArgumentValueIsQuoted()
        {
            const string script = "function MyTestFunc { param([string]$arg) Write-Output $arg } MyTestFunc";
            const string expectedResult = "Hello, World";

            var command = new PowerShellCommand(script);
            command.AddArgument(expectedResult);

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddArgument_GivenStringArgumentAndQuoteOptionsSetToQuote_WhenCommandIsRunArgumentValueIsQuoted()
        {
            const string script = "function MyTestFunc { param([string]$arg) Write-Output $arg } MyTestFunc";
            const string expectedResult = "Hello, World";

            var command = new PowerShellCommand(script);
            command.AddArgument(expectedResult, ParameterQuoteOptions.Quote);

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddArgument_GivenStringArgumentAndQuoteOptionsSetToNoQuote_WhenCommandIsRunArgumentValueIsNotQuoted()
        {
            const string script = "function MyTestFunc { param([string]$arg) Write-Output $arg } MyTestFunc";
            const string expectedResult = "Hello, World"; 

            var command = new PowerShellCommand(script);
            command.AddArgument(expectedResult, ParameterQuoteOptions.NoQuotes);

            var result = command.ExecuteScalar();

            // No comma as the arg was not quoted, this means the comma was seen as an argument seperator and not part of the value.
            Assert.AreEqual("Hello World", result);
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

        [Test]
        public void AddParameter_GivenStringArgumentWithDefaultQuoteOptions_WhenCommandIsRunArgumentValueIsQuoted()
        {
            const string script = "function MyTestFunc { param([string]$arg) Write-Output $arg } MyTestFunc";
            const string expectedResult = "Hello, World";

            var command = new PowerShellCommand(script);
            command.AddParameter(expectedResult, "arg");

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddParameter_GivenStringArgumentAndQuoteOptionsSetToQuote_WhenCommandIsRunArgumentValueIsQuoted()
        {
            const string script = "function MyTestFunc { param([string]$arg) Write-Output $arg } MyTestFunc";
            const string expectedResult = "Hello, World";

            var command = new PowerShellCommand(script);
            command.AddParameter(expectedResult, "arg", ParameterQuoteOptions.Quote);

            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void AddParameter_GivenStringArgumentAndQuoteOptionsSetToNoQuote_WhenCommandIsRunArgumentValueIsNotQuoted()
        {
            const string script = "function MyTestFunc { param([string]$arg) Write-Output $arg } MyTestFunc";
            const string expectedResult = "Hello, World";

            var command = new PowerShellCommand(script);
            command.AddParameter(expectedResult, "arg", ParameterQuoteOptions.NoQuotes);

            var result = command.ExecuteScalar();

            // No comma as the arg was not quoted, this means the comma was seen as an argument seperator and not part of the value.
            Assert.AreEqual("Hello World", result);
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\t")]
        [TestCase(null)]
        public void SetWorkingDirectory_GivenNullOrEmptyDirectory_ThrowsArgumentNullException(string directory)
        {
            var command = new PowerShellCommand("Script");
            Assert.Throws<ArgumentNullException>(() => command.SetWorkingDirectory(directory));
        }

        [Test]
        public void SetWorkingDirectory_GivenDirectoryThatDoesNotExist_ThrowsArgumentException()
        {
            var command = new PowerShellCommand("Script");
            Assert.Throws<ArgumentException>(() => command.SetWorkingDirectory(Path.Combine(Environment.CurrentDirectory, "IDoNotExist_1234098765")));
        }

        [Test]
        public void Execute_WhenWorkingDirectoryIsSpecified_ProcessIsStartedInWorkingDirectory()
        {
            const string script = "$(pwd).Path"; // Print Working Directory

            // Use GetParent as the default working dir is the executing assembly location.
            var expectedPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).ToString();

            var command = new PowerShellCommand(script);
            command.SetWorkingDirectory(expectedPath);
            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedPath, result);
        }
    }
}
