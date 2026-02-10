using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpriteSheetCreator.Core.Models;
using SpriteSheetCreator.Core.Services;

namespace SpriteSheetCreator.App.ViewModels;

/// <summary>
/// 控制面板视图模型
/// </summary>
public partial class ControlsViewModel : ObservableObject
{
    private readonly PreviewViewModel _previewViewModel;
    private readonly IImageProcessor _imageProcessor;

    [ObservableProperty]
    private SpriteSheetConfig _config;

    private int _originalWidth;
    private int _originalHeight;

    private int _marginTop;
    private int _marginBottom;
    private int _marginLeft;
    private int _marginRight;

    [ObservableProperty]
    private int _croppedWidth;

    [ObservableProperty]
    private int _croppedHeight;

    public int MarginTop
    {
        get => _marginTop;
        set
        {
            if (SetProperty(ref _marginTop, value))
            {
                UpdateCropFromMargins();
            }
        }
    }

    public int MarginBottom
    {
        get => _marginBottom;
        set
        {
            if (SetProperty(ref _marginBottom, value))
            {
                UpdateCropFromMargins();
            }
        }
    }

    public int MarginLeft
    {
        get => _marginLeft;
        set
        {
            if (SetProperty(ref _marginLeft, value))
            {
                UpdateCropFromMargins();
            }
        }
    }

    public int MarginRight
    {
        get => _marginRight;
        set
        {
            if (SetProperty(ref _marginRight, value))
            {
                UpdateCropFromMargins();
            }
        }
    }

    /// <summary>
    /// 配置变化事件
    /// </summary>
    public event EventHandler? ConfigurationChanged;

    public ControlsViewModel(
        PreviewViewModel previewViewModel,
        IImageProcessor imageProcessor)
    {
        _previewViewModel = previewViewModel;
        _imageProcessor = imageProcessor;
        _config = new SpriteSheetConfig();
        // Initialize with 0s but UI will update when file loads
        UpdateCropFromConfig();
    }

    public void SetDefaultCrop(int width, int height)
    {
        _originalWidth = width;
        _originalHeight = height;
        
        // Default crop is full image (0 margins)
        MarginTop = 0;
        MarginBottom = 0;
        MarginLeft = 0;
        MarginRight = 0;
        
        // Force update to ensure config is synced
        UpdateCropFromMargins();
    }

    private void UpdateCropFromMargins()
    {
        if (_originalWidth <= 0 || _originalHeight <= 0) return;

        int x = MarginLeft;
        int y = MarginTop;
        int w = Math.Max(1, _originalWidth - MarginLeft - MarginRight);
        int h = Math.Max(1, _originalHeight - MarginTop - MarginBottom);

        CroppedWidth = w;
        CroppedHeight = h;

        var crop = new CropRect(x, y, w, h);
        Config.CropSettings.SetCurrentCrop(crop);
        _previewViewModel.UpdateCrop(crop);
        NotifyConfigurationChanged();
    }

    #region 精灵图布局

    public int Columns
    {
        get => Config.Columns;
        set
        {
            if (Config.Columns != value)
            {
                Config.Columns = value;
                OnPropertyChanged(nameof(Columns));
                NotifyConfigurationChanged();
            }
        }
    }

    public int Rows
    {
        get => Config.Rows;
        set
        {
            if (Config.Rows != value)
            {
                Config.Rows = value;
                OnPropertyChanged(nameof(Rows));
                NotifyConfigurationChanged();
            }
        }
    }

    public int StartFrame
    {
        get => Config.StartFrame;
        set
        {
            if (Config.StartFrame != value)
            {
                Config.StartFrame = value;
                OnPropertyChanged(nameof(StartFrame));
                NotifyConfigurationChanged();
            }
        }
    }

    #endregion

    #region 裁剪设置

    // Direction property removed as requested by user - crop is global now
    // But keeping region structure for consistency

    #endregion

    #region 间距和内边距

    public int Padding
    {
        get => Config.Padding;
        set
        {
            if (Config.Padding != value)
            {
                Config.Padding = value;
                OnPropertyChanged(nameof(Padding));
                NotifyConfigurationChanged();
            }
        }
    }

    public int Spacing
    {
        get => Config.Spacing;
        set
        {
            if (Config.Spacing != value)
            {
                Config.Spacing = value;
                OnPropertyChanged(nameof(Spacing));
                NotifyConfigurationChanged();
            }
        }
    }

    #endregion

    #region 动画设置

    public int FrameRate
    {
        get => Config.FrameRate;
        set
        {
            if (Config.FrameRate != value)
            {
                Config.FrameRate = value;
                OnPropertyChanged(nameof(FrameRate));
                _previewViewModel.SetFrameRate(value);
            }
        }
    }

    public bool IsLooping
    {
        get => Config.IsLooping;
        set
        {
            if (Config.IsLooping != value)
            {
                Config.IsLooping = value;
                OnPropertyChanged(nameof(IsLooping));
                _previewViewModel.SetLooping(value);
            }
        }
    }

    #endregion

    #region 导出设置

    public OutputFormat OutputFormat
    {
        get => Config.OutputFormat;
        set
        {
            if (Config.OutputFormat != value)
            {
                Config.OutputFormat = value;
                OnPropertyChanged(nameof(OutputFormat));
            }
        }
    }

    [ObservableProperty]
    private bool _includeMetadata = true;

    #endregion

    /// <summary>
    /// 通知配置已更改
    /// </summary>
    private void NotifyConfigurationChanged()
    {
        ConfigurationChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 从配置更新裁剪参数
    /// </summary>
    private void UpdateCropFromConfig()
    {
        var crop = Config.CropSettings.GetCurrentCrop();
        
        // Convert Rect back to Margins if possible
        // Requires original size, if not set, assume 0 margins or just set what we can
        if (_originalWidth > 0 && _originalHeight > 0)
        {
            MarginLeft = crop.X;
            MarginTop = crop.Y;
            MarginRight = Math.Max(0, _originalWidth - crop.X - crop.Width);
            MarginBottom = Math.Max(0, _originalHeight - crop.Y - crop.Height);
        }
        else
        {
            // Fallback if image not loaded yet
            _marginLeft = crop.X;
            _marginTop = crop.Y;
            // Can't calculate Right/Bottom without Width/Height
            // Just keep them as is or reset
        }

        _previewViewModel.UpdateCrop(crop);
    }

    /// <summary>
    /// 重置裁剪设置
    /// </summary>
    [RelayCommand]
    private void ResetCrop()
    {
        // Reset to full image (0 margins)
        MarginTop = 0;
        MarginBottom = 0;
        MarginLeft = 0;
        MarginRight = 0;
        
        // This will trigger UpdateCropFromMargins via property setters
    }

    /// <summary>
    /// 重置所有设置为默认值
    /// </summary>
    [RelayCommand]
    private void ResetDefaults()
    {
        Config = new SpriteSheetConfig();
        
        // Reset crop to full image
        SetDefaultCrop(_originalWidth, _originalHeight);

        OnPropertyChanged(nameof(Config));
        OnPropertyChanged(nameof(Columns));
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(StartFrame));
        // Padding and Spacing removed from UI/logic as requested
        Config.Padding = 0;
        Config.Spacing = 0;
        OnPropertyChanged(nameof(Padding));
        OnPropertyChanged(nameof(Spacing));
        
        OnPropertyChanged(nameof(FrameRate));
        OnPropertyChanged(nameof(IsLooping));
        OnPropertyChanged(nameof(OutputFormat));
        NotifyConfigurationChanged();
    }

    /// <summary>
    /// 设置配置
    /// </summary>
    public void SetConfig(SpriteSheetConfig config)
    {
        Config = config;
        UpdateCropFromConfig();

        OnPropertyChanged(nameof(Columns));
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(StartFrame));
        OnPropertyChanged(nameof(Padding));
        OnPropertyChanged(nameof(Spacing));
        OnPropertyChanged(nameof(FrameRate));
        OnPropertyChanged(nameof(IsLooping));
        OnPropertyChanged(nameof(OutputFormat));
    }
}
