using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonWorld.Dungeon
{
    public static class DungeonDataBuilder
    {
        public static DungeonData CreateDungeonData(DungeonBlueprintData dungeonBlueprintData)
        {
            var mapSizeCount = dungeonBlueprintData.MapSize.x * dungeonBlueprintData.MapSize.y;
            
            var startSectionIndex = dungeonBlueprintData.IsEdgeStartPosition
                ? TableHelper.GetRandomEdgeIndex(dungeonBlueprintData.MapSize)
                : Random.Range(0, mapSizeCount);

            var endSectionIndex = dungeonBlueprintData.IsEdgeEndPosition
                ? TableHelper.GetRandomEdgeIndex(dungeonBlueprintData.MapSize)
                : Random.Range(0, mapSizeCount);

            // ダブり防止
            endSectionIndex = startSectionIndex == endSectionIndex ? (endSectionIndex + 1) % mapSizeCount : endSectionIndex;

            Debug.Log($"startSectionIndex:{startSectionIndex} endSectionIndex:{endSectionIndex}");

            var sectionPath = TableHelper.GetFastPath(
                startSectionIndex,
                endSectionIndex,
                dungeonBlueprintData.MapSize);

            sectionPath = TableHelper.GetRandomDistortPath(
                sectionPath,
                dungeonBlueprintData.DistortStrength,
                dungeonBlueprintData.MapSize);

            var sectionData = CreateSectionData(sectionPath, dungeonBlueprintData);
            var roomData = CreateRoomData(sectionData, dungeonBlueprintData);
            var tileData = CreateTileData(sectionData, roomData, dungeonBlueprintData);

            var dungeonData = new DungeonData(tileData, sectionData, roomData);

            ShowLogSection(sectionPath, dungeonBlueprintData.MapSize.x, dungeonBlueprintData.MapSize.y);
            ShowLogTile(tileData);
            
            return dungeonData;
        }

        static SectionData[] CreateSectionData(int[] path, DungeonBlueprintData dungeonBlueprintData)
        {
            var result = new List<SectionData>();
            
            for (var i = 0; i < path.Length; i++)
            {
                var prev = i != 0 ? path[i - 1] : (int?)null;
                var next = i != path.Length - 1 ? path[i + 1] : (int?)null;

                var movedUp = TableHelper.MoveUpIndex(path[i], dungeonBlueprintData.MapSize);
                var movedDown = TableHelper.MoveDownIndex(path[i], dungeonBlueprintData.MapSize);
                var movedRight = TableHelper.MoveRightIndex(path[i], dungeonBlueprintData.MapSize);
                var movedLeft = TableHelper.MoveLeftIndex(path[i], dungeonBlueprintData.MapSize);

                var prevVector = prev switch
                {
                    null => default,
                    _ when prev == movedUp => Vector2Int.up,
                    _ when prev == movedDown => Vector2Int.down,
                    _ when prev == movedRight => Vector2Int.right,
                    _ when prev == movedLeft => Vector2Int.left,
                    _ => default,
                };
                
                var nextVector = next switch
                {
                    null => default,
                    _ when next == movedUp => Vector2Int.up,
                    _ when next == movedDown => Vector2Int.down,
                    _ when next == movedRight => Vector2Int.right,
                    _ when next == movedLeft => Vector2Int.left,
                    _ => default,
                };

                var sectionPosition = TableHelper.GetPositionFromIndex(path[i], dungeonBlueprintData.MapSize);

                var referenceTileIndexes = Enumerable.Range(0, 1 + Random.Range(
                        dungeonBlueprintData.ExtraReferencePositionCountRange.x,
                        dungeonBlueprintData.ExtraReferencePositionCountRange.y))
                    .Select(_ => TableHelper.GetRandomInnerIndex(
                        dungeonBlueprintData.SectionSize,
                        dungeonBlueprintData.SectionMargin)).ToArray();
                
                var referenceTilePositionsInMap = referenceTileIndexes
                    .Select(index => 
                        TableHelper.GetPositionFromIndex(
                            TableHelper.GetTileIndex(
                                index,
                                path[i],
                                dungeonBlueprintData.SectionSize,
                                dungeonBlueprintData.MapSize,
                                dungeonBlueprintData.TileSize),
                            dungeonBlueprintData.TileSize))
                    .ToArray();
                
                result.Add(new SectionData(
                    sectionIndex: path[i],
                    mainRoutePathIndex: i,
                    sectionPosition: sectionPosition,
                    prevSectionVector: prevVector,
                    nextSectionVector: nextVector,
                    referenceTilePositionsInMap: referenceTilePositionsInMap));
            }

            return result.ToArray();
        }

        static RoomData[] CreateRoomData(SectionData[] sectionDataList, DungeonBlueprintData dungeonBlueprintData)
        {
            return sectionDataList.SelectMany(sectionData =>
            {
                return sectionData.ReferenceTilePositionsInMap
                    .Take(Random.Range(dungeonBlueprintData.RoomCountRange.x, dungeonBlueprintData.RoomCountRange.y))
                    .Select(roomPosition =>
                    {
                        var roomWidth = Random.Range(dungeonBlueprintData.RoomWidthRange.x, dungeonBlueprintData.RoomWidthRange.y);
                        var roomHeight = Random.Range(dungeonBlueprintData.RoomHeightRange.x, dungeonBlueprintData.RoomHeightRange.y);
                        
                        return new RoomData(
                            roomPosition,
                            new Vector2Int(roomWidth, roomHeight),
                            sectionData.MainRoutePathIndex);
                    }).ToArray();
            }).ToArray();
        }

        static TileData[,] CreateTileData(SectionData[] sectionDataList, RoomData[] roomDataList, DungeonBlueprintData dungeonBlueprintData)
        {
            var result = new TileData[dungeonBlueprintData.TileSize.x, dungeonBlueprintData.TileSize.y];

            // 通路作成
            for (var i = 0; i < sectionDataList.Length - 1; i++)
            {
                var sectionConnectPositions = TableHelper.GetElbowConnectPositions(
                    sectionDataList[i].ReferenceTilePositionsInMap[0],
                    sectionDataList[i + 1].ReferenceTilePositionsInMap[0],
                    sectionDataList[i].NextSectionVector.Value.x != 0,
                    dungeonBlueprintData.TileSize,
                    0);

                SetTileData(sectionConnectPositions, sectionDataList[i].MainRoutePathIndex);

                for (var t = 0; t < sectionDataList[i].ReferenceTilePositionsInMap.Length - 1; t++)
                {
                    var innerRoomConnectPositions = TableHelper.GetElbowConnectPositions(
                        sectionDataList[i].ReferenceTilePositionsInMap[t],
                        sectionDataList[i].ReferenceTilePositionsInMap[t + 1],
                        sectionDataList[i].NextSectionVector.Value.x != 0,
                        dungeonBlueprintData.TileSize,
                        0);
                    
                    SetTileData(innerRoomConnectPositions, sectionDataList[i].MainRoutePathIndex);
                }
                
                void SetTileData(Vector2Int[] positions, int dungeonDepth)
                {
                    for (var t = 0; t < positions.Length; t++)
                    {
                        var tileType = TileType.Passage;
                        if (i == 0 && t == 0)
                        {
                            tileType = TileType.Start;
                        }
                        else if (i == sectionDataList.Length - 2 && t == positions.Length - 1)
                        {
                            tileType = TileType.End;
                        }
                        else if (sectionDataList[i].ReferenceTilePositionsInMap.Any(x => x == positions[t])
                                 || sectionDataList[i + 1].ReferenceTilePositionsInMap.Any(x => x == positions[t]))
                        {
                            tileType = TileType.Reference;
                        }
                    
                        result[positions[t].x, positions[t].y] = new TileData(
                            positions[t],
                            tileType,
                            dungeonDepth);
                    }                    
                }
            }
            
            // 部屋作成
            foreach (var roomData in roomDataList)
            {
                for (var x = 0; x < roomData.RoomSize.x; x++)
                {
                    var posX = roomData.RoomPosition.x - roomData.RoomSize.x / 2 + x;
                    if (posX < 0 || dungeonBlueprintData.TileSize.x <= posX)
                    {
                        continue;
                    }

                    for (var y = 0; y < roomData.RoomSize.y; y++)
                    {
                        var posY = roomData.RoomPosition.y - roomData.RoomSize.y / 2 + y;
                        if (posY < 0 || dungeonBlueprintData.TileSize.y <= posY)
                        {
                            continue;
                        }

                        if (result[posX, posY] == null)
                        {
                            result[posX, posY] = new TileData(
                                new Vector2Int(posX, posY),
                                TileType.Room,
                                roomData.DungeonDepth);
                        }
                    }
                }
            }

            return result;
        }

        static void ShowLogSection(int[] path, int width, int height)
        {
            var logText = "";
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (path.Any(i => i == x + y * width))
                    {
                        var stepIndex = 0;
                        for (var i = 0; i < path.Length; i++)
                        {
                            if (path[i] == x + y * width)
                            {
                                stepIndex = i;
                                break;
                            }
                        }
                        
                        if (stepIndex == 0)
                        {
                            logText += " S ";
                        }
                        else if (stepIndex == path.Length - 1)
                        {
                            logText += " E ";
                        }
                        else
                        {
                            var current = path[stepIndex];
                            var prev = path[stepIndex - 1];
                            var next = path[stepIndex + 1];
                            
                            if (prev == current - 1 && next == current + 1
                                || next == current - 1 && prev == current + 1)
                            {
                                logText += " ━ ";
                            }
                            else if (prev == current - width && next == current + width
                                     || next == current - width && prev == current + width)
                            {
                                logText += " ┃ ";
                            }else if (prev == current + width && next == current + 1
                                      || next == current + width && prev == current + 1)
                            {
                                logText += " ┏ ";
                            }else if (prev == current + width && next == current - 1
                                      || next == current + width && prev == current - 1)
                            {
                                logText += " ┓ ";
                            }else if (prev == current - width && next == current + 1
                                      || next == current - width && prev == current + 1)
                            {
                                logText += " ┗ ";
                            }else if (prev == current - width && next == current - 1
                                      || next == current - width && prev == current - 1)
                            {
                                logText += " ┛ ";
                            }
                        }
                    }
                    else
                    {
                        logText += " ▢ ";
                    }
                }
                
                logText += "\n";
            }

            Debug.Log(logText);
        }

        static void ShowLogTile(TileData[,] tileData)
        {
            var logText = "";
            
            for (var y = 0; y < tileData.GetLength(1); y++)
            {
                for (var x = 0; x < tileData.GetLength(0); x++)
                {
                    switch (tileData[x, y]?.TileType)
                    {
                        case TileType.Passage:
                        case TileType.Room:
                            logText += " ■ ";
                            break;
                        case TileType.Start:
                            logText += " S ";
                            break;
                        case TileType.End:
                            logText += " E ";
                            break;
                        case TileType.Reference:
                            logText += " ☆ ";
                            break;
                        default:
                            logText += " □ ";
                            break;
                    }
                }
                
                logText += "\n";
            }

            Debug.Log(logText);
        }
    }
}