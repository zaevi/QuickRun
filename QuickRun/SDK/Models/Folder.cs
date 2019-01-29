using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickRun.SDK
{
    public class Folder : BaseFolder
    {

        public List<IButton> ButtonList = new List<IButton>();

        public override IEnumerable<IButton> Buttons => ButtonList;

        public override void LoadChild(IButton button)
        {
            ButtonList.Add(button);
        }

        public override void AddChild(IButton button)
        {
            ButtonList.Add(button);
            ItemAdded?.Invoke(button, EventArgs.Empty);
        }

        public override event EventHandler ItemAdded;
    }
}
