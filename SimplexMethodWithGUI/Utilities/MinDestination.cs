using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexMethodWithGUI.Utilities
{
    class MinDestination: IDestination
    {
		public double[] GetCoefficients(int countVariables, List<double> solutions)
			=> solutions.Skip(countVariables).Select(x => Math.Abs(x)).ToArray();
	}
}
