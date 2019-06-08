using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace PowerShellClient.Tests
{
    [TestFixture]
    public class PowerShellDataReaderTests
    {
        private const string _multipleRecordsScript = @"
                function Get-HostsInClusterTest
                {
                    $nodeInfos = @()
	                $nodeInfos += Select-Object @{n='Name';e={'LEMON'}},@{n='AvailableMemoryInMb';e={4096}} -InputObject '';
	                $nodeInfos += Select-Object @{n='Name';e={'APPLE'}},@{n='AvailableMemoryInMb';e={1024}} -InputObject '';
	                $nodeInfos += Select-Object @{n='Name';e={'PEAR'}},@{n='AvailableMemoryInMb';e={2048}} -InputObject '';
                    return $nodeInfos
                }

                Get-HostsInClusterTest";

        [Test]
        public void ExecuteReader_ReturnsPowerShellDataReader()
        {
            var command = new PowerShellCommand(_multipleRecordsScript);

            var servers = new List<(string name, int availableMemory)>();
            using (var reader = command.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    servers.Add((reader["Name"], int.Parse(reader["AvailableMemoryInMb"])));
                }
            }

            var expectedServerList = new List<(string name, int availableMemory)>()
            {
                (name: "LEMON", availableMemory: 4096),
                (name: "APPLE", availableMemory: 1024),
                (name: "PEAR", availableMemory: 2048)
            };

            servers.Should().BeEquivalentTo(expectedServerList);
        }
    }
}
