using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services.Interfaces
{
    public interface IManagementGUIService
    {
        void DrawMenu();
        void CreateEntity();
        void UpdateEntity();
        void DeleteEntity();
        void ListEntity();
        void SearchEntity();
        void ViewEntityDetails();
        void GenerateReportSummary();
        T DialogGetEntityFromList<T>();
    }
}
