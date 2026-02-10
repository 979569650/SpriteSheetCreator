using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpriteSheetCreator.Core.Models;

namespace SpriteSheetCreator.Core.Services;

public class ImageProcessor : IImageProcessor
{
    public Task<List<string>> LoadImageFilesAsync(string folderPath)
    {
        return Task.Run(() =>
        {
            if (!Directory.Exists(folderPath))
                return new List<string>();

            var extensions = new[] { ".png", ".jpg", ".jpeg", ".bmp" };
            return Directory.GetFiles(folderPath)
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .OrderBy(f => f)
                .ToList();
        });
    }

    public Task<byte[]> GeneratePreviewFrameAsync(string filePath, CropRect crop)
    {
        return Task.Run(() =>
        {
            using var original = Image.FromFile(filePath);
            
            // Validate crop
            int x = Math.Max(0, crop.X);
            int y = Math.Max(0, crop.Y);
            int w = crop.Width;
            int h = crop.Height;

            if (w <= 0) w = original.Width;
            if (h <= 0) h = original.Height;
            
            if (x + w > original.Width) w = original.Width - x;
            if (y + h > original.Height) h = original.Height - y;

            // Draw overlay on original image instead of cropping
            using var preview = new Bitmap(original);
            using (var g = Graphics.FromImage(preview))
            {
                // Draw semi-transparent overlay outside crop area to highlight crop
                using (var brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                {
                    // Top
                    g.FillRectangle(brush, 0, 0, original.Width, y);
                    // Bottom
                    g.FillRectangle(brush, 0, y + h, original.Width, original.Height - (y + h));
                    // Left
                    g.FillRectangle(brush, 0, y, x, h);
                    // Right
                    g.FillRectangle(brush, x + w, y, original.Width - (x + w), h);
                }

                // Draw border
                using (var pen = new Pen(Color.Red, 2))
                {
                    g.DrawRectangle(pen, x, y, w, h);
                }
            }

            using var ms = new MemoryStream();
            preview.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        });
    }

    public Task<string> GenerateAndSaveSpriteSheetAsync(List<string> filePaths, SpriteSheetConfig config, string outputPath)
    {
        return Task.Run(() =>
        {
            if (filePaths == null || filePaths.Count == 0)
                throw new ArgumentException("No images to process");

            var crop = config.CropSettings.GetCurrentCrop();
            int frameWidth = crop.Width;
            int frameHeight = crop.Height;

            // Determine frame size if not specified
            if (frameWidth <= 0 || frameHeight <= 0)
            {
                using var firstFrame = Image.FromFile(filePaths[0]);
                if (frameWidth <= 0) frameWidth = firstFrame.Width;
                if (frameHeight <= 0) frameHeight = firstFrame.Height;
            }

            // Calculate sheet size
            int cols = config.Columns;
            int rows = config.Rows;
            if (rows <= 0) rows = (int)Math.Ceiling((double)filePaths.Count / cols);

            int totalWidth = (frameWidth + config.Spacing) * cols + config.Padding * 2 - config.Spacing; // Subtract last spacing? Usually padding is around the whole sheet.
            // Let's assume Padding is around the border, Spacing is between frames.
            totalWidth = config.Padding * 2 + cols * frameWidth + (cols - 1) * config.Spacing;
            int totalHeight = config.Padding * 2 + rows * frameHeight + (rows - 1) * config.Spacing;

            using var sheet = new Bitmap(totalWidth, totalHeight);
            using (var g = Graphics.FromImage(sheet))
            {
                g.Clear(Color.Transparent);

                for (int i = 0; i < filePaths.Count; i++)
                {
                    if (i >= cols * rows) break; // Limit to grid size

                    int col = i % cols;
                    int row = i / cols;

                    int x = config.Padding + col * (frameWidth + config.Spacing);
                    int y = config.Padding + row * (frameHeight + config.Spacing);

                    using var frame = Image.FromFile(filePaths[i]);
                    
                    // Apply crop to source
                    int srcX = Math.Max(0, crop.X);
                    int srcY = Math.Max(0, crop.Y);
                    int srcW = Math.Min(frameWidth, frame.Width - srcX);
                    int srcH = Math.Min(frameHeight, frame.Height - srcY);

                    if (srcW > 0 && srcH > 0)
                    {
                        g.DrawImage(frame, new Rectangle(x, y, srcW, srcH), new Rectangle(srcX, srcY, srcW, srcH), GraphicsUnit.Pixel);
                    }
                }
            }

            var format = config.OutputFormat switch
            {
                OutputFormat.Jpg => ImageFormat.Jpeg,
                OutputFormat.Bmp => ImageFormat.Bmp,
                _ => ImageFormat.Png
            };

            sheet.Save(outputPath, format);
            return outputPath;
        });
    }
}
