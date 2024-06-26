using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.StructuralAnalysis.Models
{
    public class GravityAnalysisModel
    {
        public Dictionary<string, AreaBuildup> AreaBuildups { get; }

        public GravityAnalysisModel()
        {
            AreaBuildups = new Dictionary<string, AreaBuildup>();
        }
    }
}
