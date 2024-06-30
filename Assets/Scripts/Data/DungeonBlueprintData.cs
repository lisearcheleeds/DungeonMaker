using UnityEngine;

namespace DungeonWorld.Dungeon
{
    public class DungeonBlueprintData
    {
        public Vector2Int TileSize { get; }

        public Vector2Int MapSize { get; }
        public Vector2Int SectionSize { get; }

        public bool IsEdgeStartPosition { get; }
        public bool IsEdgeEndPosition { get; }
        
        public int DistortStrength { get; }

        public int SectionMargin { get; }

        public Vector2Int ExtraReferencePositionCountRange { get; }
        
        public Vector2Int RoomCountRange { get; }
        public Vector2Int RoomExtraPassageCountRange { get; }
        public Vector2Int RoomWidthRange { get; }
        public Vector2Int RoomHeightRange { get; }

        public DungeonBlueprintData(
            Vector2Int mapSize,
            Vector2Int sectionSize,
            bool isEdgeStartPosition,
            bool isEdgeEndPosition,
            int distortStrength,
            int sectionMargin,
            Vector2Int extraReferencePositionCountRange,
            Vector2Int roomCountRange,
            Vector2Int roomExtraPassageCountRange,
            Vector2Int roomWidthRange,
            Vector2Int roomHeightRange)
        {
            MapSize = mapSize;
            SectionSize = sectionSize;
            IsEdgeStartPosition = isEdgeStartPosition;
            IsEdgeEndPosition = isEdgeEndPosition;
            DistortStrength = distortStrength;
            SectionMargin = sectionMargin;
            ExtraReferencePositionCountRange = extraReferencePositionCountRange;
            RoomCountRange = roomCountRange;
            RoomExtraPassageCountRange = roomExtraPassageCountRange;
            RoomWidthRange = roomWidthRange;
            RoomHeightRange = roomHeightRange;

            TileSize = new Vector2Int(MapSize.x * SectionSize.x, MapSize.y * SectionSize.y);
        }
    }
}