﻿using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TheBugTracker.Services.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> EncodeImageAsync(IFormFile file);
        Task<byte[]> EncodeImageAsync(string fileName);
        string DecodeImage(byte[] data, string type);
        string ContentType(IFormFile file);
        int Size(IFormFile file);

    }
}
