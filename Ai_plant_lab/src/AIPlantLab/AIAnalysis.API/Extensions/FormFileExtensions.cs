namespace AIAnalysis.API.Extensions;

public static class FormFileExtensions
{
    /// <summary>
    /// Asynchronously converts an IFormFile into a byte array with optimized memory allocation.
    /// </summary>
    public static async Task<byte[]> ReadAsBytesAsync(this IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return [];
        }

        using var memoryStream = new MemoryStream((int)file.Length);
        await file.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }
}