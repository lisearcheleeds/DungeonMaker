using UnityEngine;

namespace DungeonWorld.Dungeon
{
    public class TileData
    {
        public Vector2Int Position { get; }
        public TileType TileType { get; }
        public int DungeonDepth { get; }

        public TileData(Vector2Int position, TileType tileType, int dungeonDepth)
        {
            Position = position;
            TileType = tileType;
            DungeonDepth = dungeonDepth;
        }
    }
}