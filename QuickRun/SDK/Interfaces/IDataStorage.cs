using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickRun.SDK
{
    public interface IDataStorage
    {
        IFolder RootFolder { get; }

        void Save();
    }
}
