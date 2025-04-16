# fl3pp.Analyzers

A collection of analyzers I wished existed but didn't yet.


## Installation

To install the analyzers, add the NuGet package to your `.csproj`:

```xml
<PackageReference Include="fl3pp.Analyzers" Version="0.0.1" PrivateAssets="all" />
```

Note that all analyzers are disabled by default. You'll have to enable the ones you need in your [`.editorconfig`](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022#set-rule-severity-in-an-editorconfig-file) file.

Use a [`Directory.Build.props`](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022#directorybuildprops-and-directorybuildtargets) to enable the analyzers for all projects in a directory.


## `FL30001`: Maximum line length exceeded

Warns if a line exceeds the maximum line length.

```editorconfig
[*.cs]
dotnet_diagnostic.FL30001.severity = warning # default: none
max_line_length = 100 # default: 120
```

The `max_line_length` option can be set to any positive integer. If not value is set, the `guidelines` option is used instead (for compatibility with the [EditorGuidelines](https://marketplace.visualstudio.com/items?itemName=PaulHarrington.EditorGuidelines) VS Extension). If no value can be found, a default value of `120` is used.
