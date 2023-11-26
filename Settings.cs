﻿namespace AutoPolarAlign
{
    public class Settings
    {
        public double AzimuthBacklash = 50;
        public double AltitudeBacklash = 10;
        public double AzimuthCalibrationDistance = 30;
        public double AltitudeCalibrationDistance = 30;
        public double AzimuthLimit = 300;
        public double AltitudeLimit = 300;
        public int SamplesPerCalibration = 1;
        public int SamplesPerMeasurement = 3;
        public int MaxAlignmentIterations = 10;
        public double AlignmentThreshold = 1;
        public bool AcceptBestEffort = false;
        public double StartAggressiveness = 1.0;
        public double EndAggressiveness = 0.25;
    }
}
