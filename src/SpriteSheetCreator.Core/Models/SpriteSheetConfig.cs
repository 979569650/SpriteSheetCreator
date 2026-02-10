namespace SpriteSheetCreator.Core.Models;

public class SpriteSheetConfig
{
    public int Columns { get; set; } = 8;
    public int Rows { get; set; } = 1;
    public int StartFrame { get; set; } = 0;
    
    public CropSettings CropSettings { get; set; } = new CropSettings();
    
    public int Padding { get; set; } = 0;
    public int Spacing { get; set; } = 0;
    
    public int FrameRate { get; set; } = 12;
    public bool IsLooping { get; set; } = true;
    
    public OutputFormat OutputFormat { get; set; } = OutputFormat.Png;
}
