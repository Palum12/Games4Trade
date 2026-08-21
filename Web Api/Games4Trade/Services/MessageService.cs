using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Games4TradeAPI.Core.Mapping;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Interfaces.Services;
using Games4TradeAPI.Interfaces.Repositories;

namespace Games4TradeAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository repository;
        private readonly IUserRepository userRepository;
        private const int PageSize = 20;

        public MessageService(IMessageRepository repository, IUserRepository userRepository)
        {
            this.repository = repository;
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesWithUser(int currentUserId, int selectedUserId, int page)
        {
            var repoResponse =
                await repository.GetMessagesWithReciever(currentUserId, selectedUserId, page, PageSize);
            return repoResponse.Select(message => message.ToDto());
        }

        public async Task<OperationResult> AddMessage(int currentUserId, MessagePostDto message)
        {
            var messageModel = new Message()
            {
                Content = message.Content,
                DateCreated = DateTime.UtcNow,
                IsDelivered = false,
                ReceiverId = message.ReceiverId,
                SenderId = currentUserId
            };
            await repository.AddAsync(messageModel);
            var response = await repository.SaveChangesAsync();
            if (response > 0)
            {
                return new OperationResult(){IsSuccessful = true, Payload = messageModel.ToDto()};
            }
            else
            {
                return OtherServices.GetIncorrectDatabaseConnectionResult();
            }
        }

        public async Task<IEnumerable<NewestMessageDto>> GetNewestMessages(int currentUserId)
        {
            var repoResponse = await repository.GetNewestMessagesForUser(currentUserId);
            var result = new List<NewestMessageDto>();
            foreach (var message in repoResponse)
            {
                if (message.SenderId == currentUserId)
                {
                    var newestMessageSent = new NewestMessageDto
                    {
                        Content = message.Content,
                        IsDelivered = message.IsDelivered,
                        OtherUserId = message.ReceiverId,
                        DateCreated = message.DateCreated
                    };
                    var otherUser = await GetExistingUser(message.ReceiverId);
                    newestMessageSent.OtherUser = otherUser.ToSimpleDto();
                    result.Add(newestMessageSent);
                }
                else
                {
                    var newestMessageRecieved = new NewestMessageDto
                    {
                        Content = message.Content,
                        IsDelivered = message.IsDelivered,
                        OtherUserId = message.SenderId,
                        DateCreated = message.DateCreated
                    };
                    var otherUser = await GetExistingUser(message.SenderId);
                    newestMessageRecieved.OtherUser = otherUser.ToSimpleDto();
                    result.Add(newestMessageRecieved);
                }
            }

            return result;
        }

        public async Task<OperationResult> SetMessagesAsRead(int currentUserId, int selectedUserId)
        {
            var messages =
                await repository
                    .FindAsync(m => m.ReceiverId == currentUserId && m.SenderId == selectedUserId && !m.IsDelivered);
            foreach (var message in messages)
            {
                message.IsDelivered = true;
            }
            await repository.SaveChangesAsync();
            return new OperationResult{IsSuccessful = true};
        }

        public async Task<bool> CheckIfThereAreNewMessages(int senderId, int reciverId)
        {
            return await repository.CheckIfThereAreNewMessages(senderId, reciverId);
        }

        private async Task<User> GetExistingUser(int userId)
        {
            return await userRepository.GetAsync(userId)
                ?? throw new InvalidOperationException($"Message references missing user {userId}.");
        }
    }
}
