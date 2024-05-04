using Photon.Deterministic;

namespace Quantum
{
    unsafe partial struct Input
    {
        [System.Flags]
        public enum Buttons : ushort
        {
            None = 1 << 0,          // Player is not pressing any buttons
            Jump = 1 << 1,          // Player pressed the “jump” button this tick
            FastFall = 1 << 2,      // Player pressed the “fast fall” button this tick
            Crouch = 1 << 3,        // Player pressed the “crouch” button this tick
            Block1 = 1 << 4,         // Player pressed the “block” button this tick
            Block2 = 1 << 5,         // Player pressed the “block” button this tick

            MainWeapon = 1 << 6,    // Player pressed the “main weapon” button this tick
            AlternateWeapon = 1 << 7,         // Player pressed the “skill” button this tick
            SubWeapon = 1 << 8,     // Player pressed the “sub-weapon” button this tick

            Emote = 1 << 9,         // Player pressed the “emote” button this tick
            Interact = 1 << 10,     // Player pressed the “interact” button this tick

            Join = 1 << 11,         // Player pressed the “join” button this tick
            Leave = 1 << 12,        // Player pressed the “leave” button this tick
            Ready = 1 << 13,        // Player pressed the “ready” button this tick
            Cancel = 1 << 14        // Player pressed the “cancel” button this tick
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

        public bool FastFall
        {
            readonly get
            {
                return (InputButtons & Buttons.FastFall) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.FastFall;
                else
                    InputButtons &= ~Buttons.FastFall;
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

        public bool Block1
        {
            private readonly get
            {
                return (InputButtons & Buttons.Block1) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Block1;
                else
                    InputButtons &= ~Buttons.Block1;
            }
        }

        public bool Block2
        {
            private readonly get
            {
                return (InputButtons & Buttons.Block2) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Block2;
                else
                    InputButtons &= ~Buttons.Block2;
            }
        }

        public readonly bool Block => Block1 ^ Block2;

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
            readonly get
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

        public readonly bool Ultimate => MainWeapon && AlternateWeapon;

        public readonly bool Burst => Block1 && Block2;

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

        public bool Interact
        {
            readonly get
            {
                return (InputButtons & Buttons.Interact) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Interact;
                else
                    InputButtons &= ~Buttons.Interact;
            }
        }

        public bool Join
        {
            readonly get
            {
                return (InputButtons & Buttons.Join) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Join;
                else
                    InputButtons &= ~Buttons.Join;
            }
        }

        public bool Leave
        {
            readonly get
            {
                return (InputButtons & Buttons.Leave) != 0;
            }
            set
            {
                if (value == true)
                    InputButtons |= Buttons.Leave;
                else
                    InputButtons &= ~Buttons.Leave;
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
