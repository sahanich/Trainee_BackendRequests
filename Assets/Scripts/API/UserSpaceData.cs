using System;
using System.Collections.Generic;

namespace API
{
    public sealed class UserSpaceData
    {
        public Guid Id { get; set; }
        public Dictionary<int, UserImageData> Images { get; set; }
    }
}
