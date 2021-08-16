using System;
using System.Collections.Generic;
using TaKudanAR.ViewModels;
using TaKudanAR.Views;
using Xamarin.Forms;

namespace TaKudanAR
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
