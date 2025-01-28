using Banking.Account.Query.Application.Contracts.Persistence;
using Banking.Account.Query.Application.Models;
using Banking.Account.Query.Domain;
using Banking.Cqrs.Core.Events;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Banking.Account.Query.Infrastructure.Consumers
{
    public class BankAccountConsumerService : IHostedService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        public KafkaSettings _kafkaSettings { get; }
        private int numPartitions = 3;              // Número de particiones
        private short replicationFactor = 1;        // Factor de replicación

        public BankAccountConsumerService(IServiceScopeFactory factory)
        {
            _bankAccountRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IBankAccountRepository>();
            _kafkaSettings = (factory.CreateScope().ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>()).Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = _kafkaSettings.GroupId,
                BootstrapServers = $"{_kafkaSettings.Hostname}:{_kafkaSettings.Port}",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    var bankTopics = new string[]
                    {
                        typeof(AccountOpenedEvent).Name,
                        typeof(AccountClosedEvent).Name,
                        typeof(FundsDepositedEvent).Name,
                        typeof(FundsWithdrawnEvent).Name,
                        typeof(UpdateAccountNameEvent).Name,
                    };

                    if (!RegisterTopicsAsync(config, bankTopics).Result)
                    {
                        throw new Exception("Error al crear los topics");
                    }
                    
                    consumerBuilder.Subscribe(bankTopics);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume(cancelToken.Token);


                            if (consumer.Topic == typeof(AccountOpenedEvent).Name)
                            {
                                var accountOpenedEvent = JsonConvert.DeserializeObject<AccountOpenedEvent>(consumer.Message.Value);

                                var bankAccount = new BankAccount
                                {
                                    Identifier = accountOpenedEvent!.Id,
                                    AccountHolder = accountOpenedEvent!.AccountHolder,
                                    AccountType = accountOpenedEvent!.AccountType,
                                    Balance = accountOpenedEvent.OpeningBalance,
                                    CreationDate = accountOpenedEvent.CreatedDate
                                };

                                _bankAccountRepository.AddAsync(bankAccount).Wait();

                            }


                            if (consumer.Topic == typeof(AccountClosedEvent).Name)
                            {
                                var accountClosedEvent = JsonConvert.DeserializeObject<AccountClosedEvent>(consumer.Message.Value);

                                _bankAccountRepository.DeleteByIdentifier(accountClosedEvent!.Id).Wait();

                            }

                            if (consumer.Topic == typeof(FundsDepositedEvent).Name)
                            {
                                var accountDepositEvent = JsonConvert.DeserializeObject<FundsDepositedEvent>(consumer.Message.Value);

                                var bankAccount = new BankAccount
                                {
                                    Identifier = accountDepositEvent!.Id,
                                    Balance = accountDepositEvent.Amount
                                };


                                _bankAccountRepository.DepositBankAccountByIdentifier(bankAccount).Wait();
                            }


                            if (consumer.Topic == typeof(FundsWithdrawnEvent).Name)
                            {
                                var accountWithdrawnEvent = JsonConvert.DeserializeObject<FundsWithdrawnEvent>(consumer.Message.Value);

                                var bankAccount = new BankAccount
                                {
                                    Identifier = accountWithdrawnEvent!.Id,
                                    Balance = accountWithdrawnEvent.Amount
                                };


                                _bankAccountRepository.WithdrawnBankAccountByIdentifier(bankAccount).Wait();
                            }

                            if (consumer.Topic == typeof(UpdateAccountNameEvent).Name)
                            {
                                var updateAccountNameEvent = JsonConvert.DeserializeObject<UpdateAccountNameEvent>(consumer.Message.Value);

                                var bankAccount = new BankAccount
                                {
                                    Identifier = updateAccountNameEvent!.Id,
                                    AccountHolder = updateAccountNameEvent.Name
                                };


                                _bankAccountRepository.UpdateAccountName(bankAccount).Wait();
                            }
                        }
                    }
                    catch (OperationCanceledException) 
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<bool> RegisterTopicsAsync(ConsumerConfig config, string[] topics)
        {
            bool resultRegisterTopics = true;

            using (var adminClient = new AdminClientBuilder(config).Build())
            {
                try
                {
                    // Comprobamos si los topics ya existen
                    var existingTopics = (adminClient.GetMetadata(TimeSpan.FromSeconds(10)))
                                        .Topics.Select(t => t.Topic)
                                        .ToHashSet();

                    // Filtrar los topics que no existen en Kafka
                    var topicsToCreate = topics.Where(topic => !existingTopics.Contains(topic))
                                               .Select(topicName => new TopicSpecification
                                               {
                                                   Name = topicName,
                                                   NumPartitions = numPartitions,
                                                   ReplicationFactor = replicationFactor
                                               }).ToArray();

                    if (topicsToCreate.Length > 0)
                    {
                        try
                        {
                            // Crear los topics que no existen
                            await adminClient.CreateTopicsAsync(topicsToCreate);
                            resultRegisterTopics = true;
                        }
                        catch (CreateTopicsException e)
                        {
                            foreach (var result in e.Results)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error al crear el topic {result.Topic}: {result.Error.Reason}");
                            }
                            resultRegisterTopics = false;
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultRegisterTopics = false;
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                    throw;
                }
            }

            return resultRegisterTopics;
        }
    }
}
