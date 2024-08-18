using System;
using Photon.Deterministic;

namespace Quantum.Custom.Animator
{
    [Serializable]
    public class AnimatorData
    {
        public string clipName;
        public FP length;
        public int index;
        public int frameRate;
        public int frameCount;
        public AnimatorFrame[] frames = new AnimatorFrame[17];
        public FPAnimationCurve[] curves;

        public bool looped;
        public bool mirror;
        public bool isReversed;

        public AnimatorFrame CalculateDelta(FP lastTime, FP currentTime)
        {
            return GetFrameAtTime(lastTime) - GetFrameAtTime(currentTime);
        }

        public AnimatorFrame GetFrameAtTime(FP time)
        {
            AnimatorFrame output = new()
            {
                hurtboxPositions = new HurtboxTransformInfo[17]
            };

            int timeIndex;

            if (isReversed)
            {
                time = length - time;

                if (length == FP._0)
                    return frames[0];

                while (time < length)
                {
                    time += length;
                    output -= frames[frameCount - 1];
                }

                timeIndex = 0;
                for (int f = frameCount - 1; f >= 1; f--)
                {
                    if (frames[f].time < time)
                    {
                        timeIndex = f;
                        break;
                    }
                }

                AnimatorFrame frameA = frames[timeIndex - 1];
                AnimatorFrame frameB = frames[timeIndex];
                FP currentTime = time - frameA.time;
                FP frameTime = frameB.time - frameA.time;
                FP lerp = currentTime / frameTime;

                //      Log.Info(clipName+" "+time.AsFloat()+" "+ frame_a + " "+ frame_b + " "+lerp.AsFloat());
                return output + AnimatorFrame.Lerp(frameB, frameA, lerp);
            }
            else
            {
                if (length == FP._0)
                    return frames[0];

                while (time > length)
                {
                    time -= length;
                    output += frames[frameCount - 1];
                }

                timeIndex = frameCount - 1;
                for (int f = 1; f < frameCount; f++)
                {
                    if (frames[f].time > time)
                    {
                        timeIndex = f;
                        break;
                    }
                }

                AnimatorFrame frameA = frames[timeIndex - 1];
                AnimatorFrame frameB = frames[timeIndex];
                FP currentTime = time - frameA.time;
                FP frameTime = frameB.time - frameA.time;
                FP lerp = currentTime / frameTime;

                Log.Debug($"I: {timeIndex}");

                //      Log.Info(clipName+" "+time.AsFloat()+" "+ frame_a + " "+ frame_b + " "+lerp.AsFloat());
                return output + AnimatorFrame.Lerp(frameA, frameB, lerp);
            }
        }
    }
}
