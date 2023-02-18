using System.Collections.Generic;
using System.Linq;

namespace SimplexMethodWithGUI.Utilities
{
	class MaxDestination : IDestination
	{
		public double[] GetCoefficients(int countVariables, List<double> solutions)
			=> solutions.Take(countVariables).ToArray();
	}
}
