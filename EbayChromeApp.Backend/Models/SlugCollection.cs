using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Models
{
    public class SlugCollection : List<string>
    {
        public SlugCollection()
            : base()
        {

        }

        public SlugCollection(int capacity)
            : base(capacity)
        {

        }
    }
}
