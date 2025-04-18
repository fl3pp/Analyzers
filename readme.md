# fl3pp.Analyzers

A collection of analyzers I wished existed but didn't yet (or did, but in a form that I didn't like).

- Whitespace analyzers
  - [FL30001: Maximum line length exceeded](#fl30001-maximum-line-length-exceeded)
  - [FL30002: Trailing whitespace](#fl30002-trailing-whitespace)
  - [FL30003: Consecutive empty lines](#fl30003-consecutive-empty-lines) 
  - [FL30004: Empty lines between matching consecutive braces](#fl30004-empty-lines-between-matching-consecutive-braces)

## Installation

To install the analyzers, add the NuGet package to your `.csproj`:

```xml
<PackageReference Include="fl3pp.Analyzers" Version="0.0.1" PrivateAssets="all" />
```

Note that all analyzers are disabled by default. You'll have to enable the ones you need in your [`.editorconfig`](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022#set-rule-severity-in-an-editorconfig-file) file.

Use a [`Directory.Build.props`](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022#directorybuildprops-and-directorybuildtargets) to enable the analyzers for all projects in a directory.

## Whitespace Analyzers

### `FL30001`: Maximum line length exceeded

Warns if a line exceeds the maximum line length.

__Configuration__:

```editorconfig
[*.cs]
dotnet_diagnostic.FL30001.severity = warning # default: none
max_line_length = 100 # default: 120
```

The `max_line_length` option can be set to any positive integer. If not value is set, the `guidelines` option is used instead (for compatibility with the [EditorGuidelines](https://marketplace.visualstudio.com/items?itemName=PaulHarrington.EditorGuidelines) VS Extension). If no value can be found, a default value of `120` is used.

__Example__:

```cs
// .editorconfig: max_line_length = 20

/*                 | max length     */
/*                 |                */
class Test
{
    string a = "--";
    string b = "---";
//                  ^ warning
    string c = "-----";
//                  ^^^ warning
}
/*                 |                */
```

__Available Fixes__: _None_

### `FL30002`: Trailing whitespace

Warns if a line contains trailing whitespace.

__Configuration__:

```editorconfig
[*.cs]
dotnet_diagnostic.FL30002.severity = warning # default: none
```

__Example__:

```cs
class Test
//         ^ warning
{
    string a = "";
    string b = ""; 
//                ^ warning
    string c = "";   
//                ^^^ warning
}
```

__Available Fixes__: Trim trailing whitespace

### `FL30003`: Consecutive empty lines

Warns if two or more consecutive line are empty. 

__Configuration__:

```editorconfig
[*.cs]
dotnet_diagnostic.FL30003.severity = warning # default: none
```

__Example__:

```cs

                        // warning
class Test
{
    
    string a = "";
    
                        // warning
    string c = "";   
}

```

__Available Fixes__: Remove consecutive empty lines

### `FL30004`: Empty lines between matching consecutive braces

Warns if one or more empty lines are placed between two matching consecutive braces.

__Configuration__:

```editorconfig
[*.cs]
dotnet_diagnostic.FL30004.severity = warning # default: none
```

__Example__:

```cs
class Test
{
 
    void Test()
    {
 
    }
                    // warning
}
```

__Available Fixes__: Remove empty lines between matching braces
