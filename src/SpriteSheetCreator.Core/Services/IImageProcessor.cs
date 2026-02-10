using System.Collections.Generic;
using System.Threading.Tasks;
using SpriteSheetCreator.Core.Models;

namespace SpriteSheetCreator.Core.Services;

public interface IImageProcessor
{
    /// <summary>
    /// Load image files from folder
    /// </summary>
    Task<List<string>> LoadImageFilesAsync(string folderPath);

    /// <summary>
    /// Generate a single preview frame (cropped)
    /// </summary>
    Task<byte[]> GeneratePreviewFrameAsync(string filePath, CropRect crop);

    /// <summary>
    /// Generate and save the sprite sheet
    /// </summary>
    Task<string> GenerateAndSaveSpriteSheetAsync(List<string> filePaths, SpriteSheetConfig config, string outputPath);
}
