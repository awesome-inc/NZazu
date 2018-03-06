using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle
{
    interface IFileMenuViewModel : IScreen
    {
        void ImportFile();

        void ImportFiles();

        void ExportToFiles();
    }
}
