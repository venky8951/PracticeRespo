using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ConsoleApp41
{
    public class StreamTradeDataReader : ITradeDataReader
    {
        private Stream _stream;
        public StreamTradeDataReader(Stream stream)
        {
            this._stream = stream;
        }
        public List<string> ReadRowsFromFile()
        {
            var lines = new List<string>();
            using (StreamReader reader = new StreamReader(this._stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

    }
    public class ProcessTrades : IProcessTrades
    {
        private static float lotSize = 100000f;
        public static float LotSize { get => lotSize; set => lotSize = value; }
        public List<TradeRecord> CalculateTrades(List<string> lines)
        {
            var trades = new List<TradeRecord>();
            var lineCount = 1;
            foreach (var line in lines)
            {
                var fields = line.Split(new char[] { ',' });

                if (fields.Length != 3)
                {
                    Console.WriteLine("WARN: Line {0} malformed. Only {1} field(s) found.", lineCount, fields.Length);
                    continue;
                }

                if (fields[0].Length != 6)
                {
                    Console.WriteLine("WARN: Trade currencies on line {0} malformed: '{1}'", lineCount, fields[0]);
                    continue;
                }

                int tradeAmount;
                if (!int.TryParse(fields[1], out tradeAmount))
                {
                    Console.WriteLine("WARN: Trade amount on line {0} not a valid integer:'{1}'", lineCount, fields[1]);
                }

                decimal tradePrice;
                if (!decimal.TryParse(fields[2], out tradePrice))
                {
                    Console.WriteLine("WARN: Trade price on line {0} not a valid decimal:'{1}'", lineCount, fields[2]);
                }

                var sourceCurrencyCode = fields[0].Substring(0, 3);
                var destinationCurrencyCode = fields[0].Substring(3, 3);

                // calculate values
                var trade = new TradeRecord
                {
                    SourceCurrency = sourceCurrencyCode,
                    DestinationCurrency = destinationCurrencyCode,
                    Lots = tradeAmount / lotSize,
                    Price = tradePrice
                };

                trades.Add(trade);

                lineCount++;
            }
            return trades;
        }
    }
    public class DBTradeDataWriter : ITradeDataWriter
    {
        const string conString = "DataSource = (local); Initial Catalog = TradeDatabase; Integrated Security = True";
        public void WriteData(List<TradeRecord> trades)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    foreach (var trade in trades)
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "dbo.insert_trade";
                        command.Parameters.AddWithValue("@sourceCurrency", trade.SourceCurrency);
                        command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
                        command.Parameters.AddWithValue("@lots", trade.Lots);
                        command.Parameters.AddWithValue("@price", trade.Price);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                connection.Close();
            }

            Console.WriteLine("INFO: {0} trades processed", trades.Count);
        }
    }
    public class TradeTransformer
    {
   
        private ITradeDataReader readerRef;
        private ITradeDataWriter writeRef;
        private IProcessTrades processRef;
    
        public TradeTransformer(ITradeDataReader tradeDataReader,ITradeDataWriter tradeDataWriter, IProcessTrades processTrades)
        {
            this.readerRef = tradeDataReader;
            this.writeRef = tradeDataWriter;
            this.processRef = processTrades;
        }
        public void TradeTransform()
        {
            List<string> lines = readerRef.ReadRowsFromFile();
            List<TradeRecord> trades = processRef.CalculateTrades(lines);
            writeRef.WriteData(trades);
        }
    

    }
    public class TradeRecord
    {
        public string SourceCurrency { get; set; }

        public string DestinationCurrency { get; set; }

        public float Lots { get; set; }

        public decimal Price { get; set; }
    }
    public interface ITradeDataReader
    {
        List<string> ReadRowsFromFile();
    }
    public interface IProcessTrades
    {
        List<TradeRecord> CalculateTrades(List<string> lines);
    }
    public interface ITradeDataWriter
    {
        void WriteData(List<TradeRecord> trades);
    }

}
