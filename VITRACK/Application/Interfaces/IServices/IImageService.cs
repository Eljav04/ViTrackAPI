using System.Collections.Generic;
using System.Threading.Tasks;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Application.Interfaces;

public interface IImageService
{
    Task<string?> SaveImageAsync(IFormFile image, string folderName);
}
