using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using SAL.Flatbed;
using SAL.Windows;
using SystemCert = System.Security.Cryptography.X509Certificates;

namespace Plugin.CryptoUI
{
	public class PluginWindows : IPlugin
	{
		class PasswordFinder : IPasswordFinder
		{
			private readonly String _password;
			public PasswordFinder(String password)
				=> this._password = password;

			Char[] IPasswordFinder.GetPassword()
				=> this._password.ToCharArray();
		}

		private readonly IHost _host;
		private TraceSource _trace;
		private Dictionary<String, DockState> _documentTypes;

		internal IHostWindows HostWindows => this._host as IHostWindows;
		private IMenuItem ConfigMenu { get; set; }

		internal TraceSource Trace => this._trace ?? (this._trace = PluginWindows.CreateTraceSource<PluginWindows>());

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(PanelCryptoUI).ToString(), DockState.DockLeftAutoHide },
						//{ typeof(PanelExceptionLogger).ToString(), DockState.DockLeftAutoHide },
						//{ typeof(DocumentChart).ToString(), DockState.Document },
					};
				return this._documentTypes;
			}
		}

		public PluginWindows(IHost host)
			=> this._host = host ?? throw new ArgumentNullException(nameof(host));

		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

		/// <summary>Get a list of supported algorithms</summary>
		/// <returns>List of algorithms for creating a certificate</returns>
		public IEnumerable<String> GetAlgorithmNames()
			=> PluginWindows.GetAlgorithmNamesI();

		/// <summary>Convert private key and X509 certificate with PKCS#12</summary>
		/// <param name="cert">X509 certificate as an array of bytes</param>
		/// <param name="pem">Private key</param>
		/// <param name="password">Password from private key</param>
		/// <returns>Certificate in PKCS#12 format</returns>
		public Byte[] ConvertCertPemToPkcs12(Byte[] cert, Byte[] pem, String password)
		{
			if(cert == null || cert.Length == 0)
				throw new ArgumentNullException(nameof(cert));
			if(pem == null || pem.Length == 0)
				throw new ArgumentNullException(nameof(pem));
			if(String.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password));

			AsymmetricCipherKeyPair privateKey;
			Org.BouncyCastle.X509.X509Certificate certMem;
			IPasswordFinder cPassword = new PasswordFinder(password);

			using(MemoryStream stream = new MemoryStream(pem))
			using(StreamReader reader = new StreamReader(stream))
			{
				PemReader pemReader = new PemReader(reader, cPassword);
				privateKey = (AsymmetricCipherKeyPair)pemReader.ReadObject();
			}

			using(MemoryStream stream = new MemoryStream(cert))
			using(StreamReader reader = new StreamReader(stream))
			{
				PemReader pemCert = new PemReader(reader);
				certMem = (X509Certificate)pemCert.ReadObject();
			}

			Pkcs12Store store = new Pkcs12Store();
			X509CertificateEntry certificateEntry = new X509CertificateEntry(certMem);
			String friendlyName = certMem.SubjectDN.ToString();
			store.SetCertificateEntry(friendlyName, certificateEntry);
			store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(privateKey.Private), new[] { certificateEntry });

			using(MemoryStream stream = new MemoryStream())
			{
				SecureRandom random = new SecureRandom();
				store.Save(stream, cPassword.GetPassword(), random);

				return stream.ToArray();
			}
		}

		/// <summary>Creating a certificate using the BouncyCastle assembly</summary>
		/// <param name="subject">Friendly name of the certificate</param>
		/// <param name="password">When you specify a password, a PFX certificate will be generated. No password - CRT.</param>
		/// <param name="strength">Key size. Recommended to use at least 2048Bit</param>
		/// <param name="algorithm">Encryption algorithm, default is SHA256WITHRSA</param>
		/// <param name="from">Certificate Validity Start Date</param>
		/// <param name="to">Certificate validity end date</param>
		/// <param name="extensions">Certificate extensions</param>
		/// <exception cref="ArgumentException">Certificate friendly name not specified</exception>
		/// <exception cref="ArgumentException">The validity date of the certificate is greater than the expiration date of the certificate</exception>
		/// <exception cref="ArgumentException">Encryption algorithm not specified</exception>
		/// <returns>Generated certificate as an array of bytes</returns>
		public Byte[] GenerateCertificate(String subject, String password, Int32 strength, String algorithm, DateTime from, DateTime to, params KeyValuePair<String, String>[] extensions)
		{
			if(String.IsNullOrEmpty(subject))
				throw new ArgumentException("Subject is empty", nameof(subject));
			else if(to < from)
				throw new ArgumentException("Date FROM must be less than date TO");
			else if(String.IsNullOrEmpty(algorithm))
				throw new ArgumentException("Encryption algorthm not selected", nameof(algorithm));

			KeyValuePair<SystemCert.X509Certificate, AsymmetricCipherKeyPair> certificate = this.GenerateCertificateI(subject, strength, algorithm, from, to, extensions);
			if(String.IsNullOrEmpty(password))
				return certificate.Key.Export(SystemCert.X509ContentType.Cert);
			else
			{
				SystemCert.X509Certificate2 certificate2 = this.ConvertToCertificateWithPrivateKey(certificate.Key, certificate.Value, password);
				return certificate2.Export(SystemCert.X509ContentType.Pfx, password);
			}
		}

		/// <summary>Convert certificate from CRT format to Certificate Request (CSR) format and private unsigned part of PEM certificate</summary>
		/// <param name="certificate">Initial Certificate</param>
		/// <param name="algorithm">Encryption algorithm</param>
		/// <param name="password">Password</param>
		/// <exception cref="ArgumentNullException">certificate is null</exception>
		/// <exception cref="ArgumentException">Encryption algorithm not selected</exception>
		/// <returns></returns>
		public KeyValuePair<String, String> CreateCsrAndPem(SystemCert.X509Certificate2 certificate, String algorithm, String password)
		{
			if(certificate == null)
				throw new ArgumentNullException(nameof(certificate), "Certificate file does not exist");
			else if(String.IsNullOrEmpty(algorithm))
				throw new ArgumentException("Encryption algorthm not selected", nameof(algorithm));

			X509Name name = new X509Name(certificate.Subject);
			AsymmetricCipherKeyPair ackp = DotNetUtilities.GetKeyPair(certificate.PrivateKey);

			//Key generation 2048bits
			/*RsaKeyPairGenerator rkpg = new RsaKeyPairGenerator();
			rkpg.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
			AsymmetricCipherKeyPair ackp = rkpg.GenerateKeyPair();*/

			//PKCS #10 Certificate Signing Request
			Pkcs10CertificationRequest request = new Pkcs10CertificationRequest(algorithm, name, ackp.Public, null, ackp.Private);

			//Convert BouncyCastle CSR to .PEM file.
			StringBuilder cerificateCsr = new StringBuilder();
			PemWriter csrWriter = new PemWriter(new StringWriter(cerificateCsr));
			csrWriter.WriteObject(request);
			csrWriter.Writer.Flush();

			//Convert BouncyCastle Private Key to .PEM file.
			StringBuilder certificatePem = new StringBuilder();
			PemWriter pemWriter = new PemWriter(new StringWriter(certificatePem));
			pemWriter.WriteObject(ackp.Private);
			pemWriter.Writer.Flush();

			String csrResult = cerificateCsr.ToString();
			String pemResult = certificatePem.ToString();

			return new KeyValuePair<String, String>(csrResult, pemResult);
		}

		public Byte[] ConvertPemToPkcs12(Byte[] pem, String password)
		{
			if(pem == null || pem.Length == 0)
				throw new ArgumentNullException(nameof(pem));
			if(String.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password));

			AsymmetricCipherKeyPair privateKey;
			IPasswordFinder cPassword = new PasswordFinder(password);

			using(MemoryStream stream = new MemoryStream(pem))
			using(StreamReader reader = new StreamReader(stream))
			{
				PemReader pemReader = new PemReader(reader, cPassword);
				privateKey = (AsymmetricCipherKeyPair)pemReader.ReadObject();
			}

			Pkcs12Store store = new Pkcs12Store();

			using(MemoryStream stream = new MemoryStream())
			{
				SecureRandom random = new SecureRandom();
				store.Save(stream, cPassword.GetPassword(), random);

				return stream.ToArray();
			}
		}

		private KeyValuePair<SystemCert.X509Certificate, AsymmetricCipherKeyPair> GenerateCertificateI(String subject, Int32 strength, String algorithm, DateTime from, DateTime to, params KeyValuePair<String, String>[] extensions)
		{
			RsaKeyPairGenerator kpgen = new RsaKeyPairGenerator();

			SecureRandom random = new SecureRandom(new CryptoApiRandomGenerator());
			kpgen.Init(new KeyGenerationParameters(random, strength));

			AsymmetricCipherKeyPair ackp = kpgen.GenerateKeyPair();

			X509Name certName = new X509Name(subject);
			BigInteger serialNo = BigInteger.ProbablePrime(120, new Random());

			X509V3CertificateGenerator gen = new X509V3CertificateGenerator();
			gen.SetSerialNumber(serialNo);
			gen.SetSubjectDN(certName);
			gen.SetIssuerDN(certName);
			gen.SetNotAfter(to);
			gen.SetNotBefore(from);
			gen.SetPublicKey(ackp.Public);

			if(extensions != null)
				foreach(KeyValuePair<String, String> extension in extensions)
				{
					if(extension.Key == X509Extensions.AuthorityKeyIdentifier.Id)//X509Extensions.AuthorityKeyIdentifier.Id
						gen.AddExtension(X509Extensions.AuthorityKeyIdentifier.Id, false,
							new AuthorityKeyIdentifier(
								SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(ackp.Public),
								new GeneralNames(new GeneralName(certName)),
								serialNo));
					else if(extension.Key == X509Extensions.ExtendedKeyUsage.Id)//X509Extensions.ExtendedKeyUsage.Id
						gen.AddExtension(
							X509Extensions.ExtendedKeyUsage.Id,
							false,
							new ExtendedKeyUsage(new DerObjectIdentifier[] { new DerObjectIdentifier(extension.Value), }));
					else
						gen.AddExtension(
							extension.Key,
							false,
							Encoding.UTF8.GetBytes(extension.Value));
				}

			Asn1SignatureFactory factory = new Asn1SignatureFactory(algorithm, ackp.Private, random);
			X509Certificate result = gen.Generate(factory);
			return new KeyValuePair<SystemCert.X509Certificate, AsymmetricCipherKeyPair>(DotNetUtilities.ToX509Certificate(result), ackp);
		}

		private SystemCert.X509Certificate2 ConvertToCertificateWithPrivateKey(SystemCert.X509Certificate certificate, AsymmetricCipherKeyPair ackp, String password)
		{
			Pkcs12Store newStore = new Pkcs12Store();

			X509Certificate cert = DotNetUtilities.FromX509Certificate(certificate);
			X509CertificateEntry certEntry = new X509CertificateEntry(cert);

			newStore.SetCertificateEntry(
				Environment.MachineName,
				certEntry);

			newStore.SetKeyEntry(
				Environment.MachineName,
				new AsymmetricKeyEntry(ackp.Private),
				new[] { certEntry });

			using(MemoryStream stream = new MemoryStream())
			{
				newStore.Save(stream, password.ToCharArray(), new SecureRandom(new CryptoApiRandomGenerator()));

				// reload key
				return new SystemCert.X509Certificate2(stream.ToArray(), password, SystemCert.X509KeyStorageFlags.Exportable);
			}
		}

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IHostWindows host = this.HostWindows;
			if(host != null)
			{
				IMenuItem menuTools = host.MainMenu.FindMenuItem("Tools");
				if(menuTools == null)
					this.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'Tools' not found");
				{
					this.ConfigMenu = menuTools.Create("&Crypto");
					this.ConfigMenu.Name = "Tools.Crypto";
					this.ConfigMenu.Click += (sender, e) => { this.CreateWindow(typeof(PanelCryptoUI).ToString(), true); };
					menuTools.Items.Insert(0, this.ConfigMenu);
				}
			}
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(this.ConfigMenu != null)
				this.HostWindows.MainMenu.Items.Remove(this.ConfigMenu);
			return true;
		}

		internal static IEnumerable<String> GetAlgorithmNamesI()
		{
			Type x509utilities = Assembly.GetAssembly(typeof(IX509Extension)).GetType("Org.BouncyCastle.X509.X509Utilities", true);

			IEnumerable algorithms = (IEnumerable)x509utilities.InvokeMember("GetAlgNames", BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, null);
			foreach(Object item in algorithms)
				yield return item.ToString();
		}

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}

		private IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
			=> this.DocumentTypes.TryGetValue(typeName, out DockState state)
				? this.HostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
				: null;
	}
}