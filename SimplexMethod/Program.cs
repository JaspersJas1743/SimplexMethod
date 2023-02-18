using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplexMethod
{
	public struct MatrixElement
	{
		public MatrixElement(double value, int row, int column)
			=> (Value, Row, Column) = (value, row, column);

		public double Value { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }

		public static MatrixElement GetMaxElement(MatrixElement first, MatrixElement second)
			=> first.Value < second.Value ? second : first;

		public static MatrixElement GetMinElement(MatrixElement first, MatrixElement second)
			=> first.Value > second.Value ? second : first;
	}

	public class SimplexMethod
	{
		private double _f;
		private List<double> _solutions, _resultsOfConstraints;
		private List<List<double>> _constraints;

		public SimplexMethod(List<double> solutions, List<List<double>> constraints, List<double> resultsOfConstraints)
			=> (_solutions, _constraints, _resultsOfConstraints) = (solutions, constraints, resultsOfConstraints);

		public double GetResult()
		{
			while (_solutions.Any(x => x > 0))
			{
				MatrixElement supportElement = GetSupportElement();
				RecalculateSupportRow(supportElement);
				PrintMatrix();

				foreach (int row in Enumerable.Range(0, _constraints.Count).Except(new[] { supportElement.Row }))
				{
					double elemInRow = _constraints[row][supportElement.Column];
					_resultsOfConstraints[row] -= RecalculateRowResult(supportElement.Row, elemInRow);
					RecalculateRow(_constraints[row], supportElement.Row, elemInRow);
					PrintMatrix();
				}
				PrintMatrix();
				_f -= RecalculateRowResult(supportElement.Row, _solutions[supportElement.Column]);
				RecalculateRow(_solutions, supportElement.Row, _solutions[supportElement.Column]);
			}
			PrintMatrix();
			return Math.Abs(_f);
		}

		private double RecalculateRowResult(int row, double coefficient)
			=> coefficient * _resultsOfConstraints[row];

		private void RecalculateRow(List<double> sequence, int row, double coefficient)
		{
			for (int column = 0; column < sequence.Count; ++column)
				sequence[column] -= coefficient * _constraints[row][column];
		}

		private void RecalculateSupportRow(MatrixElement supportElement)
		{
			for (int column = 0; column < _constraints[supportElement.Row].Count; ++column)
				_constraints[supportElement.Row][column] /= supportElement.Value;
			_resultsOfConstraints[supportElement.Row] /= supportElement.Value;
		}

		private void PrintMatrix()
		{
			for (int row = 0; row < _constraints.Count; ++row)
				PrintRow(_constraints[row], _resultsOfConstraints[row]);
			PrintRow(_solutions, _f);
			Console.WriteLine();
		}

		private void PrintRow<T>(List<T> row, T resultField)
		{
			row.ForEach(x => Console.Write($"{x:f3}\t"));
			Console.WriteLine($"| {resultField:f3}");
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

	internal class Program
	{
		static void Main(string[] args)
		{
			SimplexMethod simplexMethod = new SimplexMethod(
				solutions: new() { 2, -1, 0, 0, 0, 0 },
				constraints: new() {
					new() {  -1,   2,  1,   0,  0,  0 },
					new() {   5,   2,  0,  -1,  0,  0 },
					new() {   4,  -3,  0,   0,  1,  0 },
					new() {   7,   4,  0,   0,  0,  1 },
				},
				resultsOfConstraints: new() { 4, 0, 12, 28 }
			);
			Console.WriteLine($"Результат вычислений: {simplexMethod.GetResult():f3}");

			//SimplexMethod simplexMethod = new SimplexMethod(
			//	solutions: new() { 4, 0, 12, 28, 0, 0 },
			//	constraints: new() {
			//					new() {  -1,   5,  4,   7,  -1,  0 },
			//					new() {   2,   2,  -3,  4,  0,  1 },
			//	},
			//	resultsOfConstraints: new() { 2, -1 }
			//);
			//Console.WriteLine($"Результат вычислений: {simplexMethod.GetResult():f3}");


			//SimplexMethod simplexMethod2 = new SimplexMethod(
			//	solutions: new() { 3, 2, 0, 0, 0 },
			//	constraints: new() {
			//		new() {  -4,   5,  1,   0,  0  },
			//		new() {   3,  -1,  0,   1,  0  },
			//		new() {   5,   2,  0,   0, -1  }
			//	},
			//	resultsOfConstraints: new() { 29, 14, 38 }
			//);
			//Console.WriteLine($"Результат вычислений: {simplexMethod2.GetResult():f3}");

			//SimplexMethod simplexMethod3 = new SimplexMethod(
			//   solutions: new() { 1, 1, 1, 0, 0, 0 },
			//   constraints: new() {
			//		new() {  -1,  3,   2,   1,  0,  0  },
			//		new() {   2,  4,   2,   0,  1,  0  },
			//		new() {   3,  2,  -1,   0,  0,  1  }
			//   },
			//   resultsOfConstraints: new() { 6, 8, 4 }
			//);

			//Console.WriteLine($"Результат вычислений: {simplexMethod3.GetResult():f3}");
		}
	}
}