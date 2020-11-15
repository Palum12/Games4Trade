using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Interfaces.Repositories;
using Games4Trade.Interfaces.Services;

namespace Games4TradeAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int PageSize = 20;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesWithUser(int currentUserId, int selectedUserId, int page)
        {
            var repoResponse =
                await _unitOfWork.Messages.GetMessagesWithReciver(currentUserId, selectedUserId, page, PageSize);
            var result = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageDto>>(repoResponse);
            return result;
        }

        public async Task<OperationResult> AddMessage(int currentUserId, MessagePostDto message)
        {
            var messageModel = new Message()
            {
                Content = message.Content,
                DateCreated = DateTime.Now,
                IsDelivered = false,
                ReceiverId = message.ReceiverId,
                SenderId = currentUserId
            };
            await _unitOfWork.Messages.AddASync(messageModel);
            var response = await _unitOfWork.CompleteASync();
            if (response > 0)
            {
                return new OperationResult(){IsSuccessful = true, Payload = messageModel};
            }
            else
            {
                return OtherServices.GetIncorrectDatabaseConnectionResult();
            }
        }

        public async Task<IEnumerable<NewestMessageDto>> GetNewestMessages(int currentUserId)
        {
            var repoResponse = await _unitOfWork.Messages.GetNewestMessagesForUser(currentUserId);
            var result = new List<NewestMessageDto>();
            foreach (var message in repoResponse)
            {
                var otherUser = new User();   
                if (message.SenderId == currentUserId)
                {
                    var newestMessageSent = new NewestMessageDto
                    {
                        Content = message.Content,
                        IsDelivered = message.IsDelivered,
                        OtherUserId = message.ReceiverId,
                        DateCreated = message.DateCreated
                    };
                    otherUser = await _unitOfWork.Users.GetASync(message.ReceiverId);
                    newestMessageSent.OtherUser = _mapper.Map<User, UserSimpleDto>(otherUser);
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
                    otherUser = await _unitOfWork.Users.GetASync(message.SenderId);
                    newestMessageRecieved.OtherUser = _mapper.Map<User, UserSimpleDto>(otherUser);
                    result.Add(newestMessageRecieved);
                }
            }

            return result;
        }

        public async Task<OperationResult> SetMessagesAsRead(int currentUserId, int selectedUserId)
        {
            var messages =
                await _unitOfWork.Messages
                    .FindASync(m => m.ReceiverId == currentUserId && m.SenderId == selectedUserId && !m.IsDelivered);
            foreach (var message in messages)
            {
                message.IsDelivered = true;
            }
            await _unitOfWork.CompleteASync();
            return new OperationResult{IsSuccessful = true};
        }

        public async Task<bool> CheckIfThereAreNewMessages(int senderId, int reciverId)
        {
            return await _unitOfWork.Messages.CheckIfThereAreNewMessages(senderId, reciverId);
        }
    }
}
