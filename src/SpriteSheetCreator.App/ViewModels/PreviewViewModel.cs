using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SpriteSheetCreator.Core.Models;
using SpriteSheetCreator.Core.Services;

namespace SpriteSheetCreator.App.ViewModels;

public partial class PreviewViewModel : ObservableObject
{
    private readonly IImageProcessor _imageProcessor;
    private DispatcherTimer _timer;
    private List<string> _framePaths = new();
    private int _currentFrameIndex = 0;
    private CropRect _currentCrop = CropRect.Default;

    private bool _isLooping = true;

    [ObservableProperty]
    private BitmapImage? _currentFrame;

    public PreviewViewModel(IImageProcessor imageProcessor)
    {
        _imageProcessor = imageProcessor;
        _timer = new DispatcherTimer();
        _timer.Tick += OnTimerTick;
        SetFrameRate(12);
        _timer.Start();
    }

    public void SetFrameRate(int fps)
    {
        if (fps <= 0) fps = 1;
        _timer.Interval = TimeSpan.FromSeconds(1.0 / fps);
    }

    public void SetLooping(bool isLooping)
    {
        _isLooping = isLooping;
    }

    public void LoadFrames(List<string> paths)
    {
        _framePaths = paths;
        _currentFrameIndex = 0;
        UpdatePreview();
    }

    public void UpdateCrop(CropRect crop)
    {
        _currentCrop = crop;
        UpdatePreview();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_framePaths.Count == 0) return;

        if (_currentFrameIndex < _framePaths.Count - 1)
        {
            _currentFrameIndex++;
        }
        else
        {
            if (_isLooping)
            {
                _currentFrameIndex = 0;
            }
            // else: stop at last frame
        }
        
        UpdatePreview();
    }

    private async void UpdatePreview()
    {
        if (_framePaths.Count == 0) return;
        
        // Safety check index
        if (_currentFrameIndex >= _framePaths.Count) _currentFrameIndex = 0;

        var path = _framePaths[_currentFrameIndex];
        try
        {
            var bytes = await _imageProcessor.GeneratePreviewFrameAsync(path, _currentCrop);
            if (bytes != null && bytes.Length > 0)
            {
                // Create BitmapImage on UI thread (usually async void is ok for event handlers/UI updates)
                // But we need to be careful with threads. 
                // Since this is called from Timer (UI thread) or LoadFrames (UI thread), it should be fine.
                // However, GeneratePreviewFrameAsync runs on Task.Run.
                
                var image = new BitmapImage();
                using (var mem = new MemoryStream(bytes))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                CurrentFrame = image;
            }
        }
        catch (Exception)
        {
            // Log or ignore
        }
    }
}
