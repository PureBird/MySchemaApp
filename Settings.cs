using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySchemaApp
{
    public class Settings
    {
        public bool CustomizedTable { get; set; } = true;
        public bool StartDateTodayOnAll { get; set; } = false;
        public bool StartDateTodayStartupOnly { get; set; } = true;
        public Settings() { }
    }
}
