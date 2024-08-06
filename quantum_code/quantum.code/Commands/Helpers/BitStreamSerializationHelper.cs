using Photon.Deterministic;

namespace Quantum
{
    public static class BitStreamSerializationHelper
    {
        #region Build

        public static void Serialize(this IBitStream stream, ref Build value)
        {
            stream.Serialize(ref value.Equipment);
            stream.Serialize(ref value.Cosmetics);
        }

        public static void Serialize(this IBitStream stream, ref EquipmentSettings value)
        {
            stream.Serialize(ref value.Weapons);
            stream.Serialize(ref value.Ultimate);
            stream.Serialize(ref value.Badge);
            stream.Serialize(ref value.Outfit);
        }

        public static void Serialize(this IBitStream stream, ref WeaponSet value)
        {
            stream.Serialize(ref value.MainWeapon);
            stream.Serialize(ref value.AltWeapon);
            stream.Serialize(ref value.SubWeapon);
        }

        public static void Serialize(this IBitStream stream, ref Weapon value)
        {
            stream.Serialize(ref value.Template);
            stream.Serialize(ref value.Material);
            stream.Serialize(ref value.Enhancer);
            stream.Serialize(ref value.FileGuid);
        }

        public static void Serialize(this IBitStream stream, ref Sub value)
        {
            stream.Serialize(ref value.Template);
            stream.Serialize(ref value.Enhancer);
            stream.Serialize(ref value.FileGuid);
        }

        public static void Serialize(this IBitStream stream, ref Outfit value)
        {
            stream.Serialize(ref value.Headgear);
            stream.Serialize(ref value.Clothing);
            stream.Serialize(ref value.Legwear);
        }

        public static void Serialize(this IBitStream stream, ref Apparel value)
        {
            stream.Serialize(ref value.Template);
            stream.Serialize(ref value.Modifiers);
            stream.Serialize(ref value.Color);
            stream.Serialize(ref value.Stickers);
            stream.Serialize(ref value.FileGuid);
        }

        public static void Serialize(this IBitStream stream, ref ApparelModifierSet value)
        {
            stream.Serialize(ref value.Modifier1);
            stream.Serialize(ref value.Modifier2);
            stream.Serialize(ref value.Modifier3);
        }

        public static void Serialize(this IBitStream stream, ref ArrayAngularStickers value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
            stream.Serialize(ref value.Item5);
            stream.Serialize(ref value.Item6);
            stream.Serialize(ref value.Item7);
            stream.Serialize(ref value.Item8);
            stream.Serialize(ref value.Item9);
            stream.Serialize(ref value.Item10);
            stream.Serialize(ref value.Item11);
            stream.Serialize(ref value.Item12);
            stream.Serialize(ref value.Item13);
            stream.Serialize(ref value.Item14);
            stream.Serialize(ref value.Item15);
            stream.Serialize(ref value.Item16);
        }

        public static void Serialize(this IBitStream stream, ref AngularSticker value)
        {
            stream.Serialize(ref value.Sticker);
            stream.Serialize(ref value.Angle);
        }

        public static void Serialize(this IBitStream stream, ref CosmeticSettings value)
        {
            stream.Serialize(ref value.Avatar);
            stream.Serialize(ref value.Eyes);
            stream.Serialize(ref value.Hair);
            stream.Serialize(ref value.Voice);
            stream.Serialize(ref value.Emotes);
        }

        public static void Serialize(this IBitStream stream, ref AvatarColorBinding value)
        {
            stream.Serialize(ref value.Avatar);
            stream.Serialize(ref value.Color);
        }

        public static void Serialize(this IBitStream stream, ref EyesColorBinding value)
        {
            stream.Serialize(ref value.Eyes);
            stream.Serialize(ref value.Color);
        }

        public static void Serialize(this IBitStream stream, ref HairColorBinding value)
        {
            stream.Serialize(ref value.Hair);
            stream.Serialize(ref value.Color);
        }

        public static void Serialize(this IBitStream stream, ref DirectionalEmote value)
        {
            stream.Serialize(ref value.Up);
            stream.Serialize(ref value.Down);
            stream.Serialize(ref value.Left);
            stream.Serialize(ref value.Right);
        }

        public static void Serialize(this IBitStream stream, ref EmoteMessageBinding value)
        {
            stream.Serialize(ref value.Emote);
            stream.Serialize(ref value.Message);
        }

        #endregion

        #region Stage

        public static void Serialize(this IBitStream stream, ref Stage value)
        {
            stream.Serialize(ref value.Theme);
            stream.Serialize(ref value.Objects);
            stream.Serialize(ref value.Spawn);
        }

        public static void Serialize(this IBitStream stream, ref ThemeSettings value)
        {
            stream.Serialize(ref value.CameraSettings);
            stream.Serialize(ref value.Track);
            stream.Serialize(ref value.FogColor);
            stream.Serialize(ref value.FogDensity);
        }

        public static void Serialize(this IBitStream stream, ref CameraSettingsGroup value)
        {
            stream.Serialize(ref value.Default);
            stream.Serialize(ref value.Zoom);
            stream.Serialize(ref value.Tension);
            stream.Serialize(ref value.Winner);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ObjectSettings value)
        {
            stream.Serialize(ref value.Stage);
            stream.Serialize(ref value.SourceMap);
            stream.Serialize(ref value.Colliders);
            stream.Serialize(ref value.Gizmos);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ArrayStaticColliderInfo value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
            stream.Serialize(ref value.Item5);
            stream.Serialize(ref value.Item6);
            stream.Serialize(ref value.Item7);
            stream.Serialize(ref value.Item8);
        }

        public static unsafe void Serialize(this IBitStream stream, ref StaticColliderInfo value)
        {
            stream.Serialize(ref value.Rotation);
            stream.Serialize(ref value.Position);
            stream.Serialize(ref value.Height);
            stream.Serialize(ref value.VerticalOffset);
            stream.Serialize(ref value.PhysicsMaterial);
            stream.Serialize(ref value.Layer);
            stream.Serialize(ref value.CircleRadius);
            stream.Serialize(ref value.BoxExtents);
            stream.Serialize(ref value.EdgeExtent);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ArrayPositionalGizmoInfo value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
            stream.Serialize(ref value.Item5);
            stream.Serialize(ref value.Item6);
            stream.Serialize(ref value.Item7);
            stream.Serialize(ref value.Item8);
            stream.Serialize(ref value.Item9);
            stream.Serialize(ref value.Item10);
            stream.Serialize(ref value.Item11);
            stream.Serialize(ref value.Item12);
            stream.Serialize(ref value.Item13);
            stream.Serialize(ref value.Item14);
            stream.Serialize(ref value.Item15);
            stream.Serialize(ref value.Item16);
        }

        public static unsafe void Serialize(this IBitStream stream, ref PositionalGizmo value)
        {
            stream.Serialize(ref value.Gizmo);
            stream.Serialize(ref value.Position);
        }

        public static unsafe void Serialize(this IBitStream stream, ref SpawnSettings value)
        {
            stream.Serialize(ref value.PlayerSpawnPoints);
            stream.Serialize(ref value.ItemSpawnPoints);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ArrayPlayerSpawnPoints value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ArrayItemSpawnPoints value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
            stream.Serialize(ref value.Item5);
            stream.Serialize(ref value.Item6);
            stream.Serialize(ref value.Item7);
            stream.Serialize(ref value.Item8);
            stream.Serialize(ref value.Item9);
            stream.Serialize(ref value.Item10);
            stream.Serialize(ref value.Item11);
            stream.Serialize(ref value.Item12);
            stream.Serialize(ref value.Item13);
            stream.Serialize(ref value.Item14);
            stream.Serialize(ref value.Item15);
            stream.Serialize(ref value.Item16);
        }

        #endregion

        #region Ruleset

        public static void Serialize(this IBitStream stream, ref Ruleset value)
        {
            stream.Serialize(ref value.Match);
            stream.Serialize(ref value.Players);
            stream.Serialize(ref value.Stage);
            stream.Serialize(ref value.Items);
        }

        public static void Serialize(this IBitStream stream, ref MatchSettings value)
        {
            stream.Serialize(ref value.WinCondition);
            stream.Serialize(ref value.Time);
            stream.Serialize(ref value.ShowTimer);
            stream.Serialize(ref value.MatchCount);
            stream.Serialize(ref value.EndMatchesWhenWinnerClear);
            stream.Serialize(ref value.ShowScores);
            stream.Serialize(ref value.ShowCurrentWinner);
        }

        public static void Serialize(this IBitStream stream, ref PlayerSettings value)
        {
            stream.Serialize(ref value.StockCount);
            stream.Serialize(ref value.MaxHealth);
            stream.Serialize(ref value.MaxEnergy);
            stream.Serialize(ref value.EnergyChargeRate);
            stream.Serialize(ref value.RespawnTime);
            stream.Serialize(ref value.FriendlyFire);
            stream.Serialize(ref value.DisallowDuplicateSelection);
        }

        public static unsafe void Serialize(this IBitStream stream, ref StageSettings value)
        {
            stream.Serialize(ref value.Stages);
            stream.Serialize(ref value.StagePicker);
            stream.Serialize(ref value.AllowGizmos);
            stream.Serialize(ref value.AllowCustomStages);
            stream.Serialize(ref value.DisallowDuplicateSelection);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ArrayStages value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
            stream.Serialize(ref value.Item5);
            stream.Serialize(ref value.Item6);
            stream.Serialize(ref value.Item7);
            stream.Serialize(ref value.Item8);
            stream.Serialize(ref value.Item9);
            stream.Serialize(ref value.Item10);
            stream.Serialize(ref value.Item11);
            stream.Serialize(ref value.Item12);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ItemSettings value)
        {
            stream.Serialize(ref value.StartingItem);
            stream.Serialize(ref value.Items);
            stream.Serialize(ref value.SpawnFrequency);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ArrayItems value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
            stream.Serialize(ref value.Item5);
            stream.Serialize(ref value.Item6);
            stream.Serialize(ref value.Item7);
            stream.Serialize(ref value.Item8);
            stream.Serialize(ref value.Item9);
            stream.Serialize(ref value.Item10);
            stream.Serialize(ref value.Item11);
            stream.Serialize(ref value.Item12);
            stream.Serialize(ref value.Item13);
            stream.Serialize(ref value.Item14);
            stream.Serialize(ref value.Item15);
            stream.Serialize(ref value.Item16);
        }

        public static void Serialize(this IBitStream stream, ref StagePickerType value)
        {
            int intValue = (int)value;
            stream.Serialize(ref intValue);
            value = (StagePickerType)intValue;
        }

        #endregion

        #region Misc

        public static void Serialize(this IBitStream stream, ref QBoolean value)
        {
            stream.Serialize(ref value.Value);
        }

        public static void Serialize(this IBitStream stream, ref Bot value)
        {
            stream.Serialize(ref value.Name);
            stream.Serialize(ref value.Build);
            stream.Serialize(ref value.Behavior);
        }

        public static unsafe void Serialize(this IBitStream stream, ref QString32 value)
        {
            fixed (ushort* count = &value.ByteCount)
                stream.Serialize(count);

            fixed (byte* bytes = value.Bytes)
                stream.SerializeBuffer(&bytes[0], value.ByteCount);
        }

        public static unsafe void Serialize(this IBitStream stream, ref LayerMask value)
        {
            stream.Serialize(ref value.BitMask);
        }

        public static unsafe void Serialize(this IBitStream stream, ref FighterIndex value)
        {
            stream.Serialize(ref value.Local);
            stream.Serialize(ref value.Device);
            stream.Serialize(ref value.Global);
            stream.Serialize(ref value.GlobalNoBots);
            stream.Serialize(ref value.Type);
        }

        public static unsafe void Serialize(this IBitStream stream, ref FighterType value)
        {
            int intValue = (int)value;
            stream.Serialize(ref intValue);
            value = (FighterType)intValue;
        }

        public static void Serialize(this IBitStream stream, ref ColorRGBA value)
        {
            stream.Serialize(ref value.R);
            stream.Serialize(ref value.G);
            stream.Serialize(ref value.B);
            stream.Serialize(ref value.A);
        }

        #endregion
    }
}
