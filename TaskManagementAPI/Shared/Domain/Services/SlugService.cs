using System.Text;
using System.Text.RegularExpressions;

namespace TaskManagementAPI.Shared.Domain.Services;

/// <summary>
/// Service for generating SEO-friendly URL slugs from text.
/// Handles URL encoding, special character removal, and duplicate slug handling.
/// </summary>
public static class SlugService
{
    /// <summary>
    /// Generates a URL-friendly slug from the given text.
    /// Converts to lowercase, removes special characters, and replaces spaces with hyphens.
    /// </summary>
    /// <param name="text">The text to convert to a slug.</param>
    /// <returns>A URL-friendly slug.</returns>
    /// <example>
    /// "My Awesome Project!" becomes "my-awesome-project"
    /// "Hello & Goodbye" becomes "hello-goodbye"
    /// </example>
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        var slug = text.ToLowerInvariant();

        // Remove accents and diacritics
        slug = RemoveDiacritics(slug);

        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");

        // Remove any character that is not alphanumeric or hyphen
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", string.Empty);

        // Replace multiple consecutive hyphens with a single hyphen
        slug = Regex.Replace(slug, @"-+", "-");

        // Remove leading and trailing hyphens
        slug = slug.Trim('-');

        // Ensure slug is not empty
        if (string.IsNullOrEmpty(slug))
            slug = "untitled";

        // Limit to 100 characters for URL optimization
        if (slug.Length > 100)
            slug = slug.Substring(0, 100).TrimEnd('-');

        return slug;
    }

    /// <summary>
    /// Generates a unique slug by appending a number if the slug already exists.
    /// </summary>
    /// <param name="baseSlug">The base slug to make unique.</param>
    /// <param name="existingSlugs">Collection of existing slugs to check against.</param>
    /// <returns>A unique slug.</returns>
    /// <example>
    /// If "my-project" exists, returns "my-project-2"
    /// If "my-project-2" exists, returns "my-project-3"
    /// </example>
    public static string GenerateUniqueSlug(string baseSlug, IEnumerable<string> existingSlugs)
    {
        if (!existingSlugs.Contains(baseSlug))
            return baseSlug;

        var counter = 2;
        var uniqueSlug = $"{baseSlug}-{counter}";

        while (existingSlugs.Contains(uniqueSlug))
        {
            counter++;
            uniqueSlug = $"{baseSlug}-{counter}";
        }

        return uniqueSlug;
    }

    /// <summary>
    /// Removes diacritical marks (accents) from text.
    /// Converts accented characters to their base equivalents.
    /// </summary>
    /// <param name="text">The text to process.</param>
    /// <returns>Text without diacritical marks.</returns>
    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
