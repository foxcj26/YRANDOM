using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Markup.Xaml.XamlIl.Runtime;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
[CompilerGenerated]
public class _0021AvaloniaResources
{
	[CompilerGenerated]
	internal class NamespaceInfo_003A_002FViews_002FSettingsPages_002FYrandSettingsPage_002Eaxaml : IAvaloniaXamlIlXmlNamespaceInfoProvider
	{
		private IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> _xmlNamespaces;

		public static IAvaloniaXamlIlXmlNamespaceInfoProvider Singleton;

		public virtual IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> XmlNamespaces
		{
			get
			{
				if (_xmlNamespaces == null)
				{
					_xmlNamespaces = CreateNamespaces();
				}
				return _xmlNamespaces;
			}
		}

		private static AvaloniaXamlIlXmlNamespaceInfo CreateNamespaceInfo(string P_0, string P_1)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			return new AvaloniaXamlIlXmlNamespaceInfo
			{
				ClrNamespace = P_0,
				ClrAssemblyName = P_1
			};
		}

		private static IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> CreateNamespaces()
		{
			return new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(4)
			{
				{
					"",
					(IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>)(object)new AvaloniaXamlIlXmlNamespaceInfo[47]
					{
						CreateNamespaceInfo("Avalonia", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Animation", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Animation.Easings", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Data", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Data.Converters", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Input", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Input.GestureRecognizers", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Input.TextInput", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Layout", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.LogicalTree", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Media", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Media.Imaging", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Media.Transformation", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Styling", "Avalonia.Base"),
						CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.ColorPicker"),
						CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.ColorPicker"),
						CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.ColorPicker"),
						CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls.DataGrid"),
						CreateNamespaceInfo("Avalonia.Controls.Collections", "Avalonia.Controls.DataGrid"),
						CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls.DataGrid"),
						CreateNamespaceInfo("Avalonia", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Automation", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Embedding", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Presenters", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Shapes", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Templates", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Notifications", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Chrome", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Controls.Documents", "Avalonia.Controls"),
						CreateNamespaceInfo("Avalonia.Data", "Avalonia.Markup"),
						CreateNamespaceInfo("Avalonia.Markup.Data", "Avalonia.Markup"),
						CreateNamespaceInfo("Avalonia.Markup.Xaml.MarkupExtensions", "Avalonia.Markup.Xaml"),
						CreateNamespaceInfo("Avalonia.Markup.Xaml.Styling", "Avalonia.Markup.Xaml"),
						CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml"),
						CreateNamespaceInfo("Avalonia.Svg", "Avalonia.Svg"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactivity", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions.Core", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions.Custom", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions.Custom.Converters", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions.DragAndDrop", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions.Draggable", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions.Events", "Xaml.Behaviors"),
						CreateNamespaceInfo("Avalonia.Xaml.Interactions.Responsive", "Xaml.Behaviors")
					}
				},
				{
					"x",
					(IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>)(object)new AvaloniaXamlIlXmlNamespaceInfo[0]
				},
				{
					"ci",
					(IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>)(object)new AvaloniaXamlIlXmlNamespaceInfo[14]
					{
						CreateNamespaceInfo("ClassIsland.Core", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Converters", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Controls", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Controls.CommonDialog", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Controls.LessonsControls", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Controls.IconControl", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Controls.NavHyperlink", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Controls.Ruleset", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Commands", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Abstractions.Controls", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Controls.StickerControl", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Abstractions.Views", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Behaviors", "ClassIsland.Core"),
						CreateNamespaceInfo("ClassIsland.Core.Assists", "ClassIsland.Core")
					}
				},
				{
					"fa",
					(IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>)(object)new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("FluentAvalonia.UI.Controls", "FluentAvalonia") }
				}
			};
		}

		static NamespaceInfo_003A_002FViews_002FSettingsPages_002FYrandSettingsPage_002Eaxaml()
		{
			Singleton = (IAvaloniaXamlIlXmlNamespaceInfoProvider)(object)new NamespaceInfo_003A_002FViews_002FSettingsPages_002FYrandSettingsPage_002Eaxaml();
		}
	}
}
