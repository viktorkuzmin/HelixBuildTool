using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace BuildTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly string solutionPathCommand = string.Format("cd {0}\\vrtp_web\\Site", Properties.Settings.Default.RepoLocation);

		public MainWindow()
		{
			InitializeComponent();

			var userPrefs = new AppSettings();

			this.Height = userPrefs.WindowHeight;
			this.Width = userPrefs.WindowWidth;
			this.Top = userPrefs.WindowTop;
			this.Left = userPrefs.WindowLeft;
			this.WindowState = userPrefs.WindowState;
		}

		private void PublishAllProjects_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.PublishAllProjectsComand);
		}

		private void BuildSolution_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.BuildSolutionComand);
		}

		private void PublishFoundationProjects_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.PublishFoundationProjectsComand);
		}

		private void PublishFeatureProjects_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.PublishFeatureProjectsComand);
		}
		
		private void PublishProjectProjects_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.PublishProjectProjectsComand);
		}

		private void CopyAppFiles_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.CopyAppFilesComand);
		}

		private void ApplyXmlTransform_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.ApplyXmlTransformComand);
		}

		private void SyncUnicorn_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.SyncUnicornComand);
		}

		private void Build_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.BuildComand);

		}

		private void BuildNoDoc_Click(object sender, RoutedEventArgs e)
		{
			DisableButtons();
			tbOutputText.Text = string.Empty;
			Run(Properties.Settings.Default.BuildNoDocComand);
		}

		public void Watch_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(Properties.Settings.Default.CmdPath, solutionPathCommand + "&gulp watch");
		}

		public async void Run(string command)
		{
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

				spButtonsPanel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
										new Action(delegate ()
										{
											spButtonsPanel.IsEnabled = true;
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
			spButtonsPanel.IsEnabled = false;
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
	}
}
