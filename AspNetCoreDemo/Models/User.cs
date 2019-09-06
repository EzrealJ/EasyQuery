using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
