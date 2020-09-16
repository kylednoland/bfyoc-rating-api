using System;
using System.Collections.Generic;
using System.Text;

namespace bfyoc_objects_library
{
    public class Rating
    {
        public Guid id { get; set; }

        public Guid userId { get; set; }

        public Guid productId { get; set; }

        public DateTime timestamp { get; set; }

        public string locationName { get; set; }

        public int rating { get; set; }

        public string usernotes { get; set; }
    }
}
