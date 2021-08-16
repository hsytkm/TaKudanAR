using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TaKudanAR.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public override string Title => "About";

        public ICommand OpenWebCommand => _openWebCommand ??= new Command(async () =>
            await Browser.OpenAsync("https://github.com/hsytkm/TaKudanAR"));

        private ICommand _openWebCommand = default!;

    }
}
