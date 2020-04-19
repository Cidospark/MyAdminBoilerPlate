using Microsoft.AspNetCore.Hosting;
using MyAdminBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Helpers
{
    public class MyUtil
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public MyUtil(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public string FileUpload(EditUserViewModel model)
        {
            string uniqueFileName = null;
            
            // if photo is selected
            if(model.formPhoto != null)
            {
                // get the folder path
                var folderPath = Path.Combine(hostingEnvironment.WebRootPath, "images");

                // get the unique filename
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.formPhoto.FileName;

                // get full path
                var fullPath = Path.Combine(folderPath, uniqueFileName);

                // copy to server
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    model.formPhoto.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
