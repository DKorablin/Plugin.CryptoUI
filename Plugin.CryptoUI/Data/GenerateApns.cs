using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using Plugin.CryptoUI.UI;

namespace Plugin.CryptoUI.Data
{
	[DisplayName("Generate APNS Certificate")]
	internal class GenerateApns : ICertificateUI
	{
		private class Constants
		{
			public const String X9Algorithm = "IdECPublicKey";
			public const String EncAlgorithm = "secp256r1";
		}

		[Category("Security")]
		[Description("Encryption algorithm")]
		[DefaultValue(Constants.EncAlgorithm)]
		[Editor(typeof(EcAlgorithmEditor), typeof(UITypeEditor))]
		public String EncAlgorithm { get; set; } = Constants.EncAlgorithm;

		[Category("Security")]
		[Description("AlgorithmIdentifier with curve OID")]
		[DefaultValue(Constants.X9Algorithm)]
		[Editor(typeof(X9ObjectIdentifierEditor), typeof(UITypeEditor))]
		public String X9ObjectIdentifier { get; set; } = Constants.X9Algorithm;

		[Category("Output")]
		[DisplayName("Generate public key")]
		[Description("This key is used to validate JWT access token. Will be stored as .pem file")]
		[DefaultValue(true)]
		public Boolean GeneratePublicKey { get; set; } = true;

		[Category("Output")]
		[DisplayName("Generate private key")]
		[Description("This key is used to generate access token for APNS servers. Will be stored as .p8 file")]
		[DefaultValue(true)]
		public Boolean GeneratePrivateKey { get; set; } = true;

		void ICertificateUI.Invoke()
		{
			if(this.GeneratePrivateKey == false && this.GeneratePublicKey == false)
				throw new ArgumentException("Please specify what type of keys to generate");

			var ecParams = SecNamedCurves.GetByName(this.EncAlgorithm);
			var domainParams = new ECDomainParameters(ecParams.Curve, ecParams.G, ecParams.N, ecParams.H);
			var keyGenParams = new ECKeyGenerationParameters(domainParams, new SecureRandom());

			using(SaveFileDialog dlg = new SaveFileDialog() { OverwritePrompt = false, AddExtension = true, DefaultExt = "p8", Filter = "Private key file (*.p8)|*.p8" })
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					String privateKeyFileName = this.GeneratePrivateKey ? dlg.FileName : null;
					Boolean privateKeyFileNameExists = this.GeneratePrivateKey && File.Exists(privateKeyFileName);
					String publicKeyFileName = this.GeneratePublicKey ? dlg.FileName.Replace(".p8", ".pem") : null;
					Boolean publicKeyFileNameExists = this.GeneratePublicKey && File.Exists(publicKeyFileName);

					if(privateKeyFileNameExists || publicKeyFileNameExists)
					{
						List<String> files = new List<String>();
						if(privateKeyFileNameExists)
							files.Add(privateKeyFileName);
						if(publicKeyFileNameExists)
							files.Add(publicKeyFileName);

						String message = String.Format("{0} already exists.\r\nDo you want to replace {1}?", String.Join(", ", files.ToArray()), files.Count == 1 ? "it" : "them");
						if(MessageBox.Show(message, "Confirm Save As", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
							return;
					}

					var keyGen = new ECKeyPairGenerator();
					keyGen.Init(keyGenParams);
					var keyPair = keyGen.GenerateKeyPair();

					if(this.GeneratePrivateKey)
					{
						var privateKey = (ECPrivateKeyParameters)keyPair.Private;

						// 2. Create AlgorithmIdentifier with curve OID
						var curveOid = BouncyCastleReflection.GetDerObjectIdentifier(this.EncAlgorithm);//secp256r1
						var algorithmId = new AlgorithmIdentifier(BouncyCastleReflection.GetX9ObjectIdentifier(this.X9ObjectIdentifier), curveOid);//IdECPublicKey

						// 3. Encode private key to ECPrivateKey structure
						var ecPrivateKey = new Org.BouncyCastle.Asn1.Sec.ECPrivateKeyStructure(privateKey.D, ecParams);

						// 4. Create PKCS#8 PrivateKeyInfo
						var privateKeyInfo = new PrivateKeyInfo(algorithmId, ecPrivateKey.ToAsn1Object());

						var privateKeyBytes = privateKeyInfo.GetEncoded(); // DER-encoded PKCS#8

						var pemObj = new PemObject("PRIVATE KEY", privateKeyBytes);

						using(var writer = new StreamWriter(privateKeyFileName))
						{
							var pemWriter = new PemWriter(writer);
							pemWriter.WriteObject(pemObj);
							writer.Flush();
						}
					}

					if(this.GeneratePublicKey)
					{
						var publicKey = (ECPublicKeyParameters)keyPair.Public;
						var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
						var publicKeyBytes = publicKeyInfo.GetEncoded();

						using(var pubWriter = new StreamWriter(publicKeyFileName))
						{
							var pemWriter = new PemWriter(pubWriter);
							pemWriter.WriteObject(new PemObject("PUBLIC KEY", publicKeyBytes));
						}
					}
				}
		}
	}
}