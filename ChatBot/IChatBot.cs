using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot
{
    public interface IChatBot
    {
        QuestionModel FetchQuestion(int qno);
        List<OptionModel> FetchOptions(int qno);
        int Process();
        MonitorModel FetchResult(int monitorid);
    }
}
