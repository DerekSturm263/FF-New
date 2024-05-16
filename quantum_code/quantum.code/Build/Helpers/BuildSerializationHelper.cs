using Photon.Deterministic;

namespace Quantum
{
    public static class BuildSerializationHelper
    {
        public static void Serialize(this IBitStream stream, ref Build value)
        {
            stream.Serialize(ref value.SerializableData);
            stream.Serialize(ref value.Equipment);
            stream.Serialize(ref value.Cosmetics);
        }

        public static unsafe void Serialize(this IBitStream stream, ref SerializableData value)
        {
            fixed (byte* bytes = value.Name.Bytes)
                stream.SerializeBuffer(bytes, value.Name.Length);

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
            stream.Serialize(ref value.Emotes);
        }

        public static void Serialize(this IBitStream stream, ref DirectionalEmote value)
        {
            stream.Serialize(ref value.Up);
            stream.Serialize(ref value.Down);
            stream.Serialize(ref value.Left);
            stream.Serialize(ref value.Right);
        }
    }
}
