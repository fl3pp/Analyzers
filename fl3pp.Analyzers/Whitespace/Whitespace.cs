namespace fl3pp.Analyzers.Whitespace;

public static class Whitespace
{
    private static readonly IReadOnlyCollection<char> s_whitespaceChars = new HashSet<char> { ' ', '\t' };
    public static bool IsWhitespace(this char c) => s_whitespaceChars.Contains(c);
}