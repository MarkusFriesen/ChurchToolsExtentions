using ChurchToolsExtentions.Models;

namespace ChurchToolsExtentions;

public class FileFormatter(FormatSettings settings)
{
    readonly List<string> newLines = ["----", "---", "--"];
    public string Format(string content)
    {
        if (settings.MaxNumberOfLines is null) return content;

        var trimmedContent = content.Trim().ReplaceLineEndings().Split(Environment.NewLine);

        if (trimmedContent.Length == 1)
        {
            trimmedContent = content.Trim().Split();
        }

        if (trimmedContent.Last().StartsWith("--"))
        {
            trimmedContent = trimmedContent.SkipLast(1).ToArray();
        }

        var result = new List<string>();
        bool headerComplete = false;

        int currentLine = 0;
        List<string>? verseOrder = null;
        
        for (int index = 0; index < trimmedContent.Length; index++)
        {
            var line = trimmedContent[index];
            var trimmedLine = line.Trim();
            result.Add(line);

            if (newLines.Contains(trimmedLine))
            {
                headerComplete = true;
                currentLine = 0;
                continue;
            }

            if (!headerComplete)
            {
                verseOrder ??= TryExtractVerseOrder(line);
                continue;
            }

            if (!IsVerseOrderDirection(verseOrder, trimmedLine))
            {
                currentLine++;
            }

            if (currentLine >= settings.MaxNumberOfLines)
            {
                currentLine = 0;

                if (index >= trimmedContent.Length - 1)
                    continue;

                if (!newLines.Contains(trimmedContent[index + 1].Trim()))
                    result.Add("---");
            }
        }

        return string.Join(Environment.NewLine, result);
    }

    private static bool IsVerseOrderDirection(List<string>? verseOrder, string line)
    {
        return (verseOrder?.IndexOf(line) ?? -1) > -1;
    }

    private List<string>? TryExtractVerseOrder(string line)
    {
        string verseOrder = "#verseorder=";
        if (line is null  || !line.ToLower().Trim().StartsWith(verseOrder)) return null;

        return [.. line.Trim()[verseOrder.Length..].Split(',').Distinct()];
    }
}
