using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Domain
{
    public class IAudit<TPrimaryKey>
    {
        [Key]
        public TPrimaryKey Id { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;
    }

}