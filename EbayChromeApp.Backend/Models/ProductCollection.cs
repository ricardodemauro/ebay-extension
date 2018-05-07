﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Models
{
    public class ProductCollection : List<Product>
    {
        public ProductCollection(int capacity)
            : base(capacity)
        {

        }

        public ProductCollection()
            : base()
        {

        }
    }
}
