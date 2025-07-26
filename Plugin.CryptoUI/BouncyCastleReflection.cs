using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;

namespace Plugin.CryptoUI
{
	internal static class BouncyCastleReflection
	{
		internal static IEnumerable<String> GetAlgorithmNamesI()
		{
			var x509utilities = Assembly.GetAssembly(typeof(IX509Extension)).GetType("Org.BouncyCastle.X509.X509Utilities", true);

			var algorithms = (IEnumerable)x509utilities.InvokeMember("GetAlgNames", BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, null);
			foreach(Object item in algorithms)
				yield return item.ToString();
		}

		internal static DerObjectIdentifier GetDerObjectIdentifier(String encAlgorithm)
		{
			var secObjectIdentifiersType = typeof(Org.BouncyCastle.Asn1.Sec.SecObjectIdentifiers);
			return (DerObjectIdentifier)secObjectIdentifiersType.InvokeMember(encAlgorithm, BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static, null, null, null);
		}

		internal static IEnumerable<FieldInfo> GetX9ObjectIdentifiers()
		{
			var x9ObjectIdentifiersType = typeof(Org.BouncyCastle.Asn1.X9.X9ObjectIdentifiers);

			var fields = x9ObjectIdentifiersType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
			foreach(var field in fields)
				if(field.FieldType == typeof(DerObjectIdentifier))
					yield return field;
		}

		internal static DerObjectIdentifier GetX9ObjectIdentifier(String name)
		{
			foreach(var field in GetX9ObjectIdentifiers())
				if(field.Name == name)
					return (DerObjectIdentifier)field.GetValue(null);

			throw new InvalidOperationException($"X9 object identifier {name} not found");
		}
	}
}