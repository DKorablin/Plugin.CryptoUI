using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Plugin.CryptoUI.UI;

namespace Plugin.CryptoUI.Data
{
	[DisplayName("Convert CERT to CSR/PEM")]
	[Description("Convert certificate (crt) to certificate signing request (csr) and privacy enchanced certificate (pem)")]
	internal class CertificateToCsrPemRequest : ICertRequest
	{
		private readonly PluginWindows _plugin;

		[Category("Files")]
		[Description("Select certificate file (*.crt) to convert")]
		[Editor(typeof(CertFileOpener), typeof(UITypeEditor))]
		public String CertificatePath { get; set; }

		[Category("Security")]
		[Description("Encryption algorihtm")]
		[DefaultValue("SHA256WITHRSA")]
		[Editor(typeof(AlgorithmEditor), typeof(UITypeEditor))]
		public String Algorithm { get; set; } = "SHA256WITHRSA";

		[Category("Security")]
		[Description("Specify certificate password if loaded certificate is encrypted")]
		[PasswordPropertyText(true)]
		public String Password { get; set; }

		public CertificateToCsrPemRequest(PluginWindows plugin)
			=> this._plugin = plugin;

		public void Invoke()
		{
			String certFilePath = this.CertificatePath;
			String password = String.IsNullOrEmpty(this.Password) ? null : this.Password;

			if(String.IsNullOrEmpty(certFilePath) || !File.Exists(certFilePath))
				throw new ArgumentException("Certificate file does not exist");
			else
			{
				X509Certificate2 cert = new X509Certificate2(certFilePath, password, X509KeyStorageFlags.Exportable);
				KeyValuePair<String, String> csrPem = this._plugin.CreateCsrAndPem(cert, this.Algorithm, password);
				String csrPath = Path.Combine(Path.GetDirectoryName(certFilePath), Path.GetFileNameWithoutExtension(certFilePath)) + ".csr";
				String pemPath = Path.Combine(Path.GetDirectoryName(certFilePath), Path.GetFileNameWithoutExtension(certFilePath)) + ".pem";
				File.WriteAllText(csrPath, csrPem.Key);
				File.WriteAllText(pemPath, csrPem.Value);
			}
		}
	}
}