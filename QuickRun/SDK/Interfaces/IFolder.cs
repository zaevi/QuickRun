using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickRun.SDK
{
    public interface IFolder : IButton
    {
        string Title { get; }

        IEnumerable<IButton> Buttons { get; }

        event EventHandler ItemAdded;

        void AddChild(IButton button);

        void LoadChild(IButton button);
    }
}
