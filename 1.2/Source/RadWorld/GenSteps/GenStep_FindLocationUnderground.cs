using System;
using Verse;

namespace RadWorld
{
	public class GenStep_FindLocationUnderground : GenStep
	{
		public override int SeedPart
		{
			get
			{
				return 820815231;
			}
		}

		public override void Generate(Map map, GenStepParams parms)
		{
			Log.Message("GenStep_FindLocationUnderground");
			DeepProfiler.Start("RebuildAllRegions");
			map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();
			DeepProfiler.End();
			MapGenerator.PlayerStartSpot = CellFinderLoose.TryFindCentralCell(map, 7, map.AllCells.EnumerableCount(), (IntVec3 x) => x.Walkable(map));
		}
	}
}

