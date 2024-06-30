using System.Collections.Generic;
using System.Linq;

namespace DungeonWorld.Dungeon
{
    public class DungeonData
    {
        public TileData[,] TileData { get; }
        public SectionData[] SectionData { get; }
        public RoomData[] RoomData { get; }

        public TileData StartTileData { get; }
        public TileData EndTileData { get; }

        public TileData[] ReferenceTiles { get; }
        public TileData LastReferenceTile { get; }

        public DungeonData(
            TileData[,] tileData,
            SectionData[] sectionData,
            RoomData[] roomData)
        {
            TileData = tileData;
            SectionData = sectionData;
            RoomData = roomData;

            var referenceTiles = new List<TileData>();
            
            foreach (var data in tileData)
            {
                if (data == null)
                {
                    continue;
                }

                if (data.TileType == TileType.Start)
                {
                    StartTileData = data;
                }
                else if (data.TileType == TileType.End)
                {
                    EndTileData = data;
                }
                else if (data.TileType == TileType.Reference)
                {
                    referenceTiles.Add(data);
                }
            }

            ReferenceTiles = referenceTiles.ToArray();
            LastReferenceTile = referenceTiles.OrderByDescending(x => x.DungeonDepth).First();
        }
    }
}