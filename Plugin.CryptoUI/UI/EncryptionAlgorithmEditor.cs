using System;
using System.Collections.Generic;
using AlphaOmega.Design;

namespace Plugin.CryptoUI.UI
{
	internal class EncryptionAlgorithmEditor : ListBoxEditorBase
	{
		protected override IEnumerable<ListBoxItem> GetValues()
		{
			IEnumerable<String> algorithms = BouncyCastleReflection.GetAlgorithmNamesI();
			foreach(String item in algorithms)
				yield return new ListBoxItem(item, item);
		}
	}
}