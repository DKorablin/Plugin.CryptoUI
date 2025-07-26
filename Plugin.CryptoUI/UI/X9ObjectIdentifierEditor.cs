using System.Collections.Generic;
using AlphaOmega.Design;

namespace Plugin.CryptoUI.UI
{
	internal class X9ObjectIdentifierEditor : ListBoxEditorBase
	{
		protected override IEnumerable<ListBoxItem> GetValues()
		{
			var algorithms = BouncyCastleReflection.GetX9ObjectIdentifiers();
			foreach(var item in algorithms)
				yield return new ListBoxItem(item.Name, item);
		}
	}
}
