using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Plugin.CryptoUI.UI
{
	internal class CertFileOpener : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.Modal;

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			if(provider != null)
			{
				IWindowsFormsEditorService windowsFormsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(windowsFormsEditorService != null)
				{
					using(OpenFileDialog dlg = new OpenFileDialog() { CheckFileExists = true, DefaultExt = "crt", Filter = "Certificate file (*.crt)|*.crt|All files (*.*)|*.*", Title = "Open Certificate File" })
					{
						if(value != null)
							dlg.FileName = (String)value;
						if(dlg.ShowDialog() == DialogResult.OK)
							return value = dlg.FileName;
					}
				}
			}
			return value;
		}
	}
}