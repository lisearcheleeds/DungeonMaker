using System.Linq;
using UnityEngine;

namespace DungeonWorld.Dungeon
{
    public class SectionData
    {
        /// <summary>Index</summary>
        public int SectionIndex { get; }
        
        /// <summary>StartからEndへのパスのIndex</summary>
        public int MainRoutePathIndex { get; }

        /// <summary>Section内の位置</summary>
        public Vector2Int SectionPosition { get; }
        
        /// <summary>次のSectionへの方向</summary>
        public Vector2Int? PrevSectionVector { get; }
        
        /// <summary>前のSectionへの方向</summary>
        public Vector2Int? NextSectionVector { get; }
        
        /// <summary>Section内の基準PositionのTile</summary>
        public Vector2Int[] ReferenceTilePositionsInMap { get; }

        public SectionData(
            int sectionIndex,
            int mainRoutePathIndex,
            Vector2Int sectionPosition,
            Vector2Int? prevSectionVector,
            Vector2Int? nextSectionVector,
            Vector2Int[] referenceTilePositionsInMap)
        {
            SectionIndex = sectionIndex;
            MainRoutePathIndex = mainRoutePathIndex;
            SectionPosition = sectionPosition;
            PrevSectionVector = prevSectionVector;
            NextSectionVector = nextSectionVector;
            ReferenceTilePositionsInMap = referenceTilePositionsInMap;
        }
    }
}