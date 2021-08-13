using System.ComponentModel;
using TaKudanAR.ViewModels;
using Xamarin.Forms;

namespace TaKudanAR.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}