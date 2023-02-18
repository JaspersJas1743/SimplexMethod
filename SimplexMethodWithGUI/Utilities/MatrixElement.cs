namespace SimplexMethodWithGUI.Utilities
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
}
