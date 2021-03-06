﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WilCommon;

namespace RgbToSpectrum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConnectionViewModel connectionViewModel = new ConnectionViewModel();
        
        bool initialized = false;
        SimpleSpectrum spectrum;
        FilteredSpectrum filtered;

        public enum DisplayType : int
        {
            SnakeCurves,
            CatmullRomSplines
        }

        public MainWindow()
        {
            //Utilities.ConvertImage(@"..\..\Docs\Test Images\16Rx16Gx16B.png", new WarmingFilter85());
            //Utilities.ConvertImage(@"..\..\Docs\Test Images\16Rx16Gx16B.png", new ColorSpectrumFilter(1.0, 0.0, 0.0));
            //Utilities.ConvertImage(@"..\..\Docs\Test Images\16Rx16Gx16B.png", new ColorSpectrumFilter(0.9255, 0.54117, 0.0));
            //Utilities.PrecomputeCoefficients();

            InitializeComponent();
            DataContext = connectionViewModel;

            ComboBoxDisplay.ItemsSource = Enum.GetValues(typeof(DisplayType)).Cast<DisplayType>();
            ComboBoxDisplay.SelectedIndex = 0;

            initialized = true;

            UpdateInputSpectrum();
            UpdateFilter();
            UpdateFilteredSpectrum();
        }

        private void SliderInputColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateInputSpectrum();
            UpdateFilteredSpectrum();
        }

        private void SliderFilterColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateFilterColor();
            UpdateFilteredSpectrum();
        }

        private void ComboBoxFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateFilter();
            UpdateFilteredSpectrum();
        }

        private void ComboBoxDisplay_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInputSpectrum();
            UpdateFilter();
            UpdateFilteredSpectrum();
        }

        void UpdateInputSpectrum()
        {
            if (!initialized)
                return;

            var R = SliderInR.Value;
            var G = SliderInG.Value;
            var B = SliderInB.Value;

            LabelRin.Content = String.Format("R={0:000}", R);
            LabelGin.Content = String.Format("G={0:000}", G);
            LabelBin.Content = String.Format("B={0:000}", B);

            var color = System.Drawing.Color.FromArgb(255, R.ToByte(), G.ToByte(), B.ToByte());
            LabelHin.Content = String.Format("H={0:000}", color.GetHue());
            LabelSin.Content = String.Format("S={0:0.000}", color.GetSaturation());
            LabelLin.Content = String.Format("B={0:0.000}", color.GetBrightness());

            var brush = new SolidColorBrush();
            brush.Color = System.Windows.Media.Color.FromArgb(255, R.ToByte(), G.ToByte(), B.ToByte());
            InputColorGrid.Background = brush;

            spectrum = new SimpleSpectrum(R / 255, G / 255, B / 255);
            ImageInput.Source = spectrum.ToBitmap(ComboBoxDisplay.SelectedIndex==(int)DisplayType.CatmullRomSplines).ToBitmapSource();

            // TODO more precision in RGBCYM spectra (lux render has data for 32 bins)
        }

        void UpdateFilter()
        {
            if (!initialized)
                return;

            ImageFilter.Source = connectionViewModel.SelectedFilter.ToBitmap(ComboBoxDisplay.SelectedIndex == (int)DisplayType.CatmullRomSplines).ToBitmapSource();
        }

        void UpdateFilterColor()
        {
            if (!initialized)
                return;

            ColorSpectrumFilter filter = connectionViewModel.SelectedFilter as ColorSpectrumFilter;
            if (filter != null)
            {
                var R = SliderFilterR.Value;
                var G = SliderFilterG.Value;
                var B = SliderFilterB.Value;

                LabelRfilter.Content = String.Format("R={0:000}", R);
                LabelGfilter.Content = String.Format("G={0:000}", G);
                LabelBfilter.Content = String.Format("B={0:000}", B);

                var color = System.Drawing.Color.FromArgb(255, R.ToByte(), G.ToByte(), B.ToByte());
                LabelHfilter.Content = String.Format("H={0:000}", color.GetHue());
                LabelSfilter.Content = String.Format("S={0:0.000}", color.GetSaturation());
                LabelLfilter.Content = String.Format("B={0:0.000}", color.GetBrightness());

                var brush = new SolidColorBrush();
                brush.Color = System.Windows.Media.Color.FromArgb(255, R.ToByte(), G.ToByte(), B.ToByte());
                FilterColorGrid.Background = brush;

                filter.SetColor(R / 255, G / 255, B / 255);
                UpdateFilter();
            }
        }

        void UpdateFilteredSpectrum()
        {
            if (!initialized || connectionViewModel.SelectedFilter == null || spectrum == null)
                return;

            filtered = new FilteredSpectrum(spectrum, connectionViewModel.SelectedFilter);
            ImageOutput.Source = filtered.ToBitmap(ComboBoxDisplay.SelectedIndex == (int)DisplayType.CatmullRomSplines).ToBitmapSource();

            XYZColor xyz = new XYZColor(filtered);
            System.Drawing.Color rgb = xyz.ToRGB();
            var brush = new SolidColorBrush();
            brush.Color = System.Windows.Media.Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
            OutputColorGrid.Background = brush;

            LabelRout.Content = String.Format("R={0:000}", rgb.R);
            LabelGout.Content = String.Format("G={0:000}", rgb.G);
            LabelBout.Content = String.Format("B={0:000}", rgb.B);

            LabelHout.Content = String.Format("H={0:000}", rgb.GetHue());
            LabelSout.Content = String.Format("S={0:0.000}", rgb.GetSaturation());
            LabelLout.Content = String.Format("B={0:0.000}", rgb.GetBrightness());
        }




    }
}
