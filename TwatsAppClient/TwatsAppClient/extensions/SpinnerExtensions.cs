using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TwatsAppClient.extensions
{
    public static class SpinnerExtensions
    {
        public static void Work(this Grid grid)
        {
            grid.Visibility = System.Windows.Visibility.Visible;
        }
        public static void Stop(this Grid grid)
        {
            grid.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
