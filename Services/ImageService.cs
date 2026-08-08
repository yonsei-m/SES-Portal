using Microsoft.AspNetCore.Http;

namespace SES_Portal.Services;

public class ImageService
{
    private readonly IWebHostEnvironment _environment;
    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }
    private static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png"
    };
private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public async Task<string?> SaveEmployeeImageAsync(IFormFile? imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("画像は JPG、JPEG、PNG のみアップロードできます。");
        }

        if (imageFile.Length > MaxFileSize)
        {
            throw new InvalidOperationException("画像サイズは5MB以下にしてください。");
        }

        var fileName = $"{Guid.NewGuid()}{extension}";

        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "employees");

        Directory.CreateDirectory(uploadPath);    

        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"/uploads/employees/{fileName}";
    }

}