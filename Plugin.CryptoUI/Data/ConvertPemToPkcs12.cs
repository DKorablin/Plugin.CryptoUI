using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using Plugin.CryptoUI.UI;

namespace Plugin.CryptoUI.Data
{
	[DisplayName("Convert CRT/PEM to PKCS#12")]
	[Description("Convert certificate (crt) & privacy enhanced certificate (pem) to PKCS#12 (pfx)")]
	internal class ConvertPemToPkcs12 : ICertificateUI
	{
		private readonly PluginWindows _plugin;

		[Category("Files")]
		[DisplayName("Certificate path")]
		[Description("Select certificate file (*.crt) to convert")]
		[Editor(typeof(CertFileOpener), typeof(UITypeEditor))]
		public String CertificatePath { get; set; }

		[Category("Files")]
		[DisplayName("Pem path")]
		[Description("Select privacy enhanced certificate file (*.pem) to convert")]
		[Editor(typeof(PemFileOpener), typeof(UITypeEditor))]
		public String PemFilePath { get; set; }

		[Category("Security")]
		[Description("Password for the private key")]
		[PasswordPropertyText(true)]
		public String Password { get; set; }

		public ConvertPemToPkcs12(PluginWindows plugin)
			=> this._plugin = plugin;

		public void Invoke()
		{
			if(String.IsNullOrEmpty(this.CertificatePath) || !File.Exists(this.CertificatePath))
				throw new ArgumentException("Certificate file not found");
			else if(String.IsNullOrEmpty(this.PemFilePath) || !File.Exists(this.PemFilePath))
				throw new ArgumentException("Privacy Enhanced Certificate File not found");
			else if(String.IsNullOrEmpty(this.Password))
				throw new ArgumentException("Password not specified");
			else
			{
				Byte[] payload = this._plugin.ConvertCertPemToPkcs12(File.ReadAllBytes(this.CertificatePath), File.ReadAllBytes(this.PemFilePath), this.Password);

				using(SaveFileDialog dlg = new SaveFileDialog() { OverwritePrompt = true, AddExtension = true, FileName = Path.GetFileNameWithoutExtension(this.CertificatePath), DefaultExt = "pfx", Filter = "Personal Information Exchange file (*.pfx)|*.pfx", })
					if(dlg.ShowDialog() == DialogResult.OK)
						File.WriteAllBytes(dlg.FileName, payload);
			}
		}
	}
}