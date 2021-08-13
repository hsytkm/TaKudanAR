#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TaKudanAR.Models;
using TaKudanAR.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaKudanAR.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}
