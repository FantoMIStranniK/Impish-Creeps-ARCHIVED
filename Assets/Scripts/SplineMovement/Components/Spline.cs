using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GC.SplineMovement
{
    [ChunkSerializable]
    public struct Spline : IComponentData
    {
        public NativeArray<SplineSegment> SplineSegments;

        public float SplineLength;

        public void Init(NativeArray<SplineSegment> segments)
        {
            SplineSegments = segments;

            SplineLength = 0f;

            InitSpline();

            GetTotalLength();
        }
        private void InitSpline()
        {
            for (int i = 0; i < SplineSegments.Length; i++)
            {
                float startPercent = 0f;

                if (i != 0)
                    startPercent = SplineSegments[i - 1].SplineData.brokenLinesPercents[^1];

                var segment = SplineSegments[i];

                segment.Init(startPercent);
                SplineSegments[i] = segment;
            }
        }

        private void GetTotalLength()
        {
            foreach (var segment in SplineSegments)
            {
                SplineLength += segment.SplineData.brokenLinesPercents[^1];
            }
        }

        public (float3 position, float3 rotation) GetPointAndRotationOnSpline(float time)
        {
            int currentSegmentIndex = GetCurrentSegment(time);

            var data = SplineSegments[currentSegmentIndex].SplineData;

            var percents = data.brokenLinesPercents;
            var points = data.brokenLinePoints;
            var rotations = data.brokenLineRotations;

            for (int i = 0; i < percents.Length - 1; i++)
            {
                if (time > percents[i] && time < percents[i + 1])
                {
                    float alpha = (time - percents[i]) / (percents[i + 1] - percents[i]);

                    return (math.lerp(points[i], points[i + 1], alpha), math.lerp(rotations[i], rotations[i + 1], alpha));
                }
            }

            return (points[^1], rotations[^1]);
        }

        private int GetCurrentSegment(float time)
        {
            for (int i = 0; i < SplineSegments.Length; i++)
            {
                float percent = SplineSegments[i].SplineData.brokenLinesPercents[^1];

                if (time / SplineLength < percent / SplineLength)
                    return i;
            }

            return SplineSegments.Length - 1;
        }
    }
}
