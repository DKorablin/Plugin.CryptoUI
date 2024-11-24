using System;
using System.Collections;
using System.Collections.Generic;
using AlphaOmega.Design;

namespace Plugin.CryptoUI.UI
{
	internal class AlgorithmEditor : ListBoxEditorBase
	{
		protected override IEnumerable<ListBoxItem> GetValues()
		{
			IEnumerable<String> algorithms = PluginWindows.GetAlgorithmNamesI();
			foreach(String item in algorithms)
				yield return new ListBoxItem(item, item);
		}
	}
}