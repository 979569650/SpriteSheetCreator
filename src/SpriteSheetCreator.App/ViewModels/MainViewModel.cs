using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpriteSheetCreator.Core.Models;
using SpriteSheetCreator.Core.Services;
using SpriteSheetCreator.App.Services;

namespace SpriteSheetCreator.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IFileService _fileService;

    [ObservableProperty]
    private ControlsViewModel _controls;

    [ObservableProperty]
    private PreviewViewModel _preview;

    [ObservableProperty]
    private string _currentFolderPath = "No folder selected";

    [ObservableProperty]
    private bool _isBusy;

    private List<string> _loadedFiles = new();

    public MainViewModel(
        IImageProcessor imageProcessor,
        IFileService fileService,
        ControlsViewModel controls,
        PreviewViewModel preview)
    {
        _imageProcessor = imageProcessor;
        _fileService = fileService;
        _controls = controls;
        _preview = preview;
    }

    [RelayCommand]
    private async Task OpenFolder()
    {
        var folder = _fileService.OpenFolder();
        if (string.IsNullOrEmpty(folder)) return;

        CurrentFolderPath = folder;
        IsBusy = true;
        try
        {
            _loadedFiles = await _imageProcessor.LoadImageFilesAsync(folder);
            _preview.LoadFrames(_loadedFiles);

            if (_loadedFiles.Count > 0)
            {
                using var img = System.Drawing.Image.FromFile(_loadedFiles[0]);
                _controls.SetDefaultCrop(img.Width, img.Height);

                // Auto-calculate rows and columns
                int count = _loadedFiles.Count;
                int cols = (int)Math.Ceiling(Math.Sqrt(count));
                int rows = (int)Math.Ceiling((double)count / cols);
                _controls.Columns = cols;
                _controls.Rows = rows;
            }
        }
        catch (Exception ex)
        {
            _fileService.ShowError($"Failed to load images: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Export()
    {
        if (_loadedFiles.Count == 0)
        {
            _fileService.ShowMessage("Please open a folder with images first.");
            return;
        }

        var filter = _controls.OutputFormat == OutputFormat.Png ? "PNG Image|*.png" : "JPEG Image|*.jpg";
        var path = _fileService.SaveFile("sprite_sheet", filter);
        if (string.IsNullOrEmpty(path)) return;

        IsBusy = true;
        try
        {
            await _imageProcessor.GenerateAndSaveSpriteSheetAsync(_loadedFiles, _controls.Config, path);
            _fileService.ShowMessage($"Exported successfully to {path}");
        }
        catch (Exception ex)
        {
            _fileService.ShowError($"Error exporting: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
