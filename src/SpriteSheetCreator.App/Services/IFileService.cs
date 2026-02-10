namespace SpriteSheetCreator.App.Services;

public interface IFileService
{
    string OpenFolder();
    string SaveFile(string defaultName, string filter);
    void ShowMessage(string message);
    void ShowError(string message);
}
