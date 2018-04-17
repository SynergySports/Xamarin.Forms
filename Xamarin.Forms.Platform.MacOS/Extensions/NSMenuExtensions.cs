using System;
using System.Linq;
using AppKit;

namespace Xamarin.Forms.Platform.macOS.Extensions
{
  public static class NSMenuExtensions
	{
    public static NSMenu ToNSMenu(this Menu menus, NSMenu nsMenu = null, Action<MenuItem, NSMenuItem> propertiesBinder = null, Action<MenuItem, NSMenuItem> onActivated = null)
		{
			if (nsMenu == null)
				nsMenu = new NSMenu(menus.Text ?? "");

      if(propertiesBinder == null)
      {
        propertiesBinder = BindProperties;
      }
     
      foreach (var menu in menus)
			{
				var menuItem = new NSMenuItem(menu.Text ?? "");
				var subMenu = new NSMenu(menu.Text ?? "");
				menuItem.Submenu = subMenu;
				foreach (var item in menu.Items)
				{
          var subMenuItem = item.ToNSMenuItem(onActivated: onActivated);         
					GetAccelerators(subMenuItem, item);
					subMenu.AddItem(subMenuItem);
          propertiesBinder(item, subMenuItem);
				}
				nsMenu.AddItem(menuItem);
				menu.ToNSMenu(subMenu);
			}
			return nsMenu;
		}


    public static NSMenuItem ToNSMenuItem(this MenuItem menuItem, int i = -1, Action<MenuItem, NSMenuItem> onActivated = null)
		{
			var nsMenuItem = new NSMenuItem(menuItem.Text ?? "");
			if (i != -1)
				nsMenuItem.Tag = i;

      var onActivateAction = onActivated;
      if(onActivateAction == null)
      {
        onActivateAction = (arg1, arg2) => arg1.Activate();
      }
      else
      {
        onActivateAction = (arg1, arg2) => {
          onActivated(arg1, arg2);
          arg1.Activate(); };
      }
			nsMenuItem.Enabled = menuItem.IsEnabled;
      nsMenuItem.Activated += (sender, e) => onActivateAction(menuItem, nsMenuItem);
			if (!string.IsNullOrEmpty(menuItem.Icon))
				nsMenuItem.Image = new NSImage(menuItem.Icon);

			return nsMenuItem;
		}

    public static void BindProperties(MenuItem item, NSMenuItem nativeItem) => 
      item.PropertyChanged += (sender, e) => (sender as MenuItem)?.UpdateNSMenuItem(nativeItem, new string[] { e.PropertyName });


		public static void UpdateNSMenuItem(this MenuItem item, NSMenuItem menuItem, string[] properties)
		{
			foreach (var property in properties)
			{
				if (property.Equals(nameof(MenuItem.Text)))
				{
					menuItem.Title = item.Text;
				}
				if (property.Equals(nameof(MenuItem.IsEnabled)))
				{
					menuItem.Enabled = item.IsEnabled;
				}
				if (property.Equals(nameof(MenuItem.Icon)))
				{
					if (!string.IsNullOrEmpty(item.Icon))
						menuItem.Image = new NSImage(item.Icon);
					else
						menuItem.Image = null;
				}
			}
		}

		static void GetAccelerators(NSMenuItem nsMenuItem, MenuItem item)
		{
			var accelerator = MenuItem.GetAccelerator(item);

			if (accelerator == null)
				return;

			bool hasModifierMask = accelerator.Modifiers?.Count() > 1;

			if (hasModifierMask)
			{
				nsMenuItem.KeyEquivalentModifierMask = 0;

				for (int i = 0; i < accelerator.Modifiers.Count(); i++)
				{
					var modifierMask = accelerator.Modifiers.ElementAt(i).ToLower();
					switch (modifierMask)
					{
						case "ctrl":
							nsMenuItem.KeyEquivalentModifierMask = nsMenuItem.KeyEquivalentModifierMask | NSEventModifierMask.ControlKeyMask;
							break;
						case "cmd":
							nsMenuItem.KeyEquivalentModifierMask = nsMenuItem.KeyEquivalentModifierMask | NSEventModifierMask.CommandKeyMask;
							break;
						case "alt":
							nsMenuItem.KeyEquivalentModifierMask = nsMenuItem.KeyEquivalentModifierMask | NSEventModifierMask.AlternateKeyMask;
							break;
						case "shift":
							nsMenuItem.KeyEquivalentModifierMask = nsMenuItem.KeyEquivalentModifierMask | NSEventModifierMask.ShiftKeyMask;
							break;
						case "fn":
							nsMenuItem.KeyEquivalentModifierMask = nsMenuItem.KeyEquivalentModifierMask | NSEventModifierMask.FunctionKeyMask;
							break;
					}
				}
			}
			nsMenuItem.KeyEquivalent = accelerator.Keys.FirstOrDefault();
		}
	}
}
