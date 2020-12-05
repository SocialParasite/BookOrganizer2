using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.Services;
using System;

namespace BookOrganizer2.UI.Wpf.Wrappers
{
    public class SeriesWrapper : BaseWrapper<Series, SeriesId>
    {
        public SeriesWrapper(Series model) : base(model) { }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string PicturePath
        {
            get => GetValue<string>();
            set => SetValue(DomainHelpers.SetPicturePath(value, "SeriesPics"));
        }

        public string Description
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

    }
}
