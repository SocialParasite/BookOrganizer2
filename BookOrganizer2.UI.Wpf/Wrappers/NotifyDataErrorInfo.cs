using BookOrganizer2.UI.Wpf.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class NotifyDataErrorInfo : ViewModelBase, INotifyDataErrorInfo
    {
        public bool HasErrors => Errors?.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            return Errors.ContainsKey(propertyName) ? Errors[propertyName] : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void AddError(string propertyName, string error)
        {
            if (!Errors.ContainsKey(propertyName))
            {
                Errors[propertyName] = new List<string>();
            }
            if (!Errors[propertyName].Contains(error))
            {
                Errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        public void ClearErrors(string propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
