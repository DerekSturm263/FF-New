using Photon.Deterministic;

namespace Quantum
{
    unsafe partial struct Input
    {
        [System.Flags]
        public enum Buttons : ushort
        {
            Jump = 1 << 1,           // Player pressed the “jump” button this tick
            LookUp = 1 << 2,         // Player pressed the “look up” button this tick
            Burst = 1 << 3,          // Player pressed the “burst” button this tick
            Block = 1 << 4,          // Player pressed the “block” button this tick

            MainWeapon = 1 << 5,     // Player pressed the “main weapon” button this tick
            AlternateWeapon = 1 << 6,// Player pressed the “skill” button this tick
            SubWeapon = 1 << 7,      // Player pressed the “sub-weapon” button this tick
            Ultimate = 1 << 8,       // Player pressed the “ultimate” button this tick

            Emote = 1 << 9,          // Player pressed the “emote” button this tick
            LeftRight = 1 << 10,     // Player pressed the “interact” button this tick

            Dodge = 1 << 11,         // Player pressed the “dodge” button this tick
            Crouch = 1 << 12,        // Player pressed the “leave” button this tick
            Ready = 1 << 13,         // Player pressed the “ready” button this tick
            Cancel = 1 << 14         // Player pressed the “cancel” button this tick
        }

        #region Internal Values

        private const byte DIRECTION_OFFSET = 0;
        private const byte MAGNITUDE_OFFSET = 8;
        private const byte BUTTONS_OFFSET = 16;

        internal FP Direction
        {
            readonly get
            {
                byte encodedDirection = (byte)(_encodedInformation >> DIRECTION_OFFSET);

                FP angle = (encodedDirection / ((FP)255 / 360)) * FP.Deg2Rad - ((FP)1 / 17);
                return angle;
            }
            set
            {
                FP angle = (value * FP.Rad2Deg) / ((FP)360 / 255) + FP.Pi;
                _encodedInformation |= (byte)angle.AsInt << DIRECTION_OFFSET;
            }
        }

        internal FP Magnitude
        {
            readonly get
            {
                byte encodedMagnitude = (byte)(_encodedInformation >> MAGNITUDE_OFFSET);

                FP magnitude = (FP)encodedMagnitude / 255;
                return magnitude;
            }
            set
            {
                FP magnitude = value * 255;
                _encodedInformation |= (byte)magnitude.AsInt << MAGNITUDE_OFFSET;
            }
        }

        internal Buttons InputButtons
        {
            readonly get => (Buttons)(_encodedInformation >> BUTTONS_OFFSET);
            set => _encodedInformation |= (ushort)value << BUTTONS_OFFSET;
        }

        #endregion

        #region Inputs

        public FPVector2 Movement
        {
            readonly get => FPVector2.Rotate(new FPVector2(0, Magnitude), Direction);
            set
            {
                Magnitude = value.Magnitude;
                Direction = FPVector2.RadiansSigned(FPVector2.Up, value);
            }
        }

        private static FP DOT_SUCCESS = FP._0_50;

        public readonly FPVector2 SnapMovementTo8Directions
        {
            get
            {
                FPVector2 movement = Movement;
                FPVector2 dir = FPVector2.Zero;

                if (FPVector2.Dot(movement, FPVector2.Up) > DOT_SUCCESS)
                    dir += FPVector2.Up;
                else if (FPVector2.Dot(movement, FPVector2.Down) > DOT_SUCCESS)
                    dir += FPVector2.Down;

                if (FPVector2.Dot(movement, FPVector2.Left) > DOT_SUCCESS)
                    dir += FPVector2.Left;
                else if (FPVector2.Dot(movement, FPVector2.Right) > DOT_SUCCESS)
                    dir += FPVector2.Right;

                return dir.Normalized;
            }
        }

        public readonly bool MovementUp => SnapMovementTo8Directions == FPVector2.Up;
        public readonly bool MovementDown => SnapMovementTo8Directions == FPVector2.Down;
        public readonly bool MovementLeft => SnapMovementTo8Directions == FPVector2.Left;
        public readonly bool MovementRight => SnapMovementTo8Directions == FPVector2.Right;

        public bool Jump
        {
            readonly get
            {
                return (InputButtons & Buttons.Jump) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Jump;
                else
                    InputButtons &= ~Buttons.Jump;
            }
        }

        public bool LookUp
        {
            readonly get
            {
                return (InputButtons & Buttons.LookUp) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.LookUp;
                else
                    InputButtons &= ~Buttons.LookUp;
            }
        }

        public bool Burst
        {
            readonly get
            {
                return (InputButtons & Buttons.Burst) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Burst;
                else
                    InputButtons &= ~Buttons.Burst;
            }
        }

        public bool Block
        {
            readonly get
            {
                return (InputButtons & Buttons.Block) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Block;
                else
                    InputButtons &= ~Buttons.Block;
            }
        }

        public bool MainWeapon
        {
            readonly get
            {
                return (InputButtons & Buttons.MainWeapon) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.MainWeapon;
                else
                    InputButtons &= ~Buttons.MainWeapon;
            }
        }

        public bool AlternateWeapon
        {
            readonly get
            {
                return (InputButtons & Buttons.AlternateWeapon) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.AlternateWeapon;
                else
                    InputButtons &= ~Buttons.AlternateWeapon;
            }
        }

        public bool SubWeapon
        {
            private readonly get
            {
                return (InputButtons & Buttons.SubWeapon) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.SubWeapon;
                else
                    InputButtons &= ~Buttons.SubWeapon;
            }
        }

        public bool Ultimate
        {
            private readonly get
            {
                return (InputButtons & Buttons.Ultimate) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Ultimate;
                else
                    InputButtons &= ~Buttons.Ultimate;
            }
        }

        public bool Emote
        {
            readonly get
            {
                return (InputButtons & Buttons.Emote) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Emote;
                else
                    InputButtons &= ~Buttons.Emote;
            }
        }

        public bool LeftRight
        {
            readonly get
            {
                return (InputButtons & Buttons.LeftRight) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.LeftRight;
                else
                    InputButtons &= ~Buttons.LeftRight;
            }
        }

        public bool Dodge
        {
            readonly get
            {
                return (InputButtons & Buttons.Dodge) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Dodge;
                else
                    InputButtons &= ~Buttons.Dodge;
            }
        }

        public bool Crouch
        {
            readonly get
            {
                return (InputButtons & Buttons.Crouch) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Crouch;
                else
                    InputButtons &= ~Buttons.Crouch;
            }
        }

        public bool Ready
        {
            readonly get
            {
                return (InputButtons & Buttons.Ready) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Ready;
                else
                    InputButtons &= ~Buttons.Ready;
            }
        }

        public bool Cancel
        {
            readonly get
            {
                return (InputButtons & Buttons.Cancel) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Cancel;
                else
                    InputButtons &= ~Buttons.Cancel;
            }
        }

        #endregion
    }
}
