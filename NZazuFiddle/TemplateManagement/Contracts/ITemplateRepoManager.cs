using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.TemplateManagement.Contracts
{
    interface ITemplateRepoManager
    {
     
        // File IO interactions
        void LoadTemplateFromFile(Uri pathToTemplate);

        void LoadTemplatesFromFolder(Uri pathToTemplateFolder);

        void ExportTemplateToFile(Uri pathToTemplate);

        void ExportTemplatesToFolder(Uri pathToTemplateFolder);

        // Interactions within NZazuFiddle UI
        void CreateNewEmptyTemplate(string id);

        void RemoveTemplate(string id);

        // Db interactions
        void RemoveTemplateFromDb(string id);

        void UpdateTemplateOnDb(string id);

        void CreateTemplateOnDb(string id);
    
        void GetTemplatesFromDb();

        void setDbConfig(string endpoint);

    }
}
