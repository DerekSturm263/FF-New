using Photon.Deterministic;

namespace Quantum
{
    public static class BitStreamSerializationHelper
    {
        #region Build

        public static void Serialize(this IBitStream stream, ref Build value)
        {
            stream.Serialize(ref value.SerializableData);
            stream.Serialize(ref value.Equipment);
            stream.Serialize(ref value.Cosmetics);
        }

        public static unsafe void Serialize(this IBitStream stream, ref SerializableData value)
        {
            fixed (ushort* count = &value.Name.ByteCount)
                stream.Serialize(count);

            fixed (byte* bytes = value.Name.Bytes)
                stream.SerializeBuffer(&bytes[0], value.Name.ByteCount);

            fixed (ushort* count = &value.Description.ByteCount)
                stream.Serialize(count);

            fixed (byte* bytes = value.Description.Bytes)
                stream.SerializeBuffer(&bytes[0], value.Description.ByteCount);

            stream.Serialize(ref value.Guid);
            stream.Serialize(ref value.CreationDate);
            stream.Serialize(ref value.LastEdittedDate);
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
            stream.Serialize(ref value.Enhancers);
        }

        public static void Serialize(this IBitStream stream, ref WeaponEnhancerSet value)
        {
            stream.Serialize(ref value.Enhancer1);
            stream.Serialize(ref value.Enhancer2);
        }

        public static void Serialize(this IBitStream stream, ref Sub value)
        {
            stream.Serialize(ref value.Template);
            stream.Serialize(ref value.Enhancers);
        }

        public static void Serialize(this IBitStream stream, ref SubEnhancerSet value)
        {
            stream.Serialize(ref value.Enhancer1);
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
            stream.Serialize(ref value.Pattern);
            stream.Serialize(ref value.Modifiers);
            stream.Serialize(ref value.Color);
        }

        public static void Serialize(this IBitStream stream, ref ColorRGBA value)
        {
            stream.Serialize(ref value.R);
            stream.Serialize(ref value.G);
            stream.Serialize(ref value.B);
            stream.Serialize(ref value.A);
        }

        public static void Serialize(this IBitStream stream, ref ApparelModifierSet value)
        {
            stream.Serialize(ref value.Modifier1);
            stream.Serialize(ref value.Modifier2);
            stream.Serialize(ref value.Modifier3);
            stream.Serialize(ref value.Modifier4);
        }

        public static void Serialize(this IBitStream stream, ref CosmeticSettings value)
        {
            stream.Serialize(ref value.Avatar);
            stream.Serialize(ref value.Eyes);
            stream.Serialize(ref value.Hair);
            stream.Serialize(ref value.Voice);
            stream.Serialize(ref value.Emotes);
        }

        public static void Serialize(this IBitStream stream, ref DirectionalEmote value)
        {
            stream.Serialize(ref value.Up);
            stream.Serialize(ref value.Down);
            stream.Serialize(ref value.Left);
            stream.Serialize(ref value.Right);
        }

        #endregion

        #region Stage

        public static void Serialize(this IBitStream stream, ref Stage value)
        {
            stream.Serialize(ref value.SerializableData);
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
        }

        public static unsafe void Serialize(this IBitStream stream, ref ObjectSettings value)
        {
            stream.Serialize(ref value.Stage);
            stream.Serialize(ref value.Map);
            stream.SerializeBuffer(value.Gizmos.Buffer, value.Gizmos.Length);
        }

        public static unsafe void Serialize(this IBitStream stream, ref SpawnSettings value)
        {
            stream.Serialize(ref value.PlayerSpawnPoints);
            //stream.SerializeBuffer(value.ItemSpawnPoints.Buffer, value.ItemSpawnPoints.Length);
        }

        public static unsafe void Serialize(this IBitStream stream, ref Array4FPVector2 value)
        {
            stream.Serialize(ref value.Item1);
            stream.Serialize(ref value.Item2);
            stream.Serialize(ref value.Item3);
            stream.Serialize(ref value.Item4);
        }

        #endregion

        #region Ruleset

        public static void Serialize(this IBitStream stream, ref Ruleset value)
        {
            stream.Serialize(ref value.SerializableData);
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
            stream.SerializeBuffer(value.Stages.Buffer, value.Stages.Length);
            stream.Serialize(ref value.StagePicker);
            stream.Serialize(ref value.AllowGizmos);
            stream.Serialize(ref value.AllowCustomStages);
            stream.Serialize(ref value.DisallowDuplicateSelection);
        }

        public static unsafe void Serialize(this IBitStream stream, ref ItemSettings value)
        {
            stream.Serialize(ref value.StartingItem);
            stream.SerializeBuffer(value.Items.Buffer, value.Items.Length);
            stream.Serialize(ref value.SpawnFrequency);
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

        #endregion
    }
}
