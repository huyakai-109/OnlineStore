﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class OrderDto
    {
        public string? CustomerName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
