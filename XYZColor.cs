﻿using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathnetHelper;
using System.Drawing;
using WilCommon;

namespace RgbToSpectrum
{
    public class XYZColor
    {
        static readonly int BinsCount = 89;
        static readonly double[] Lambdas   = { 360, 365, 370, 375, 380, 385, 390, 395, 400, 405, 410, 415, 420, 425, 430, 435, 440, 445, 450, 455, 460, 465, 470, 475, 480, 485, 490, 495, 500, 505, 510, 515, 520, 525, 530, 535, 540, 545, 550, 555, 560, 565, 570, 575, 580, 585, 590, 595, 600, 605, 610, 615, 620, 625, 630, 635, 640, 645, 650, 655, 660, 665, 670, 675, 680, 685, 690, 695, 700, 705, 710, 715, 720, 725, 730, 735, 740, 745, 750, 755, 760, 765, 770, 775, 780, 785, 790, 795, 800 };
        static readonly double[] MatchingX = { 0.000130, 0.000230, 0.000410, 0.000740, 0.001370, 0.002230, 0.004240, 0.007650, 0.014310, 0.023190, 0.043510, 0.077630, 0.134380, 0.214770, 0.283900, 0.328500, 0.348280, 0.348060, 0.336200, 0.318700, 0.290800, 0.251100, 0.195360, 0.142100, 0.095640, 0.057950, 0.032010, 0.014700, 0.004900, 0.002400, 0.009300, 0.029100, 0.063270, 0.109600, 0.165500, 0.225750, 0.290400, 0.359700, 0.433450, 0.512050, 0.594500, 0.678400, 0.762100, 0.842500, 0.916300, 0.978600, 1.026300, 1.056700, 1.062200, 1.045600, 1.002600, 0.938400, 0.854450, 0.751400, 0.642400, 0.541900, 0.447900, 0.360800, 0.283500, 0.218700, 0.164900, 0.121200, 0.087400, 0.063600, 0.046770, 0.032900, 0.022700, 0.015840, 0.011360, 0.008110, 0.005790, 0.004110, 0.002890, 0.002050, 0.001440, 0.001000, 0.000690, 0.000480, 0.000330, 0.000230, 0.000170, 0.000120, 0.000080, 0.000060, 0.000041, 0.000029, 0.000020, 0.000014, 0.000010 };
        static readonly double[] MatchingY = { 0.000000, 0.000000, 0.000010, 0.000020, 0.000040, 0.000060, 0.000120, 0.000220, 0.000400, 0.000640, 0.001200, 0.002180, 0.004000, 0.007300, 0.011600, 0.016840, 0.023000, 0.029800, 0.038000, 0.048000, 0.060000, 0.073900, 0.090980, 0.112600, 0.139020, 0.169300, 0.208020, 0.258600, 0.323000, 0.407300, 0.503000, 0.608200, 0.710000, 0.793200, 0.862000, 0.914850, 0.954000, 0.980300, 0.994950, 1.000000, 0.995000, 0.978600, 0.952000, 0.915400, 0.870000, 0.816300, 0.757000, 0.694900, 0.631000, 0.566800, 0.503000, 0.441200, 0.381000, 0.321000, 0.265000, 0.217000, 0.175000, 0.138200, 0.107000, 0.081600, 0.061000, 0.044580, 0.032000, 0.023200, 0.017000, 0.011920, 0.008210, 0.005730, 0.004100, 0.002930, 0.002090, 0.001050, 0.001050, 0.000740, 0.000520, 0.000360, 0.000250, 0.000170, 0.000120, 0.000080, 0.000060, 0.000040, 0.000030, 0.000020, 0.000014, 0.000010, 0.000007, 0.000005, 0.000003 };
        static readonly double[] MatchingZ = { 0.000610, 0.001080, 0.001950, 0.003490, 0.006450, 0.010550, 0.020050, 0.036210, 0.067850, 0.110200, 0.207400, 0.371300, 0.645600, 1.039050, 1.385600, 1.622960, 1.747060, 1.782600, 1.772110, 1.744100, 1.669200, 1.528100, 1.287640, 1.041900, 0.812950, 0.616200, 0.465180, 0.353300, 0.272000, 0.212300, 0.158200, 0.111700, 0.078250, 0.057250, 0.042160, 0.029840, 0.020300, 0.013400, 0.008750, 0.005750, 0.003900, 0.002750, 0.002100, 0.001800, 0.001650, 0.001400, 0.001100, 0.001000, 0.000800, 0.000600, 0.000340, 0.000240, 0.000190, 0.000100, 0.000050, 0.000030, 0.000020, 0.000010, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000 };

        public double LambdaMin { get { return Lambdas[0]; } }
        public double LambdaMax { get { return Lambdas[BinsCount - 1]; } }
        public double LambdaStep { get { return Lambdas[1] - Lambdas[0]; } }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }


        public XYZColor(FilteredSpectrum spectrum)
        {           
            // TODO
            // http://en.wikipedia.org/wiki/CIE_1931_color_space#Color_matching_functions

            X = 0;
            Y = 0;
            Z = 0;

            for (double l = 360; l <= 800; l += LambdaStep)
            {
                int index = SampleIndex(l);
                double x = MatchingX[index];
                double y = MatchingY[index];
                double z = MatchingZ[index];

                double I = spectrum.Sample(l);

                X += I * x * LambdaStep;
                Y += I * y * LambdaStep;
                Z += I * z * LambdaStep;
            }

        }

        public Color ToRGB()
        {
            var xyz = Vector<double>.Build.DenseOfArray(new[] { X, Y, Z});
            var rgb = ComplexSpectrum.XYZtoRGB * xyz;

            return Color.FromArgb(255, rgb[0].ScaleToByte(), rgb[1].ScaleToByte(), rgb[2].ScaleToByte());
        }


        int SampleIndex(double lambda)
        {
            // find closest smaller lambda
            int i;
            for (i = 0; i < BinsCount - 1; ++i)
            {
                if (Lambdas[i + 1] > lambda)
                    break;
            }
            if (lambda >= Lambdas[BinsCount - 1])
                i = BinsCount - 1;

            return i;
        }

    }
}
