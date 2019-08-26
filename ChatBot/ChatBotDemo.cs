using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot
{
    class ChatBotDemo : IChatBot
    {
        const string sqlConnectionString = "Data Source=YY150607;Initial Catalog=ChatBot;Integrated Security=True";
        public MonitorModel FetchResult(int monitorid)
        {
            var query = "select * from monitors where MonitorId=@id";
            var cmd = createCommand(query);
            cmd.Parameters.AddWithValue("@id", monitorid);
            try
            {
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                MonitorModel monitor = new MonitorModel();
                monitor.MonitorId = Convert.ToInt32(reader[0]);
                monitor.MonitorName = reader[1].ToString();
                return monitor;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public List<OptionModel> FetchOptions(int qno)
        {
            var query = "select * from options where QuestionId=@id";
            var cmd = createCommand(query);
            cmd.Parameters.AddWithValue("@id", qno);
            try
            {
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<OptionModel> options = new List<OptionModel>();
                while(reader.Read())
                {
                    var option = new OptionModel();
                    option.OptionId= Convert.ToInt32(reader["OptionId"]);
                    option.QuestionId= Convert.ToInt32(reader["QuestionId"]);
                    option.LinkId = Convert.ToInt32(reader["LinkId"]);
                    option.MonitorId = Convert.ToInt32(reader["MonitorId"]);
                    option.OptionText = reader["OptionText"].ToString();
                }
                return options;
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public QuestionModel FetchQuestion(int qno)
        {
            var query = "select * from questions where QuestionID=@id";
            var cmd = createCommand(query);
            cmd.Parameters.AddWithValue("@id", qno);
            try
            {
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                QuestionModel question = new QuestionModel();
                question.QuestionId = Convert.ToInt32(reader[0]);
                question.QuestionText= reader[1].ToString();
                return question;
            }
            catch ( Exception ex)
            {

                throw ex;
            }
            finally {
                cmd.Connection.Close();
            }
            
           
        }

        public int Process()
        {
            int qno = 1;
            OptionModel optionSelected=null;
            while (qno != 0) {
                QuestionModel question = FetchQuestion(qno);
                Console.WriteLine(question.QuestionText);
                List<OptionModel> options = FetchOptions(qno);
                foreach (var option in options)
                {
                    Console.WriteLine("Select an option");
                    Console.WriteLine("{0}:{1}", option.OptionId, option.OptionText);
                }
                bool valid = false;
                int option_choosen = 0;
                while (!valid)
                {
                    var input = Console.ReadLine();
                    if (int.TryParse(input, out option_choosen))
                    {
                        if (option_choosen > 0 && option_choosen <= options.Count())
                            valid = true;
                        else
                            Console.WriteLine("!!!! Choose the Valid Option !!!!\n");
                    }
                    else
                        Console.WriteLine("!!!! Choose the Valid Option !!!!\n");
                }
                optionSelected = (options.Where(c => c.OptionId == option_choosen).FirstOrDefault());
                qno = optionSelected.LinkId;
            }
            return optionSelected.MonitorId;
          
            

        }
        private SqlCommand createCommand(string query)
        {
            SqlConnection con = new SqlConnection(sqlConnectionString);
            SqlCommand cmd = new SqlCommand(query, con);
            return cmd;
        }
    }
}
