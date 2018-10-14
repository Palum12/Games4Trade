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

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult> GetMessagesWithUser(int currentUserId, int selectedUserId, int? page = null)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResult> AddMessage(MessagePostDto message)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResult> GetNewestMessages(IList<NewestMessageDto> messages)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResult> DeleteMessagesForUser(int currentUserId, int selectedUserId)
        {
            throw new System.NotImplementedException();
        }
    }
}
