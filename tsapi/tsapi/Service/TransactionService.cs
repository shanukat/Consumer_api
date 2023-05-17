using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tsapi.Modal;


/// <summary>
/// Base class for implementing Kafka Consumer.
/// </summary>

public class TransactionService
{
    private ConsumerConfig _consumconfig;
    public TransactionService(string bootstrapServers, string topic, string groupId)
    {
        _consumconfig = new ConsumerConfig
        {
            GroupId = groupId,
            BootstrapServers = bootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }


    /// <summary>
    /// Get a list of transaction for arbitrary calender month for a given customer 
    /// </summary>
    /// <param name="UniqueIdentityKey"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Transactions> GetTransactionAsync(string UniqueIdentityKey, int year, int month, CancellationToken cancellationToken)
    {

        var trans = new Transactions();
        decimal exchangeRate = 0;
        
        using (var consumerBuilder = new ConsumerBuilder<Null, string>(_consumconfig).Build())
        {
            consumerBuilder.Subscribe("MyTransactionTopic");
            var cancelToken = new CancellationTokenSource();
            try
            {

                var response = consumerBuilder.Consume(cancelToken.Token);
                var transList = JsonConvert.DeserializeObject<Transactions>(response.Message.Value).TransactionList;

                trans.TransactionList = transList.Where(x => x.UniqueIdentityKey == UniqueIdentityKey && x.ValueDate.Year == year && x.ValueDate.Month == month).ToList();

                //implementing to get the currennt exchange rate from third party provider(external api)

                foreach (var itm in trans.TransactionList)
                {
                    
                    string currency = itm.Currency;
                    exchangeRate = await getCurrentExchangeRate(currency);
                    itm.Amount = exchangeRate * itm.Amount;

                }


            }
            catch (Exception)
            {

                throw;
            }
        }
        return trans;
    }



    private async Task<decimal> getCurrentExchangeRate(string currency)
    {
        decimal exchangeRate = 0;
        using (var httpClient = new HttpClient())
        {
            string api_key = "10fe5a0e1b-45adf088e6-rurlzq";
            string main = "MYR";
            if (!String.IsNullOrEmpty(currency))
            {
                try
                {
                    using (var response = await httpClient.GetAsync("https://api.fastforex.io/fetch-multi?api_key=" + api_key + "&from=" + currency + "&to=" + main))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var userObj = JObject.Parse(apiResponse);
                        var amount = Convert.ToString(userObj["results"]["MYR"]);
                        exchangeRate = Convert.ToDecimal(amount);

                    }
                }
                catch (Exception ex)
                {

                    exchangeRate = 0;
                }
            }
           
        }
        return exchangeRate;

    }


}
