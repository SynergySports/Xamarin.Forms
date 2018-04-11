using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.Issues
{
    [Preserve(AllMembers = true)]
    [Issue(IssueTracker.Github, 2399, "Label Renderer Dispose never called")]
    public class Issue2399 : TestNavigationPage
    {
        protected override async void Init()
        {
            var newPage = new ContentPage
            {
                Content = new Button
                {
                    Text = "Show New ListView",
                    Command = new Command(async o => await Navigation.PushAsync(new ListPage())),
                },
            };

            newPage.Appearing += OnAppearing; 
            await Navigation.PushAsync(newPage); 
             
        }

        async void OnAppearing(object sender, EventArgs args)
        {
            (sender as ContentPage).Appearing -= OnAppearing;
            await Navigation.PushAsync(new ListPage());
            await Navigation.PopAsync(); 
            GC.Collect();
            GC.WaitForPendingFinalizers();
        } 

        public class ListPage : ContentPage
        {
            public ListPage()
            {
                Content = new ListView
                {
                    ItemTemplate = new DataTemplate(typeof(Cell)),
                    ItemsSource = new List<string> { "A" },
                };
            }
        }

        class Cell : ViewCell
        {
            string guid = Guid.NewGuid().ToString();

            public Cell()
            {
                Console.WriteLine("Constructor " + guid);
                View = new StackLayout
                {
                    Children = {
                new ContentView
                {
                    Content = new Label { Text = guid },
                }
            }
                };
            }

            ~Cell()
            {
                Console.WriteLine("Destructor " + guid);
            }
        }
    }
}
