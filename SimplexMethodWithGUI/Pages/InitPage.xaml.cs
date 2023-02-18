using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SimplexMethodWithGUI.Pages
{
	public partial class InitPage : Page
	{
		public InitPage()
			=> InitializeComponent();

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (!Int32.TryParse(CountConstraints.Text, out int countConstraints))
			{
				MessageBox.Show("Некорректно введено количество ограничений!");
				return;
			}
			if (!Int32.TryParse(CountVariables.Text, out int countVariables))
			{
				MessageBox.Show("Некорректно введено количество переменных!");
				return;
			}
			NavigationService.Navigate(new CalculationPage(countConstraints, countVariables));
		}
	}
}
