using Microsoft.Win32;
using System.Windows;

namespace SpriteSheetCreator.App.Services;

public class FileService : IFileService
{
    public string OpenFolder()
    {
        var dialog = new OpenFolderDialog();
        if (dialog.ShowDialog() == true)
        {
            return dialog.FolderName;
        }
        return string.Empty;
    }

    public string SaveFile(string defaultName, string filter)
    {
        var dialog = new SaveFileDialog
        {
            FileName = defaultName,
            Filter = filter
        };
        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }
        return string.Empty;
    }

    public void ShowMessage(string message)
    {
        MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
