using System;

namespace API
{
    public sealed class UserImageData
    {
        public Guid Id { get; set; } = default;
        public string FileUrl { get; set; } = null;
        public string PreviewUrl { get; set; } = null;
    }
}
