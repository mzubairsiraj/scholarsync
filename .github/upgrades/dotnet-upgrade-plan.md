# .NET 8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade ScholarSync\ScholarSync.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                              | Current Version | New Version | Description                                        |
|:------------------------------------------|:---------------:|:-----------:|:---------------------------------------------------|
| Microsoft.Bcl.AsyncInterfaces             | 1.1.0           | 8.0.0       | Recommended for .NET 8.0                           |
| System.Buffers                            | 4.5.0           |             | Functionality included with .NET 8.0 framework     |
| System.Memory                             | 4.5.3           |             | Functionality included with .NET 8.0 framework     |
| System.Numerics.Vectors                   | 4.5.0           |             | Functionality included with .NET 8.0 framework     |
| System.Runtime.CompilerServices.Unsafe    | 4.6.0           | 6.1.2       | Recommended for .NET 8.0                           |
| System.Text.Encodings.Web                 | 4.6.0           | 10.0.2      | Security vulnerability - upgrade required          |
| System.Text.Json                          | 4.6.0           | 8.0.6       | Recommended for .NET 8.0                           |
| System.Threading.Tasks.Extensions         | 4.5.3           |             | Functionality included with .NET 8.0 framework     |
| System.ValueTuple                         | 4.5.0           |             | Functionality included with .NET 8.0 framework     |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### ScholarSync\ScholarSync.csproj modifications

Project conversion:
  - Project file needs to be converted from traditional .NET Framework format to SDK-style project format

Project properties changes:
  - Target framework should be changed from `net481` to `net8.0-windows`

NuGet packages changes:
  - Microsoft.Bcl.AsyncInterfaces should be updated from `1.1.0` to `8.0.0` (*recommended for .NET 8.0*)
  - System.Buffers should be removed (*functionality included with .NET 8.0 framework*)
  - System.Memory should be removed (*functionality included with .NET 8.0 framework*)
  - System.Numerics.Vectors should be removed (*functionality included with .NET 8.0 framework*)
  - System.Runtime.CompilerServices.Unsafe should be updated from `4.6.0` to `6.1.2` (*recommended for .NET 8.0*)
  - System.Text.Encodings.Web should be updated from `4.6.0` to `10.0.2` (*security vulnerability - upgrade required*)
  - System.Text.Json should be updated from `4.6.0` to `8.0.6` (*recommended for .NET 8.0*)
  - System.Threading.Tasks.Extensions should be removed (*functionality included with .NET 8.0 framework*)
  - System.ValueTuple should be removed (*functionality included with .NET 8.0 framework*)
