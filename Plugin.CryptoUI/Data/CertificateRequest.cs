using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using Plugin.CryptoUI.UI;

namespace Plugin.CryptoUI.Data
{
	[DisplayName("Generate Certificate")]
	[Description("Create self-signed certificate or certificate with private key")]
	internal class CertificateRequest : ICertRequest
	{
		private class Constants
		{
			public const String Algorithm = "SHA256WITHRSA";
		}
		private readonly PluginWindows _plugin;

		[Category("Description")]
		[Description("CN={commonName}, C={country}, O={organization}, L={location}")]
		public String Subject { get; set; }

		[Category("Description")]
		[DisplayName("Valid from")]
		[Description("Certificate valid from date")]
		public DateTime From { get; set; } = DateTime.Today;

		[Category("Description")]
		[DisplayName("Valid to")]
		[Description("Certificate valid to date")]
		public DateTime To { get; set; } = DateTime.Today.AddYears(1);

		[Category("Security")]
		[Description("Certificate password. If password is not specified, then sef signed certificate will be generated")]
		[PasswordPropertyText(true)]
		public String Password { get; set; }

		[Category("Security")]
		[Description("Cerficate encryption strength")]
		[DefaultValue(typeof(UInt32), "2048")]
		public UInt32 Strength { get; set; } = 2048;

		[Category("Security")]
		[Description("Encryption algorihtm")]
		[DefaultValue(Constants.Algorithm)]
		[Editor(typeof(AlgorithmEditor), typeof(UITypeEditor))]
		public String Algorithm { get; set; } = Constants.Algorithm;

		public CertificateRequest(PluginWindows plugin)
			=> this._plugin = plugin;

		public void Invoke()
		{
			Byte[] payload = this._plugin.GenerateCertificate(
				this.Subject,
				this.Password,
				(Int32)this.Strength,
				this.Algorithm,
				this.From,
				this.To);

			String defaultExt;
			String description;
			if(String.IsNullOrEmpty(this.Password))
			{
				defaultExt = "crt";
				description = "Certificate file (*.crt)|*.crt";
			} else
			{
				defaultExt = "pfx";
				description = "Personal Information Exchange file (*.pfx)|*.pfx";
			}

			using(SaveFileDialog dlg = new SaveFileDialog() { OverwritePrompt = true, AddExtension = true, DefaultExt = defaultExt, Filter = description })
				if(dlg.ShowDialog() == DialogResult.OK)
					File.WriteAllBytes(dlg.FileName, payload);
		}
	}
}