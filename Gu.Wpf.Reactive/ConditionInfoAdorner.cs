﻿namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class ConditionInfoAdorner : Adorner, IWeakEventListener
    {
        private readonly SolidColorBrush _transparentBrush = new SolidColorBrush(Colors.AliceBlue);
        private readonly ButtonBase _button;
        private readonly Button _adornerButton;
        private readonly Popup _popup;

        static ConditionInfoAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionInfoAdorner), new FrameworkPropertyMetadata(typeof(ConditionInfoAdorner)));
        }

        // Be sure to call the base class constructor. 
        public ConditionInfoAdorner(ButtonBase button)
            : base(button)
        {
            _adornerButton = new Button
                          {
                              Width = 100,
                              Height = 100,
                              Background = this._transparentBrush,
                              FocusVisualStyle = null
                          };
            _adornerButton.Click += AdornerButtonOnClick;
            AddVisualChild(this._adornerButton);
            AddLogicalChild(this._adornerButton);
            _button = button;
            _popup = new Popup
            {
                PlacementTarget = _button,
                DataContext = _button.DataContext,
                Child = new Border
                {
                    BorderThickness = new Thickness(1),
                    Background = Brushes.LightYellow,
                    Child = new ConditionControl
                    {
                        Condition = ((ConditionRelayCommand)_button.Command).Condition,
                    }
                },
            };
            _button.LostFocus += (sender, args) =>
            {
                if (_popup.IsOpen)
                {
                    _popup.IsOpen = false;
                }
            };
            _adornerButton.LostFocus += (sender, args) =>
            {
                if (_popup.IsOpen && !_popup.IsKeyboardFocusWithin)
                {
                    _popup.IsOpen = false;
                }
            };
            _popup.LostFocus += (sender, args) =>
            {
                if (_popup.IsOpen)
                {
                    _popup.IsOpen = false;
                }
            };
            Gu.Wpf.Reactive.CanExecuteChangedEventManager.AddListener(_button.Command, this);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType != typeof(Gu.Wpf.Reactive.CanExecuteChangedEventManager))
            {
                return false;
            }
            var command = _button.Command;
            if (command != null)
            {
                this.Visibility = command.CanExecute(_button.CommandParameter)
                    ? Visibility.Hidden
                    : Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Hidden;
            }
            return true;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return _adornerButton;
            }
            throw new ArgumentOutOfRangeException("index");
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _adornerButton.Measure(constraint);
            return _adornerButton.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornerButton.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        private void AdornerButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            _popup.IsOpen = !_popup.IsOpen;
        }
    }
}
