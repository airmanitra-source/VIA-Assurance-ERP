using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace ClientApp.E2ETests;

/// <summary>
/// Génère à la volée une image PNG valide et unique pour les tests d'upload.
/// Aucune ressource externe n'est nécessaire : l'image est créée en mémoire puis
/// écrite dans un fichier temporaire que Playwright fournira à l'élément &lt;InputFile&gt;.
/// </summary>
public static class TestImageGenerator
{
    /// <summary>
    /// Crée un fichier PNG temporaire (dégradé coloré) et renvoie son chemin absolu.
    /// </summary>
    /// <param name="fileName">Nom de fichier souhaité (doit être unique pour la vérification BD).</param>
    /// <param name="width">Largeur en pixels.</param>
    /// <param name="height">Hauteur en pixels.</param>
    /// <returns>Le chemin absolu du fichier PNG généré.</returns>
    public static string CreatePngFile(string fileName, int width = 240, int height = 160)
    {
        using var image = new Image<Rgba32>(width, height);

        // Remplit l'image d'un dégradé déterministe pour produire un PNG non trivial.
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var r = (byte)(x * 255 / width);
                var g = (byte)(y * 255 / height);
                var b = (byte)((x + y) * 255 / (width + height));
                image[x, y] = new Rgba32(r, g, b, 255);
            }
        }

        var directory = Path.Combine(Path.GetTempPath(), "erp-assur-e2e");
        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, fileName);
        image.Save(filePath, new PngEncoder());

        return filePath;
    }
}
