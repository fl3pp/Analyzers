using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.Helpers;

internal record struct TextCharacter(int Index, char Character, SourceText SourceText)
{
    public TextCharacter? Previous() => SourceText.GetTextCharacter(Index - 1);
    public TextCharacter? Next() => SourceText.GetTextCharacter(Index + 1);
}

internal static class TextCharacterExtensions
{
    public static IEnumerable<TextCharacter> EnumerateTextCharacters(this SourceText text, TextSpan span)
    {
        return Enumerable.Range(span.Start, span.Length)
            .Select(i => GetTextCharacter(text, i)!.Value);
    }
    
    public static IEnumerable<TextCharacter> EnumerateTextCharactersReverse(this SourceText text, TextSpan span)
    {
        return Enumerable.Range(0, span.Length)
            .Select(i => GetTextCharacter(text, span.End - i - 1)!.Value);
    }
 
    public static TextCharacter? GetTextCharacter(this SourceText text, int index)
    {
        if (index < 0 || index >= text.Length) return null;
        return new(index, text[index], text);
    }
}