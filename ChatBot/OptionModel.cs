using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot
{
    public class OptionModel
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public int LinkId { get; set; }
        public int MonitorId { get; set; }
        public string OptionText { get; set; }

    }
}
