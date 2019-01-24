﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AiosKingdom.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MarketPage : ContentView
    {
		public MarketPage()
		{
			InitializeComponent();

            BindingContext = new ViewModels.MarketPageViewModel(Navigation);
		}
	}
}