using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamCore.Application.Interfaces;

public interface IVideoProcessor
{
    Task<string> CreateThumbnailAsync(string videoPath);
}