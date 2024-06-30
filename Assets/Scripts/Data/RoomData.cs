using UnityEngine;

namespace DungeonWorld.Dungeon
{
    public class RoomData
    {
        public Vector2Int RoomPosition { get; }
        public Vector2Int RoomSize { get; }   
        public int DungeonDepth { get; }

        public RoomData(Vector2Int roomPosition, Vector2Int roomSize, int dungeonDepth)
        {
            RoomPosition = roomPosition;
            RoomSize = roomSize;
            DungeonDepth = dungeonDepth;
        }
    }
}