using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClassIsland.YRAND.Views.SettingsPages;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
[CompilerGenerated]
public class _0021XamlLoader
{
	public static object TryLoad(IServiceProvider P_0, string P_1)
	{
		if (string.Equals(P_1, "avares://ClassIsland.YRAND/Views/SettingsPages/YrandSettingsPage.axaml", StringComparison.OrdinalIgnoreCase))
		{
			return new YrandSettingsPage();
		}
		return null;
	}

	public static object TryLoad(string P_0)
	{
		return TryLoad(null, P_0);
	}
}
