﻿using BookOrganizer2.UI.BOThemes.DialogServiceManager.Windows;
using System.Windows;
using System.Windows.Media.Effects;

namespace BookOrganizer2.UI.BOThemes.DialogServiceManager
{
    public class DialogService : IDialogService
    {
        public T OpenDialog<T>(BaseDialog<T> viewModel)
        {
            var window = new DialogWindow {Owner = Application.Current.MainWindow};
            if (Application.Current.MainWindow != null)
            {
                window.Left = Application.Current.MainWindow.Left;
                window.Width = Application.Current.MainWindow.ActualWidth - 16;
                window.Height = Application.Current.MainWindow.ActualHeight / 3;
                window.DataContext = viewModel;

                Application.Current.MainWindow.Effect = new BlurEffect();

                window.ShowDialog();

                Application.Current.MainWindow.Effect = null;
            }

            return viewModel.DialogResult;
        }
    }
}
