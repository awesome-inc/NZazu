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

        void ExportTemplate(ISample sample, string pathToFile);

        void ExportTemplates(List<ISample> samples, string pathToFolder);

        ISample LoadTemplateFromFile(string pathToTemplate);

        List<ISample> LoadTemplatesFromFolder(string pathToFolder);

    }
}
