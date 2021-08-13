#nullable disable
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TaKudanAR.Models;
using Xamarin.Forms;

namespace TaKudanAR.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        public string Id { get; set; }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
        private string _text;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        private string _description;

        public string ItemId
        {
            get => _itemId;
            set
            {
                _itemId = value;
                LoadItemId(value);
            }
        }
        private string _itemId;

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Id = item.Id;
                Text = item.Text;
                Description = item.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
