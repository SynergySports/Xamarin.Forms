﻿#if __ANDROID_28__
using Android.Content;
using Android.Views;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.Material.Android
{
	public class MaterialContextThemeWrapper : ContextThemeWrapper
	{
		public MaterialContextThemeWrapper(Context context) : base(context, Resource.Style.XamarinFormsMaterialTheme)
		{
		}


		public static Context Create(Context context)
		{
			if (context is MaterialContextThemeWrapper)
				return context;

			return new MaterialContextThemeWrapper(context);
		}
	}
}
#endif