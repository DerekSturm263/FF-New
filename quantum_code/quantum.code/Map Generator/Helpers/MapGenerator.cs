using Quantum.Types;
using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    static unsafe class MapGenerator
    {
        public static Map GenerateMapFromStage(Stage stage, IAssetSerializer assetSerializer, Map sourceMap)
        {
            Map map = CloneMap(assetSerializer, sourceMap);
            List<MapStaticCollider2D> staticColliders = new(map.StaticColliders2D);
            List<EntityPrototypeContainer> entities = new(sourceMap.MapEntities);

            for (int i = 0; i < 8; ++i)
            {
                StaticColliderInfo collider = ArrayHelper.All(stage.Objects.Colliders)[i];
                if (collider.Equals(default(StaticColliderInfo)))
                    continue;

                MapStaticCollider2D staticInfo = AddStaticCollider(collider, i);
                staticColliders.Add(staticInfo);
            }

            map.WorldSize = 256;
            map.StaticColliders2D = staticColliders.ToArray();
            map.MapEntities = entities.ToArray();

            return map;
        }

        private static MapStaticCollider2D AddStaticCollider(StaticColliderInfo colliderInfo, int index)
        {
            return new()
            {
                Rotation = colliderInfo.Rotation,
                Position = colliderInfo.Position,
                Height = colliderInfo.Height,
                VerticalOffset = colliderInfo.VerticalOffset,
                PhysicsMaterial = colliderInfo.PhysicsMaterial,
                Layer = colliderInfo.Layer,
                CircleRadius = colliderInfo.CircleRadius,
                BoxExtents = colliderInfo.BoxExtents,
                EdgeExtent = colliderInfo.EdgeExtent,
                ShapeType = Shape2DType.Box,
                StaticData = new()
                {
                    ColliderIndex = index,
                    MutableMode = PhysicsCommon.StaticColliderMutableMode.Immutable
                }
            };
        }

        public static Map CloneMap(IAssetSerializer assetSerializer, Map source)
        {
            var data = assetSerializer.SerializeAssets(new[] { source });
            var assets = assetSerializer.DeserializeAssets(data);
            var newMap = assets.First() as Map;

            newMap.Guid = default;
            
            return newMap;
        }
    }
}
