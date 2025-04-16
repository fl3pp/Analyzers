# fl3pp.Analyzers

A collection of analyzers I wished existed but didn't yet.

## Installation

To install the analyzers, add the NuGet package to your `.csproj`:

```
<PackageReference Include="fl3pp.Analyzers" Version="0.0.1" />
```

Use a [`Directory.Build.props`](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022#directorybuildprops-and-directorybuildtargets)
to enable the analyzers for all projects in a directory.

## `FL30001`: Maximum line length exceeded

Warns if a line exceeds the maximum line length.

```editorconfig
[*.cs]
dotnet_diagnostic.FL30001.severity = warning
max_line_length = 100
```

The `max_line_length` option can be set to any positive integer.
If not value is set, the `guidelines` option is used instead (for compatibility with the
    [EditorGuidelines](https://marketplace.visualstudio.com/items?itemName=PaulHarrington.EditorGuidelines) VS Extension). 
If no value can be found, a default value of `120` is used.
