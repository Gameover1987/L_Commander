using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.OperatingSystem
{
    public interface IApplicationsProvider
    {
        ApplicationModel[] GetAssociatedApplications(string fileExtension);

        ApplicationModel[] GetInstalledApplications();

        ApplicationModel GetSelectedApplication(string fileExtension);

        void SaveSelectedApplication(string fileExtension, ApplicationModel selectedApplication);
    }
}
