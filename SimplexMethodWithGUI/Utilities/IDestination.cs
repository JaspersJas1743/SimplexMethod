using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexMethodWithGUI.Utilities
{
    public interface IDestination
    {
        double[] GetCoefficients(int countVariables, List<double> solutions);
    }
}
