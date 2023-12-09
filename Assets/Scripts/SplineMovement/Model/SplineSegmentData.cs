using Unity.Collections;
using Unity.Mathematics;

namespace GC.SplineMovement
{
    public struct SplineSegmentData
    {
        public const int COUNT_OF_BROKEN_LINE_POINTS = 50;

        public NativeArray<float3> brokenLinePoints;
        public NativeArray<float3> brokenLineRotations;
        public NativeArray<float> brokenLinesPercents;

        public void Init(SplineSegment segmentToInit, float startPercent)
        {
            brokenLinePoints = new NativeArray<float3>(COUNT_OF_BROKEN_LINE_POINTS + 1, Allocator.Persistent);
            brokenLineRotations = new NativeArray<float3>(COUNT_OF_BROKEN_LINE_POINTS + 1, Allocator.Persistent);
            brokenLinesPercents = new NativeArray<float>(COUNT_OF_BROKEN_LINE_POINTS + 1, Allocator.Persistent);

            InitBrokenLinePoints(segmentToInit);

            InitBrokenLinePercents(startPercent);
        }

        private void InitBrokenLinePoints(SplineSegment segment)
        {
            for (int i = 0; i <= COUNT_OF_BROKEN_LINE_POINTS; i++)
            {
                brokenLinePoints[i] = segment.GetPointOnSegment((float)i / COUNT_OF_BROKEN_LINE_POINTS);
            }
        }

        private void InitBrokenLinePercents(float startPercent)
        {
            brokenLinesPercents[0] = startPercent;
            brokenLineRotations[0] = float3.zero;

            for (int i = 0; i < COUNT_OF_BROKEN_LINE_POINTS; i++)
            {
                float3 start = brokenLinePoints[i];
                float3 end = brokenLinePoints[i + 1];

                float length = math.length(end - start);

                brokenLinesPercents[i + 1] = length + brokenLinesPercents[i];
                brokenLineRotations[i + 1] = end - start;
            }
        }
    }
}
