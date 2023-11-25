﻿using System.Collections.Generic;
using System;

namespace AutoPolarAlign
{
    public class Simulation
    {
        public static void Run()
        {
            int numRuns = 10;

            var rng = new Random();

            List<double> results = new List<double>();
            List<double> totals = new List<double>();
            List<int> iterations = new List<int>();

            for (int i = 0; i < numRuns; ++i)
            {
                Vec2 initialAlignmentOffset = new Vec2(42.0f, -87.0f);

                Vec2 altAxis = new Vec2(1, 1);
                Vec2 azAxis = new Vec2(-1, 1);

                float altAxisScale = 3.0f;
                float azAxisScale = 2.0f;

                float backlashScale = 1.0f;
                float compensationScale = 1.0f;

                float aggressiveness = 1.0f;

                float randomnessScale = 5.0f;
                float offsetJitter = 1.0f;
                float moveJitter = 0.1f;

                float altBacklash = 30.0f;
                float azBacklash = 30.0f;

                float altBacklashCompensation = 30.0f;
                float azBacklashCompensation = 30.0f;

                float altCalibrationDistance = 150.0f;
                float azCalibrationDistance = 150.0f;

                float initialAltBacklash = 15.0f;
                float initialAzBacklash = -5.0f;

                var simulator = new Simulator(
                    rng,
                    initialAlignmentOffset,
                    altAxis * altAxisScale, azAxis * azAxisScale,
                    offsetJitter * randomnessScale, moveJitter * randomnessScale,
                    altBacklash * backlashScale, azBacklash * backlashScale,
                    initialAltBacklash * backlashScale, -initialAzBacklash * backlashScale
                    );

                var settings = new Settings()
                {
                    AltitudeBacklash = altBacklashCompensation * backlashScale,
                    AltitudeCalibrationDistance = altCalibrationDistance,
                    AzimuthBacklash = azBacklashCompensation * backlashScale,
                    AzimuthCalibrationDistance = azCalibrationDistance
                };

                var aligner = new AutoPolarAlignment(simulator, simulator, settings);

                try
                {
                    simulator.Connect();

                    aligner.Calibrate();

                    double totalOffsets = 0.0;

                    int j = 0;
                    for (; j < settings.MaxAlignmentIterations; ++j)
                    {
                        if (!aligner.AlignOnce(settings.AlignmentThreshold, aggressiveness, compensationScale))
                        {
                            break;
                        }
                        totalOffsets += simulator.TrueAlignmentOffset.Length;
                    }

                    results.Add(simulator.TrueAlignmentOffset.Length);
                    totals.Add(totalOffsets);
                    iterations.Add(j);
                }
                finally
                {
                    simulator.Dispose();
                }
            }

            Console.WriteLine("┌────────────────────────────────────────────┐");
            Console.WriteLine("│  Run      Offset        Total   Iterations │");
            Console.WriteLine("├────────────────────────────────────────────┤");
            double resultsSum = 0;
            double totalsSum = 0;
            int totalIterations = 0;
            for (int i = 0; i < results.Count; ++i)
            {
                Console.WriteLine(string.Format("│ {0,4:###0} {1,11:#######0.00}  {2,11:#######0.00}  {3,11:#######0} │", i, results[i], totals[i], iterations[i]));
                resultsSum += results[i];
                totalsSum += totals[i];
                totalIterations += iterations[i];
            }
            Console.WriteLine("├────────────────────────────────────────────┤");
            Console.WriteLine(string.Format("│ Avg. {0,11:#######0.00}  {1,11:#######0.00}  {2,11:#######0.00} │", resultsSum / results.Count, totalsSum / results.Count, totalIterations / (float)results.Count));
            Console.WriteLine("└────────────────────────────────────────────┘");
        }
    }
}
