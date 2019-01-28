using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickRun.SDK
{
    public interface IButton
    {

        string Id { get; }

        string Name { get; }

        void OnInitialized(object sender, EventArgs e);

        event EventHandler Created;

        event EventHandler Initialized;

        event EventHandler Modified;

        event EventHandler Removed;
    }
}
