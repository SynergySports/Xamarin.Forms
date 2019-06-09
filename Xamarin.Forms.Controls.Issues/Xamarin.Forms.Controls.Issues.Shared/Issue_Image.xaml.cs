using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9999, "[Bug] [Forms 4.0] [macOS] Crash when Image removed from visual tree", PlatformAffected.macOS)]
	public partial class IssueImage : ContentPage
	{
		public IssueImage()
		{
#if APP
			InitializeComponent();
			TestImage.Source = ImageSource.FromUri(new Uri("https://cdn.pixabay.com/photo/2013/07/12/17/47/test-pattern-152459_960_720.png"));
			RemoveButton.Clicked += delegate
			{
				// remove the Image
				ImageHolder.Content = null;
			};
#endif
		}
	}
}