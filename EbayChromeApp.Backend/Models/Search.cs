using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Models
{
    public class Search
    {
        public int Id { get; set; }

        public string IP { get; set; }

        public DateTime Created { get; set; }

        public string Query { get; set; }
    }
}
