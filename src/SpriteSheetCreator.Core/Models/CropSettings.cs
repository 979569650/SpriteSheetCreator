using System.Collections.Generic;

namespace SpriteSheetCreator.Core.Models;

public class CropSettings
{
    private Dictionary<Direction, CropRect> _crops = new();

    public Direction CurrentDirection { get; set; } = Direction.Down;

    public CropSettings()
    {
        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            _crops[dir] = CropRect.Default;
        }
    }

    public CropRect GetCurrentCrop()
    {
        // Ignore direction, return global crop
        if (_crops.TryGetValue(Direction.Down, out var crop))
        {
            return crop;
        }
        return CropRect.Default;
    }

    public void SetCurrentCrop(CropRect crop)
    {
        // Set same crop for all directions
        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            _crops[dir] = crop;
        }
    }
}
