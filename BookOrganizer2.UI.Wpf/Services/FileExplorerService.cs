using System;
using System.IO;
using System.Reflection;
using BookOrganizer2.UI.BOThemes.DialogServiceManager;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.Enums;
using BookOrganizer2.UI.BOThemes.DialogServiceManager.ViewModels;
using Microsoft.Win32;
using NetVips;


namespace BookOrganizer2.UI.Wpf.Services
{
    public static class FileExplorerService
    {
        public static string BrowsePicture()
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select an image as an author picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png"
            };

            return (op.ShowDialog() == true) ? op.FileName : null;
        }

        public static string GetImagePath()
            => $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)?.Substring(6)}\\placeholder.png";

        public static void CreateThumbnail(string path, IDialogService dialogService = null)
        {
            Image image = Image.Thumbnail(path, 75, 75);

            var newPath = "";
            int index = path.IndexOf(".", StringComparison.InvariantCulture);
            bool overwrite = false;

            if (index > 0)
                newPath = path.Substring(0, index) + "_thumb.jpg";

            image.WriteToFile("test.jpg");
            if (File.Exists(newPath))
            {
                var dialog = new OkCancelViewModel("File already exists.", "File already exists. Would you like to replace the existing file?");
                if (dialogService?.OpenDialog(dialog) == DialogResult.No)
                    return;
                
                overwrite = true;
            }

            File.Move("test.jpg", newPath, overwrite);
        }
    }
}
