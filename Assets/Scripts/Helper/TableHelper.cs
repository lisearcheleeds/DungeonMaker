using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonWorld.Dungeon
{
    public static class TableHelper
    {
        public static int? MoveUpIndex(int index, Vector2Int tableSize) 
        {
            return 0 <= index - tableSize.x ? index - tableSize.x : null;
        }
        
        public static int? MoveDownIndex(int index, Vector2Int tableSize) 
        {
            return index + tableSize.x < tableSize.x * tableSize.y ? index + tableSize.x : null;
        }
        
        public static int? MoveRightIndex(int index, Vector2Int tableSize) 
        {
            return (index + 1) % tableSize.x != 0 ? index + 1 : null;
        }
        
        public static int? MoveLeftIndex(int index, Vector2Int tableSize) 
        {
            return index % tableSize.x != 0 ? index - 1 : null;
        }
        
        public static Vector2Int? MoveUpPosition(Vector2Int position, Vector2Int _) 
        {
            return position.y != 0 ? new Vector2Int(position.x, position.y - 1) : null;
        }
        
        public static Vector2Int? MoveDownPosition(Vector2Int position, Vector2Int tableSize) 
        {
            return position.y != tableSize.y - 1 ? new Vector2Int(position.x, position.y + 1) : null;
        }
        
        public static Vector2Int? MoveRightPosition(Vector2Int position, Vector2Int tableSize) 
        {
            return position.x != tableSize.x - 1 ? new Vector2Int(position.x + 1, position.y) : null;
        }
        
        public static Vector2Int? MoveLeftPosition(Vector2Int position, Vector2Int _) 
        {
            return position.x != 0 ? new Vector2Int(position.x - 1, position.y) : null;
        }

        /// <summary>
        /// width height平面上のランダムなindexを返す
        /// </summary>
        /// <param name="tableSize">tableSize</param>
        /// <param name="margin">margin</param>
        /// <returns>random index</returns>
        public static int GetRandomInnerIndex(Vector2Int tableSize, int margin)
        {
            var innerWidth = tableSize.x - margin * 2;
            var innerHeight = tableSize.y - margin * 2;
            var innerCount = innerWidth * innerHeight;
            var innerRandomIndex = Random.Range(0, innerCount);

            var rowCount = innerRandomIndex / innerWidth;
            var offset = margin * tableSize.x + margin + margin * rowCount * 2;
            
            return innerRandomIndex + offset;
        }

        /// <summary>
        /// width height平面上の側面のランダムなindexを返す
        /// </summary>
        /// <param name="tableSize">tableSize</param>
        /// <returns>random point</returns>
        public static int GetRandomEdgeIndex(Vector2Int tableSize)
        {
            var edgeCount = tableSize.x * 2 + tableSize.y * 2 - 4;
            var startEdgeSectionIndex = Random.Range(0, edgeCount);

            if (startEdgeSectionIndex < tableSize.x)
            {
                // 上段
                return startEdgeSectionIndex;
            }
            
            if (startEdgeSectionIndex < tableSize.x * 2)
            {
                // 下段
                return tableSize.x * (tableSize.y - 1) + startEdgeSectionIndex - tableSize.x;
            }
            
            var offset = startEdgeSectionIndex - tableSize.x * 2;
            if (offset <= tableSize.y - 2)
            {
                // 左
                return tableSize.x * (offset + 1);
            }

            // 右
            offset -= tableSize.y - 2;
            return tableSize.x * (offset + 1) + tableSize.x - 1;
        }
        
        /// <summary>
        /// width height平面上でstartからendまで移動する時のindex配列を返す
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="end">end</param>
        /// <param name="tableSize">tableSize</param>
        /// <returns>path</returns>
        public static int[] GetFastPath(int start, int end, Vector2Int tableSize)
        {
            var result = new List<int>();
            result.Add(start);

            var endPos = GetPositionFromIndex(end, tableSize);

            while (result.Last() != end)
            {
                var current = result.Last();
                var currentPos = GetPositionFromIndex(current, tableSize);

                if (Mathf.Abs(currentPos.y - endPos.y) < Mathf.Abs(currentPos.x - endPos.x))
                {
                    if (currentPos.x < endPos.x)
                    {
                        result.Add(MoveRightIndex(current, tableSize).Value);
                    }
                    else
                    {
                        result.Add(MoveLeftIndex(current, tableSize).Value);
                    }
                }
                else
                {
                    if (currentPos.y < endPos.y)
                    {
                        result.Add(MoveDownIndex(current, tableSize).Value);
                    }
                    else
                    {
                        result.Add(MoveUpIndex(current, tableSize).Value);
                    }
                }
            }

            return result.ToArray();
        }
        
        /// <summary>
        /// pathの両端を維持しながらpathを歪めたindex配列を返す
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="strength">strength</param>
        /// <param name="tableSize">tableSize</param>
        /// <returns>distorted path</returns>
        public static int[] GetRandomDistortPath(int[] path, int strength, Vector2Int tableSize)
        {
            List<int> result = new List<int>(path);

            for (var strengthCount = 0; strengthCount < strength; strengthCount++)
            {
                for (var i = 0; i < result.Count - 1; i++)
                {
                    var current = result[i];
                    var next = result[i + 1];
                    int[] insertIndexes = null;
                    
                    // 回り込めるなら回り込むようにpathを上書きする
                    if (MoveUpIndex(result[i], tableSize) == next)
                    {
                        insertIndexes = Random.Range(0, 2) == 0
                            ? GetDistortIndexes(current, next, result, MoveRightIndex, MoveUpIndex, MoveLeftIndex, tableSize)
                            : GetDistortIndexes(current, next, result, MoveLeftIndex, MoveUpIndex, MoveRightIndex, tableSize);
                    }
                    else if (MoveDownIndex(result[i], tableSize) == next)
                    {
                        insertIndexes = Random.Range(0, 2) == 0
                            ? GetDistortIndexes(current, next, result, MoveLeftIndex, MoveDownIndex, MoveRightIndex, tableSize)
                            : GetDistortIndexes(current, next, result, MoveRightIndex, MoveDownIndex, MoveLeftIndex, tableSize);
                    }
                    else if (MoveRightIndex(current, tableSize) == next)
                    {
                        insertIndexes = Random.Range(0, 2) == 0
                            ? GetDistortIndexes(current, next, result, MoveDownIndex, MoveRightIndex, MoveUpIndex, tableSize)
                            : GetDistortIndexes(current, next, result, MoveUpIndex, MoveRightIndex, MoveDownIndex, tableSize);
                    }
                    else if (MoveLeftIndex(current, tableSize) == next)
                    {
                        insertIndexes = Random.Range(0, 2) == 0
                            ? GetDistortIndexes(current, next, result, MoveUpIndex, MoveLeftIndex, MoveDownIndex, tableSize)
                            : GetDistortIndexes(current, next, result, MoveDownIndex, MoveLeftIndex, MoveUpIndex, tableSize);
                    }
                    
                    if (insertIndexes != null)
                    {
                        result.InsertRange(i + 1, insertIndexes);
                        i += insertIndexes.Length;
                    }
                }
            }

            return result.ToArray();
            
            static int[] GetDistortIndexes(
                int current,
                int next,
                List<int> path, 
                Func<int, Vector2Int, int?> checker1,
                Func<int, Vector2Int, int?> checker2,
                Func<int, Vector2Int, int?> checker3,
                Vector2Int tableSize)
            {
                var res1 = checker1(current, tableSize);
                if (!res1.HasValue || path.Contains(res1.Value))
                {
                    return null;
                }
                
                var res2 = checker2(res1.Value, tableSize);
                if (!res2.HasValue || path.Contains(res2.Value))
                {
                    return null;
                }
                
                var res3 = checker3(res2.Value, tableSize);
                if (!res3.HasValue || next != res3.Value)
                {
                    return null;
                }

                return new[] { res1.Value, res2.Value };
            }
        }
        
        /// <summary>
        /// indexから位置を取得する
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="tableSize">tableSize</param>
        /// <returns>position</returns>
        public static Vector2Int GetPositionFromIndex(int index, Vector2Int tableSize)
        {
            return new Vector2Int(index % tableSize.x, index / tableSize.x);
        }

        /// <summary>
        /// Section内のIndexをTile indexを取得する
        /// </summary>
        /// <param name="tileIndexInSection">tileIndexInSection</param>
        /// <param name="sectionIndex">sectionIndex</param>
        /// <param name="sectionSize">sectionSize</param>
        /// <param name="mapSize">mapSize</param>
        /// <param name="tileSize">tileSize</param>
        /// <returns>tile index</returns>
        public static int GetTileIndex(
            int tileIndexInSection,
            int sectionIndex,
            Vector2Int sectionSize,
            Vector2Int mapSize,
            Vector2Int tileSize)
        {
            var sectionBaseIndex = sectionSize.x * (sectionIndex % mapSize.x)
                                   + (sectionSize.x * sectionSize.y) * ((sectionIndex / mapSize.y) * mapSize.y);
            var tileIndexOffset = tileIndexInSection % sectionSize.x
                                  + (tileIndexInSection / sectionSize.x) * tileSize.x;
                
            return sectionBaseIndex + tileIndexOffset;
        }

        public static Vector2Int[] GetElbowConnectPositions(
            Vector2Int fromPosition,
            Vector2Int toPosition,
            bool isXVector,
            Vector2Int tileSize,
            int extraCornerCount)
        {
            var result = new List<Vector2Int>();
            result.Add(fromPosition);
            
            var basePosition = fromPosition;
            var targetPosition = toPosition;

            for (var i = 0; i < 2; i++)
            {
                var offsetPosition = targetPosition - basePosition;
                var offsetCount = isXVector ? offsetPosition.x : offsetPosition.y;
            
                Func<Vector2Int, Vector2Int, Vector2Int?> moveFunc = (isXVector, (int)Mathf.Sign(offsetCount)) switch
                {
                    (false, -1) => MoveUpPosition,
                    (false, 1) => MoveDownPosition,
                    (true, 1) => MoveRightPosition,
                    (true, -1) => MoveLeftPosition,
                    _ => null,
                };

                if (moveFunc == null)
                {
                    continue;
                }

                for (var step = 0; step < Mathf.Abs(offsetCount); step++)
                {
                    result.Add(moveFunc(result.Last(), tileSize).Value);
                }

                basePosition = result.Last();
                isXVector = !isXVector;
            }

            return result.ToArray();
        }
    }
}