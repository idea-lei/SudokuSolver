using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.DataType
{
    public class AssumptionNode
    {
        public Unit Unit { get; init; }
        public Unit? Parent { get; init; }
        public HashSet<Unit> Children { get; } = new();
        public AssumptionNode(Unit unit) => Unit = unit;
    }
}
