using Microsoft.EntityFrameworkCore;
using SoppSnackis.Areas.Identity.Data;

namespace SoppSnackis.Utilities;

public class WordFilter
{

    private readonly SoppSnackisIdentityDbContext _context;
    public WordFilter(SoppSnackisIdentityDbContext context)
    {
        _context = context;
    }
    public async Task<(string filteredText, bool hadForbidden)> FilterForbiddenWordsAsync(string text)
    {
        bool hadForbidden = false;
        if (string.IsNullOrWhiteSpace(text)) return (text, false);
        var filtered = text;

        var forbiddenWords = await _context.ForbiddenWords
        .Select(fw => fw.Word)
        .ToListAsync();

        foreach (var word in forbiddenWords)
        {
            var pattern = $@"\b{System.Text.RegularExpressions.Regex.Escape(word)}\b";
            if (System.Text.RegularExpressions.Regex.IsMatch(filtered, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                hadForbidden = true;
            filtered = System.Text.RegularExpressions.Regex.Replace(
                filtered,
                pattern,
                new string('*', word.Length),
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }
        return (filtered, hadForbidden);
    }
}
