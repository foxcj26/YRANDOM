using System.ComponentModel;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using ClassIsland.Core.Controls;

namespace ClassIsland.YRAND.Views;

public class YrandNotificationControl : UserControl
{
	public class ControlModel : INotifyPropertyChanged
	{
		private string _prefix = "正在抽选";

		private string _name = "";

		public string Prefix
		{
			get
			{
				return _prefix;
			}
			set
			{
				if (!(_prefix == value))
				{
					_prefix = value;
					Raise("Prefix");
				}
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (!(_name == value))
				{
					_name = value;
					Raise("Name");
				}
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void Raise(string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public ControlModel Model { get; } = new ControlModel();


	public YrandNotificationControl()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		TextBlock val = new TextBlock
		{
			FontWeight = (FontWeight)700,
			Opacity = 0.7,
			VerticalAlignment = (VerticalAlignment)2,
			FontSize = 22.0
		};
		((AvaloniaObject)val).Bind((AvaloniaProperty)(object)TextBlock.TextProperty, (IBinding)new Binding("Prefix", (BindingMode)0)
		{
			Source = Model
		});
		Border val2 = new Border
		{
			Width = 2.0,
			Height = 20.0,
			Background = (IBrush)(object)Brushes.Black,
			Opacity = 0.8,
			VerticalAlignment = (VerticalAlignment)2,
			Margin = new Thickness(2.0, 0.0, 2.0, 0.0)
		};
		TextBlock val3 = new TextBlock
		{
			FontWeight = (FontWeight)700,
			VerticalAlignment = (VerticalAlignment)2,
			FontSize = 26.0
		};
		((AvaloniaObject)val3).Bind((AvaloniaProperty)(object)TextBlock.TextProperty, (IBinding)new Binding("Name", (BindingMode)0)
		{
			Source = Model
		});
		StackPanel val4 = new StackPanel
		{
			Orientation = (Orientation)0,
			HorizontalAlignment = (HorizontalAlignment)2,
			VerticalAlignment = (VerticalAlignment)2,
			Spacing = 8.0
		};
		// YRAND 品牌标识：Fluent System Icons「抽签 lottery」实心版（\uEAFB）
		FluentIcon valBrandIcon = new FluentIcon("\uEAFB")
		{
			FontSize = 24.0,
			Opacity = 0.9,
			VerticalAlignment = (VerticalAlignment)2,
			Margin = new Thickness(0.0, 0.0, 2.0, 0.0)
		};
		((AvaloniaList<Control>)(object)((Panel)val4).Children).Add((Control)(object)valBrandIcon);
		((AvaloniaList<Control>)(object)((Panel)val4).Children).Add((Control)(object)val);
		((AvaloniaList<Control>)(object)((Panel)val4).Children).Add((Control)(object)val2);
		((AvaloniaList<Control>)(object)((Panel)val4).Children).Add((Control)(object)val3);
		((ContentControl)this).Content = (object)val4;
	}
}
