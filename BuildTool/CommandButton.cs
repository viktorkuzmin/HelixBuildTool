using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BuildTool
{
	public class CommandButton : Button 
	{
		public static readonly DependencyProperty CmdCommandProperty = DependencyProperty.Register("CmdCommand", typeof(string), typeof(CommandButton), new UIPropertyMetadata(string.Empty));

		public string CmdCommand
		{
			get { return (string)GetValue(CmdCommandProperty); }

			set { SetValue(CmdCommandProperty, value); }
		}
	}
}
