using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerShellClient.Tests
{
    [TestFixture]
    public class PowerShellDataReaderTests
    {
        private const string _multipleRecordsScript = @"
                function Get-TabularData
                {
                    $rows = @()
	                $rows += Select-Object @{n='ColumnOne';e={'ValueOne'}},@{n='ColumnTwo';e={100}} -InputObject '';
                    $rows += Select-Object @{n='ColumnOne';e={'ValueTwo'}},@{n='ColumnTwo';e={200}} -InputObject '';
                    $rows += Select-Object @{n='ColumnOne';e={'ValueThree'}},@{n='ColumnTwo';e={300}} -InputObject '';
                    return $rows
                }
                Get-TabularData";

        private const string _valueWithSpaceScript = @"
                function Get-TabularData
                {
                    $rows = @()
	                $rows += Select-Object @{n='ColumnOne';e={'Value One'}},@{n='ColumnTwo';e={100}} -InputObject '';
                    return $rows
                }
                Get-TabularData";

        private const string _columnNameWithSpaceScript = @"
                function Get-TabularData
                {
                    $rows = @()
	                $rows += Select-Object @{n='Column One';e={'ValueOne'}},@{n='Column Two';e={100}} -InputObject '';
                    return $rows
                }
                Get-TabularData";

        private const string _valueWithMultipleColonsScript = @"
                function Get-TabularData
                {
                    $rows = @()
	                $rows += Select-Object @{n='ColumnOne';e={'The:Value:Is:One'}},@{n='ColumnTwo';e={100}} -InputObject '';
                    return $rows
                }
                Get-TabularData";

        private const string _nullValueScript = @"
                function Get-TabularData
                {
                    $rows = @()
	                $rows += Select-Object @{n='ColumnOne';e={$null}},@{n='ColumnTwo';e={100}} -InputObject '';
                    return $rows
                }
                Get-TabularData";

        private const string _noResultsScript = @"
                function Get-TabularData
                {
                    return @()
                }
                Get-TabularData";

        private const string _nonTabularDataScript = @"
                function Get-NonTabularData
                {
                    return 'Hello, world!'
                }
                Get-NonTabularData";

        [Test]
        public void ExecuteReader_WhenResultsSetHasMultipleRows_RowsCanBeReadSequentially()
        {
            var results = GetResultsForScript(_multipleRecordsScript);

            var expectedResults = new List<Result>()
            {
                new Result() { ColumnOne = "ValueOne", ColumnTwo = 100 },
                new Result() { ColumnOne = "ValueTwo", ColumnTwo = 200 },
                new Result() { ColumnOne = "ValueThree", ColumnTwo = 300 }
            };

            results.Should().BeEquivalentTo(expectedResults);
        }

        [Test]
        public void ExecuteReader_WhenValueContainsSpaces_CorrectValueIsReturned()
        {
            var results = GetResultsForScript(_valueWithSpaceScript);

            var expectedResult = new Result() { ColumnOne = "Value One", ColumnTwo = 100 };

            results.Single().Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void ExecuteReader_WhenColumnNameContainsSpaces_ValueCanBeLookedUp()
        {
            var results = GetResultsForScript(_columnNameWithSpaceScript, "Column One", "Column Two");

            var expectedResult = new Result() { ColumnOne = "ValueOne", ColumnTwo = 100 };

            results.Single().Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void ExecuteReader_WhenFieldIncludesMultipleColons_ColumnNameIsSplitAtFirstIndexOfColon()
        {
            var results = GetResultsForScript(_valueWithMultipleColonsScript);

            var expectedResult = new Result() { ColumnOne = "The:Value:Is:One", ColumnTwo = 100 };

            results.Single().Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void ExecuteReader_WhenFieldValueIsNull_ReturnEmptyString()
        {
            var results = GetResultsForScript(_nullValueScript);

            var expectedResult = new Result() { ColumnOne = string.Empty, ColumnTwo = 100 };

            results.Single().Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void ExecuteReader_WhenThereAreNoResults_ReaderCannotBeRead()
        {
            var results = GetResultsForScript(_noResultsScript);

            results.Should().BeEmpty();
        }

        [Test]
        public void ExecuteReader_WhenTheCommandResultIsNotTabular_PowerShellExceptionIsThrown()
        {
            Assert.Throws<PowerShellException>(() => GetResultsForScript(_nonTabularDataScript));
        }

        [Test]
        public void FieldIndexer_GivenInvalidColumnName_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => GetResultsForScript(_multipleRecordsScript, columnOneName: "InvalidColumnName"));
        }

        [Test]
        public void FieldIndexer_IfAccessedBeforeReadIsCalled_ThrowsInvalidOperationException()
        {
            var command = new PowerShellCommand(_multipleRecordsScript);
            using(var reader = command.ExecuteDataReader())
            {
                Assert.Throws<InvalidOperationException>(() => _ = reader["ColumnOne"]);
            }
        }

        [Test]
        public void FieldIndexer_IfAccessedWhenReaderCannotBeRead_ThrowsInvalidOperationException()
        {
            var command = new PowerShellCommand(_noResultsScript);
            using (var reader = command.ExecuteDataReader())
            {
                Assert.IsFalse(reader.Read());
                Assert.Throws<InvalidOperationException>(() => _ = reader["ColumnOne"]);
            }
        }

        private static List<Result> GetResultsForScript(string script, string columnOneName = "ColumnOne", string columnTwoName = "ColumnTwo")
        {
            var command = new PowerShellCommand(script);

            var results = new List<Result>();
            using (var reader = command.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    results.Add(new Result()
                    {
                        ColumnOne = reader[columnOneName],
                        ColumnTwo = string.IsNullOrWhiteSpace(reader[columnTwoName]) ? default(int?) : int.Parse(reader[columnTwoName])
                    });
                }
            }

            return results;
        }

        private class Result
        {
            public string ColumnOne { get; set; }

            public int? ColumnTwo { get; set; }
        }
    }
}
