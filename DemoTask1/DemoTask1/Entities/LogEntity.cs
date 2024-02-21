﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTask1.Entities
{
    public class LogEntity : TableEntity
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
