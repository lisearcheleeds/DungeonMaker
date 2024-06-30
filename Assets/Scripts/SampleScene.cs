using UnityEngine;

namespace DungeonWorld.Dungeon
{
    public class EntryPoint : MonoBehaviour
    {
        [ContextMenu("CreateDungeonData")]
        DungeonData CreateDungeonData()
        {
            return DungeonDataBuilder.CreateDungeonData(
                new DungeonBlueprintData(
                    mapSize: new Vector2Int(4, 4),
                    sectionSize: new Vector2Int(10, 10),
                    isEdgeStartPosition: true,
                    isEdgeEndPosition: true,
                    distortStrength: 15,
                    sectionMargin: 1,
                    extraReferencePositionCountRange: new Vector2Int(0, 1),
                    roomCountRange: new Vector2Int(0, 2),
                    roomExtraPassageCountRange: new Vector2Int(0, 1),
                    roomWidthRange: new Vector2Int(4, 6),
                    roomHeightRange: new Vector2Int(4, 6)
                ));
        }
    }
}