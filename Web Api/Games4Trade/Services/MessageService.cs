using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;

namespace Games4Trade.Services
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
                ReciverId = message.ReciverId,
                SenderId = currentUserId
            };
            await _unitOfWork.Messages.AddASync(messageModel);
            var response = await _unitOfWork.CompleteASync();
            if (response > 0)
            {
                return new OperationResult(){IsSuccessful = true};
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
                var temp  = new NewestMessageDto
                {
                    Content = message.Content,
                    IsDelivered = message.IsDelivered,
                    ReciverId = message.ReciverId,
                    DateCreated = message.DateCreated
                };
                temp.Reciver = _mapper.Map<User, UserSimpleDto>(message.Reciver);
                result.Add(temp);
            }

            return result;
        }
    }
}
