using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement.Contracts
{
    interface ITemplateFileIoDialogService
    {
        void ExportTemplate(ISample sample);

        void ExportTemplates(List<ISample> samples);

        ISample ImportTemplateFromFile();

        List<ISample> ImportTemplatesFromFolder();
    }
}
