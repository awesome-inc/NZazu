using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement.Contracts
{
    interface ITemplateFileIo
    {

        void exportTemplate(Uri pathToFile);

        ISample loadTemplate(Uri pathToTemplate);

    }
}
