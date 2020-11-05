using BookOrganizer2.Domain.Shared;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class BaseWrapper<T, TId> : NotifyDataErrorInfo
        where T : IIdentifiable<TId>
        where TId : ValueObject
    {
        protected BaseWrapper(T model)
        {
            Model = model;
        }

        public TId Id => Model.Id;

        public T Model { get; }

        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            string errorMessage = null;
            try
            {
                typeof(T).GetMethod($"Set{propertyName}")?.Invoke(Model, new object[] {value});
            }
            catch (Exception ex)
            {
                errorMessage = ex.InnerException?.Message;
                typeof(T).GetProperty(propertyName)?.SetValue(Model, value);
            }

            OnPropertyChanged(propertyName);
            ValidatePropertyInternal(propertyName, value, errorMessage);
        }

        protected virtual TValue GetValue<TValue>([CallerMemberName] [NotNull] string propertyName = null)
            => (TValue) typeof(T).GetProperty(propertyName!)?.GetValue(Model);

        private void SetErrorElement(string propertyName, string error, object value)
        {
            AddError(propertyName, error);
        }

        private void ValidatePropertyInternal(string propertyName, object currentValue, string error)
        {
            ClearErrors(propertyName);
            if (error is not null)
            {
                SetErrorElement(propertyName, error, currentValue);
            }
        }
    }
}
