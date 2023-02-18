using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplexMethodWithGUI.Utilities
{
	public class SimplexMethod
	{
		private double _f;
		private List<double> _solutions, _resultsOfConstraints;
		private List<List<double>> _constraints;
		private IDestination _destination;

		public SimplexMethod(List<double> solutions, List<List<double>> constraints, List<double> resultsOfConstraints, IDestination destination)
			=> (_solutions, _constraints, _resultsOfConstraints, _destination) = (solutions, constraints, resultsOfConstraints, destination);

		public (double[] x, double f) GetResult(int countVariables)
		{
			while (_solutions.Any(x => x > 0))
			{
				MatrixElement supportElement = GetSupportElement();
				RecalculateSupportRow(supportElement);

				foreach (int row in Enumerable.Range(0, _constraints.Count).Except(new[] { supportElement.Row }))
				{
					double elemInRow = _constraints[row][supportElement.Column];
					_resultsOfConstraints[row] -= RecalculateRowResult(supportElement.Row, elemInRow);
					RecalculateRow(_constraints[row], supportElement.Row, elemInRow);
				}
				_f -= RecalculateRowResult(supportElement.Row, _solutions[supportElement.Column]);
				RecalculateRow(_solutions, supportElement.Row, _solutions[supportElement.Column]);
			}
			return (_destination.GetCoefficients(countVariables, Enumerable.Range(0, countVariables).Select(
				   i => _solutions[i] < 0 ? 0 : _resultsOfConstraints[_constraints.Select(x => x[i]).ToList().IndexOf(1)]
			   ).Concat(_solutions.Skip(countVariables)).Select(x => Math.Round(x, 3)).ToList()), Math.Abs(_f));
		}

		private double RecalculateRowResult(int row, double coefficient)
			=> coefficient * _resultsOfConstraints[row];

		private void RecalculateRow(List<double> sequence, int row, double coefficient)
			=> Array.ForEach(Enumerable.Range(0, sequence.Count).ToArray(), column => sequence[column] -= coefficient * _constraints[row][column]);

		private void RecalculateSupportRow(MatrixElement supportElement)
		{
			for (int column = 0; column < _constraints[supportElement.Row].Count; ++column)
				_constraints[supportElement.Row][column] /= supportElement.Value;
			_resultsOfConstraints[supportElement.Row] /= supportElement.Value;
		}

		private MatrixElement GetSupportElement()
		{
			var sequence = _constraints.Select((x, row) => x.Select(
				(y, column) => new MatrixElement(new[] {
						y, _resultsOfConstraints[row], _solutions[column]
					}.All(t => t > 0) ? _resultsOfConstraints[row] / y : double.MaxValue, row, column)
			).ToList()).ToList() ?? throw new ArgumentNullException();

			MatrixElement max = new(0, 0, 0);
			for (int column = 0; column < sequence[0].Count; ++column)
			{
				MatrixElement min = new(double.MaxValue, 0, 0);
				for (int row = 0; row < sequence.Count; ++row)
					min = MatrixElement.GetMinElement(sequence[row][column], min);
				min.Value *= _solutions[column];
				max = MatrixElement.GetMaxElement(max, min);
			}

			return max with { Value = _constraints[max.Row][max.Column] };
		}
	}
}
