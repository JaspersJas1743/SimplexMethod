using SimplexMethodWithGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SimplexMethodWithGUI.Pages
{
	public partial class CalculationPage : Page
	{
		private readonly string[] _signs = new[] { ">=", "=", "<=" };
		private readonly int _countConstraints, _countVariables;
		private IDestination _destination;

		public CalculationPage(int countConstraints, int countVariables)
		{
			InitializeComponent();
			(_countConstraints, _countVariables) = (countConstraints, countVariables);

			GenerateFormFor(_countConstraints, _countVariables);
		}

		private void GenerateFormFor(int countConstraints, int countVariables)
		{
			Array.ForEach(new[] { GridForFunction, GridForConstraints }, x => x.Columns = countVariables);
			Array.ForEach(new[] { GridForConstraints, GridForSign, GridForResults }, x => x.Rows = countConstraints);
			Array.ForEach(Enumerable.Range(0, countVariables).ToArray(), _ => GridForFunction.Children.Add(GetTextBox()));

			for (int row = 0; row < countConstraints; ++row)
			{
				Array.ForEach(Enumerable.Range(0, countVariables).ToArray(), _ => GridForConstraints.Children.Add(GetTextBox()));
				GridForSign.Children.Add(new ComboBox() { ItemsSource = _signs, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center});
				GridForResults.Children.Add(GetTextBox());
			}

			TextBox GetTextBox()
				=> new() { HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center };
		}

		private void GetResultClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var solutions = GridForFunction.Children.OfType<TextBox>().Select(x => Double.Parse(x.Text)).ToList();
				var constraints = GetConstraints(GridForConstraints.Children.OfType<TextBox>().ToList());
				(constraints, solutions) = AddFreeMembers(constraints, solutions);
				var resultsOfConstraints = GridForResults.Children.OfType<TextBox>().Select(x => Double.Parse(x.Text)).ToList();
				var (x, f) = new SimplexMethod(solutions, constraints, resultsOfConstraints, _destination).GetResult(_countVariables);
				MessageBox.Show($"X = ({String.Join("; ", x)})\nFmax: {f:f3}");
			}
			catch
			{
				MessageBox.Show("Некорректные данные!");
			}
		}

		private void MaxSelected(object sender, RoutedEventArgs e)
			=> _destination = new MaxDestination();

		private void MinSelected(object sender, RoutedEventArgs e)
			=> _destination = new MinDestination();

		private void PreviousPageClick(object sender, RoutedEventArgs e)
			=> NavigationService.Navigate(new InitPage());

		private List<List<double>> GetConstraints(List<TextBox> textBoxes)
			=> Enumerable.Range(0, _countConstraints).Select(row => Enumerable.Range(0, _countVariables)
					.Select(column => Double.Parse(textBoxes[row * _countVariables + column].Text)).ToList()).ToList();

		private (List<List<double>> Constraints, List<double> Solutions) AddFreeMembers(List<List<double>> constraints, List<double> solutions)
		{
			for (int row = 0; row < constraints.Count; ++row)
			{
				ComboBox box = (ComboBox)GridForSign.Children[row];
				if (box.Text == "=")
					continue;

				solutions.Add(0);
				Array.ForEach(Enumerable.Range(0, _countConstraints).ToArray(),
					i => constraints[i].Add(row != i ? 0 : Array.IndexOf(_signs, box.SelectedItem.ToString()) - 1));
			}
			return (constraints, solutions);
		}
	}
}
