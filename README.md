# PowerShellClient
.Net Standard PowerShell client with a SqlClient based API.


### Examples:

Reading a single result:
```
	var command = new PowerShellCommand("My-Command");
	command.AddArgument("ArgValue");
	
	// OR:
	command.AddParameter("ArgValue", "ArgName");

	var result = command.ExecuteScalar();
```

Reading a hashtable:
```
var command = new PowerShellCommand("Get-ServerMemory");

var servers = new List<Server>();
using (var reader = command.ExecuteDataReader())
{
	while (reader.Read())
	{
		servers.Add(new Server()
		{
			Name = reader["Name"],
			Memory = int.Parse(reader["MemoryInMb"])
		});
	}
}
```

See the tests for more.