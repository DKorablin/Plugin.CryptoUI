using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Plugin.CryptoUI.Properties;
using SAL.Windows;

namespace Plugin.CryptoUI
{
	public partial class PanelCryptoUI : UserControl
	{
		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		private IWindow Window => (IWindow)base.Parent;

		private Data.ICertificateUI[] _moduleTypes;

		private Data.ICertificateUI[] ModuleTypes
		{
			get
			{
				if(this._moduleTypes == null)
				{
					var interfaceType = typeof(Data.ICertificateUI);
					Type[] types = Assembly.GetAssembly(this.GetType()).GetTypes();

					List<Data.ICertificateUI> modules = new List<Data.ICertificateUI>();
					foreach(var type in types)
						if(type.IsInterface == false && Array.Find(type.GetInterfaces(), i => i == interfaceType) != null)
						{
							ConstructorInfo ctor = type.GetConstructor(new Type[] { });
							Object[] ctorArgs = new Object[] { };
							if(ctor == null)
							{
								ctor = type.GetConstructor(new Type[] { typeof(PluginWindows) });
								ctorArgs = new Object[] { this.Plugin };
							}

							if(ctor != null)
								modules.Add((Data.ICertificateUI)ctor.Invoke(ctorArgs));
							else
								this.Plugin.Trace.TraceInformation("Can't create UI control of type {0}", type);
						}

					this._moduleTypes = modules.ToArray();
				}
				return this._moduleTypes;
			}
		}

		public PanelCryptoUI()
			=> this.InitializeComponent();

		protected override void OnCreateControl()
		{
			this.Window.Caption = "Crypto Helpers";
			this.Window.SetTabPicture(Resources.icoCertificate);

			foreach(Data.ICertificateUI module in this.ModuleTypes)
			{
				DisplayNameAttribute attr = TypeExtender.GetCustomAttributes<DisplayNameAttribute>(module.GetType());
				lbModules.Items.Add(attr == null ? "<Unknown>" : attr.DisplayName);
			}

			base.OnCreateControl();
		}

		private void lbModules_SelectedIndexChanged(Object sender, EventArgs e)
		{
			Int32 moduleIndex = lbModules.SelectedIndex;
			Data.ICertificateUI module = this.ModuleTypes[moduleIndex];
			DescriptionAttribute attr = TypeExtender.GetCustomAttributes<DescriptionAttribute>(module.GetType());

			txtDescription.Text = attr == null ? String.Empty : attr.Description;
			pgMain.SelectedObject = module;
			error.SetError(bnInvoke, String.Empty);
		}

		private void bnInvoke_Click(Object sender, EventArgs e)
		{
			error.SetError(bnInvoke, String.Empty);
			Data.ICertificateUI request = (Data.ICertificateUI)pgMain.SelectedObject;
			try
			{
				request.Invoke();
			} catch(ArgumentException exc)
			{
				error.SetError(bnInvoke, exc.Message);
			}catch(Exception exc)
			{
				error.SetError(bnInvoke, exc.Message);
				throw;
			}
		}
	}
}