﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantMeasurements : ContentPage
    {
        public PlantMeasurements()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
        }

        private void Controls_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PlantControls());
        }
    }
}