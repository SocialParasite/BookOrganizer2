using BookOrganizer2.Domain.Shared;
using JetBrains.Annotations;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        protected virtual async Task SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            string errorMessage = string.Empty;
            try
            {
                typeof(T).GetMethod($"Set{propertyName}")?.Invoke(Model, new object[] { value });
            }
            catch (Exception ex)
            {
                errorMessage =ex.InnerException?.Message;
            }
            OnPropertyChanged(propertyName);
            await ValidatePropertyInternal(propertyName, value, errorMessage);
        }

        protected virtual TValue GetValue<TValue>([CallerMemberName] [NotNull] string propertyName = null)
            => (TValue)typeof(T).GetProperty(propertyName!)?.GetValue(Model);

        private async Task SetErrorElement(string propertyName, string error)
        {
            AddError(propertyName, error);
            
            await Task.Delay(5_000);
            ClearErrors(propertyName);
        }
        private async Task ValidatePropertyInternal(string propertyName, object currentValue, string error)
        {
            ClearErrors(propertyName);

            await SetErrorElement(propertyName, error);
        }
    }
}
