using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Barricade.Presentation.Dynamisch
{
    /// <summary>
    /// Interaction logic for Pion.xaml
    /// </summary>
    public partial class Pion : UserControl
    {
        public Logic.Pion Stuk { get; private set; }
        private readonly Queue<Point> _queue = new Queue<Point>();
        private bool _moving;

        public Pion(Logic.Pion pion)
        {
            Stuk = pion;
            InitializeComponent();
            //Icon.Content = pion.Speler.Name;
            Icon.Fill = new SolidColorBrush(pion.Speler.Kleur);
            BorderBrush = new SolidColorBrush(Color.FromRgb(111, 0, 111));   
        }

        public void WisselLicht(bool b)
        {
            BorderThickness = new Thickness(b ? 5 : 0);
        }

        public void Beweeg(IEnumerable<Point> stack)
        {
            foreach (var point in stack.Reverse())
            {
                _queue.Enqueue(point);
            }
            Start();
        }

        private void Start()
        {
            if (!_queue.Any() || _moving)
                return;
            _moving = true;

            var target = _queue.Dequeue();
            var thickness = new Thickness(target.X, target.Y, 0, 0);
            var moveAnimation = new ThicknessAnimation(Margin, thickness, TimeSpan.FromMilliseconds(500))
            {
                FillBehavior = FillBehavior.Stop
            };
            moveAnimation.Completed += (sender, args) =>
                {
                    _moving = false;
                    Margin = thickness;
                    Start();
                };
            
            BeginAnimation(MarginProperty, moveAnimation);
        }
    }
}
