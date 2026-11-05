using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveProductImageAsync(Stream fileStream, string fileName);
        void DeleteFile(string relativeUrl);
    }
}
