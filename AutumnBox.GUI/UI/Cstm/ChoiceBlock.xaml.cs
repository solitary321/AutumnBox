﻿using AutumnBox.GUI.Helper;
using AutumnBox.Support.CstmDebug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutumnBox.GUI.UI.Cstm
{
    /// <summary>
    /// ChoiceGrid.xaml 的交互逻辑
    /// </summary>
    public partial class ChoiceBlock : UserControl, IDisposable
    {
        public static readonly int AnimationLength = 300;
        public bool HasExited { get; private set; } = true;
        /// <summary>
        /// 等待结束
        /// </summary>
        public void WaitToExit()
        {
            while (!HasExited) ;
        }

        private readonly ThicknessAnimation _riseAnimation = new ThicknessAnimation()
        {
            To = new Thickness(0, 0, 0, 0),
            Duration = TimeSpan.FromMilliseconds(AnimationLength),
        };
        private readonly ThicknessAnimation _hideAnimation = new ThicknessAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(AnimationLength),
        };
        private readonly Grid _father;

        private Action<ChoiceResult> _callback;

        public ChoiceBlock(Grid father, ChoiceData data)
        {
            InitializeComponent();
            BtnLeft.Content = data.TextBtnLeft;
            BtnRight.Content = data.TextBtnRight;
            TBTitle.Text = data.Title;
            TBContent.Text = data.Text;
            _father = father;
            _father.Children.Add(this);
            this.Height = _father.ActualHeight;
            this.Width = _father.ActualWidth;
            SetTop(_father.ActualHeight);
            _hideAnimation.To = new Thickness(0, _father.ActualHeight, 0, 0);
        }

        private void SetTop(double topValue)
        {
            var margin = this.Margin;
            margin.Top = topValue;
            this.Margin = margin;
        }

        public void Show(Action<ChoiceResult> callback)
        {
            this._callback = callback;
            this.BeginAnimation(MarginProperty, _riseAnimation);
        }
        public ChoiceResult Show()
        {
            ChoiceResult result = ChoiceResult.Cancel;
            this.Dispatcher.Invoke(() =>
            {
                this.Show((r) =>
                {
                    result = r;
                });
            });
            WaitToExit();
            return result;
        }
        public void Hide()
        {
            Hide(ChoiceResult.Cancel);
        }

        private void Hide(ChoiceResult result)
        {
            this.Dispatcher.Invoke(() =>
            {
                HasExited = true;
                this.BeginAnimation(MarginProperty, _hideAnimation);
                Task.Run(() =>
                {
                    Thread.Sleep(AnimationLength);
                    this.Dispatcher.Invoke(() => { _callback?.Invoke(result); });
                    Thread.Sleep(5000);
                    Dispose();
                });
            });
        }

        public void Dispose()
        {
            this.Dispatcher.Invoke(() =>
            {
                _father.Children.Remove(this);
                GC.SuppressFinalize(this);
            });
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            Hide(ChoiceResult.Right);
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            Hide(ChoiceResult.Left);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            Task.Run(() =>
            {
                Thread.Sleep(AnimationLength);
                this.Dispatcher.Invoke(() => { _callback?.Invoke(ChoiceResult.Cancel); });
            });
        }
    }
}