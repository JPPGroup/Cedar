using JPP.StructuralAnalysis;
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
        public Dictionary<string, AreaBuildup> WallBuildups { get; }

        public List<AnalyticalFloor> Floors { get; }
        public List<AnalyticalWall> Walls { get; }

        public List<GridLine> HorizontalGrids { get; }
        public List<GridLine> VerticalGrids { get; }

        public GravityAnalysisModel()
        {
            AreaBuildups = new Dictionary<string, AreaBuildup>();
            WallBuildups = new Dictionary<string, AreaBuildup>();
            Floors = new List<AnalyticalFloor>();
            Walls = new List<AnalyticalWall>();

            HorizontalGrids = new List<GridLine>();
            VerticalGrids = new List<GridLine>();
        }
    }
}
