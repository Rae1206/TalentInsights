namespace Shared.Helpers;

/// <summary>
/// Utilidad para generar textos aleatorios.
/// </summary>
public static class Generate
{
    private static readonly Random Random = new();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>
    /// Genera un texto aleatorio de la longitud especificada.
    /// </summary>
    /// <param name="length">Longitud del texto a generar</param>
    /// <returns>Texto aleatorio</returns>
    public static string RandomText(int length)
    {
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = Chars[Random.Next(Chars.Length)];
        }
        return new string(result);
    }
}