namespace SpriteSheetCreator.Core.Models;

public struct CropRect
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public static CropRect Default => new CropRect(0, 0, 0, 0);

    public CropRect(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
