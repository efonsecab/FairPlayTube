using System;
using Android.App;
using Android.Runtime;
using Microsoft.Maui;

namespace FairPlayTube.MAUI
{
	[Application]
	public class MainApplication : MauiApplication<Startup>
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership)
			: base(handle, ownership)
		{
		}
	}
}