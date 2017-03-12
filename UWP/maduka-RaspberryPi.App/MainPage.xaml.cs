using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x404

namespace maduka_RaspberryPi.App
{
    using maduka_RaspberryPi.Motor;

    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GpioController objGpioController = GpioController.GetDefault();
        StepMotor objMotor = null;

        public MainPage()
        {
            this.InitializeComponent();
            objMotor = new StepMotor(5, 6, 13, 26) { Sleep = 15, };
        }

        private async void btnReverseStep_Click(object sender, RoutedEventArgs e)
        {
            await objMotor.TurnAsync(int.Parse(txtAngleStep.Text), Enums.TurnDirection.Right, Enums.DrivingMethod.WaveDrive);
        }

        private async void btnFowardStep_Click(object sender, RoutedEventArgs e)
        {
            await objMotor.TurnAsync(int.Parse(txtAngleStep.Text), Enums.TurnDirection.Left, Enums.DrivingMethod.FullStep);
        }

        private void btnStopStep_Click(object sender, RoutedEventArgs e)
        {
            objMotor.Stop();
        }
    }
}
