using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Plugin.CryptoUI.Properties;
using SAL.Windows;

namespace Plugin.CryptoUI
{
	public partial class PanelCryptoUI : UserControl
	{
		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		private IWindow Window => (IWindow)base.Parent;

		private Data.ICertRequest[] _moduleTypes;

		private Data.ICertRequest[] ModuleTypes
		{
			get
			{
				if(this._moduleTypes == null)
				{
					this._moduleTypes = new Data.ICertRequest[]{
						new Data.CertificateRequest(this.Plugin),
						new Data.CertificatePemToPkcs12Request(this.Plugin),
						new Data.CertificateToCsrPemRequest(this.Plugin),
					};
				}
				return this._moduleTypes;
			}
		}

		public PanelCryptoUI()
			=> InitializeComponent();

		protected override void OnCreateControl()
		{
			this.Window.Caption = "Crypto Helpers";
			this.Window.SetTabPicture(Resources.icoCertificate);

			foreach(Data.ICertRequest module in this.ModuleTypes)
			{
				DisplayNameAttribute attr = TypeExtender.GetCustomAttributes<DisplayNameAttribute>(module.GetType());
				lbModules.Items.Add(attr == null ? "<Unknown>" : attr.DisplayName);
			}

			base.OnCreateControl();
		}

		private void lbModules_SelectedIndexChanged(Object sender, EventArgs e)
		{
			Int32 moduleIndex = lbModules.SelectedIndex;
			Data.ICertRequest module = this.ModuleTypes[moduleIndex];
			DescriptionAttribute attr = TypeExtender.GetCustomAttributes<DescriptionAttribute>(module.GetType());

			txtDescription.Text = attr == null ? String.Empty : attr.Description;
			pgMain.SelectedObject = module;
			error.SetError(bnInvoke, String.Empty);
		}

		private void bnInvoke_Click(Object sender, EventArgs e)
		{
			error.SetError(bnInvoke, String.Empty);
			Data.ICertRequest request = (Data.ICertRequest)pgMain.SelectedObject;
			try
			{
				request.Invoke();
			} catch(ArgumentException exc)
			{
				error.SetError(bnInvoke, exc.Message);
			}
		}
	}
}