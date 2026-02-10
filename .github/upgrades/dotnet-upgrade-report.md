# .NET 8.0 Upgrade Report

## Project target framework modifications

| Project name                       | Old Target Framework | New Target Framework | Commits                          |
|:-----------------------------------|:--------------------:|:--------------------:|----------------------------------|
| ScholarSync\ScholarSync.csproj     | net481               | net8.0-windows       | 7625df38, d9cec1f5               |

## NuGet Packages

| Package Name                           | Old Version | New Version | Commit Id                        |
|:---------------------------------------|:-----------:|:-----------:|----------------------------------|
| Microsoft.Bcl.AsyncInterfaces          | 1.1.0       | 8.0.0       | d9cec1f5                         |
| System.Buffers                         | 4.5.0       | (removed)   | d9cec1f5                         |
| System.Memory                          | 4.5.3       | (removed)   | d9cec1f5                         |
| System.Numerics.Vectors                | 4.5.0       | (removed)   | d9cec1f5                         |
| System.Runtime.CompilerServices.Unsafe | 4.6.0       | 6.1.2       | d9cec1f5                         |
| System.Text.Encodings.Web              | 4.6.0       | 10.0.2      | d9cec1f5                         |
| System.Text.Json                       | 4.6.0       | 8.0.6       | d9cec1f5                         |
| System.Threading.Tasks.Extensions      | 4.5.3       | (removed)   | d9cec1f5                         |
| System.ValueTuple                      | 4.5.0       | (removed)   | d9cec1f5                         |

## Project feature upgrades

### ScholarSync\ScholarSync.csproj

Here is what changed for the project during upgrade:

- **Project converted to SDK-style format**: The traditional .NET Framework project format was converted to the modern SDK-style project format, which provides a simplified and more maintainable project structure.

- **Target framework upgraded**: Changed from .NET Framework 4.8.1 (net481) to .NET 8.0 for Windows (net8.0-windows).

- **Assembly references cleaned up**: Removed 11 legacy assembly references that are now implicitly included in .NET 8.0:
  - Microsoft.CSharp
  - System, System.Core, System.Data
  - System.Data.DataSetExtensions
  - System.Deployment
  - System.Drawing
  - System.Net.Http
  - System.Numerics
  - System.Windows.Forms
  - System.Xml, System.Xml.Linq

- **NuGet packages updated**: 
  - Upgraded Microsoft.Bcl.AsyncInterfaces from 1.1.0 to 8.0.0
  - Upgraded System.Runtime.CompilerServices.Unsafe from 4.6.0 to 6.1.2
  - Upgraded System.Text.Encodings.Web from 4.6.0 to 10.0.2 (security vulnerability fixed)
  - Upgraded System.Text.Json from 4.6.0 to 8.0.6

- **Obsolete packages removed**: Removed 5 packages whose functionality is now included in .NET 8.0:
  - System.Buffers
  - System.Memory
  - System.Numerics.Vectors
  - System.Threading.Tasks.Extensions
  - System.ValueTuple

- **Project files cleaned up**: Removed AssemblyInfo.cs and packages.config files as these are no longer needed in SDK-style projects.

## All commits

| Commit ID | Description                                                                                      |
|:----------|:-------------------------------------------------------------------------------------------------|
| 971ab130  | Commit upgrade plan                                                                              |
| 7625df38  | Migrate ScholarSync to SDK-style project and .NET 8                                              |
| d9cec1f5  | Update ScholarSync.csproj package references                                                     |

## Next steps

- **Build and test your application**: Run your application to ensure it works correctly with .NET 8.0
- **Review breaking changes**: Check the [.NET 8.0 breaking changes documentation](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0) for any API changes that might affect your code
- **Update CI/CD pipelines**: Update your build and deployment pipelines to use .NET 8.0 SDK
- **Consider upgrading to .NET 9.0**: .NET 9.0 is now available if you want to use the latest features
