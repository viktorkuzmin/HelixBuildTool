using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace BuildTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly string excludeFolder = "App";
		readonly string solutionPathCommand = string.Format("cd {0}\\vrtp_web\\Site", Properties.Settings.Default.RepoLocation);

		private List<Tuple<string, List<String>>> projectsButtons = new List<Tuple<string, List<String>>>();

		public MainWindow()
		{
			InitializeComponent();

			var userPrefs = new AppSettings();

			this.Height = userPrefs.WindowHeight;
			this.Width = userPrefs.WindowWidth;
			this.Top = userPrefs.WindowTop;
			this.Left = userPrefs.WindowLeft;
			this.WindowState = userPrefs.WindowState;

			CreateProjectButtons();
		}

		public void Watch_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(Properties.Settings.Default.CmdPath, solutionPathCommand + "&gulp watch");
		}

		public async void Run(string command)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;

			await Task.Run(() =>
			{
				Process p = new Process();
				ProcessStartInfo info = new ProcessStartInfo();
				info.FileName = Properties.Settings.Default.CmdPath;
				info.CreateNoWindow = true;
				info.RedirectStandardInput = true;
				info.RedirectStandardOutput = true;
				info.RedirectStandardError = true;
				info.UseShellExecute = false;

				p.OutputDataReceived += new DataReceivedEventHandler(DataOutputHandler);
				p.ErrorDataReceived += new DataReceivedEventHandler(ErrorOutputHandler);
				p.StartInfo = info;
				p.Start();
				p.BeginOutputReadLine();
				p.BeginErrorReadLine();


				p.StandardInput.WriteLine(solutionPathCommand);
				p.StandardInput.WriteLine(command);
				p.StandardInput.Flush();
				p.StandardInput.Close();
				p.WaitForExit();

				tcButtons.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
										new Action(delegate ()
										{
											tcButtons.IsEnabled = true;
										}));

			});			
		}

		private void DataOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			// Collect the sort command output. 
			tbOutputText.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
													new Action(delegate ()
													{
														tbOutputText.Text += "\n" + outLine.Data;
														scvOutputText.ScrollToBottom();
													}));
		}

		private void ErrorOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			// Collect the sort command output. 
			tbOutputText.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
													new Action(delegate ()
													{
														tbOutputText.Text += "\n" + outLine.Data;
														scvOutputText.ScrollToBottom();
													}));
		}

		private void DisableButtons()
		{
			tcButtons.IsEnabled = false;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var userPrefs = new AppSettings();

			userPrefs.WindowHeight = this.Height;
			userPrefs.WindowWidth = this.Width;
			userPrefs.WindowTop = this.Top;
			userPrefs.WindowLeft = this.Left;
			userPrefs.WindowState = this.WindowState;

			userPrefs.Save();
		}

		private void CI_Button_Click(object sender, RoutedEventArgs e)
		{
			CommandButton button = (CommandButton)sender;

			if (button == null)
				return;

			Run(string.Format(Properties.Settings.Default.CICommandBase, button.CmdCommand));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			CommandButton button = (CommandButton)sender;

			if (button == null)
				return;

			Run(button.CmdCommand);
		}

		private void CreateProjectButtons()
		{
			var topFolders = Directory.GetDirectories($"{Properties.Settings.Default.RepoLocation}\\vrtp_web\\Site\\src");

			var data = topFolders
				.Where(x => !Path.GetFileName(x).Equals(excludeFolder))
				.Select(x => new Tuple<string, IList<string>>(Path.GetFileName(x), Directory.GetDirectories(x).Select(y => Path.GetFileName(y)).ToList()))
				.ToList();

			var scrollViewer = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };

			var topStackPanel = new StackPanel
			{
				Margin = new Thickness(10),
			};

			scrollViewer.Content = topStackPanel;

			foreach (var item in data)
			{
				var groupBox = new GroupBox()
				{
					Header = item.Item1
				};

				var buttonStackPanel = new StackPanel();

				groupBox.Content = buttonStackPanel;

				foreach (var name in item.Item2)
				{
					var button = new CommandButton()
					{
						Content = name,
						Margin = new Thickness(10, 5, 10, 0),
						CmdCommand = string.Format(Properties.Settings.Default.PublishProjectCommand, $"{item.Item1}/{name}")
					};

					button.Click += Button_Click;

					buttonStackPanel.Children.Add(button);
				}

				topStackPanel.Children.Add(groupBox);
			}

			tciProjectButtons.Content = scrollViewer;
		}
	}
}
