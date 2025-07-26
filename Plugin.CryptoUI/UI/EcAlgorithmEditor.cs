using System;
using System.Collections.Generic;
using AlphaOmega.Design;
using Org.BouncyCastle.Asn1.Sec;

namespace Plugin.CryptoUI.UI
{
	internal class EcAlgorithmEditor : ListBoxEditorBase
	{
		protected override IEnumerable<ListBoxItem> GetValues()
		{
			foreach(String item in SecNamedCurves.Names)
				yield return new ListBoxItem(item, item);
		}
	}
}