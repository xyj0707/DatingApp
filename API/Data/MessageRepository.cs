
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }



        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            // Retrieve messages from the context based on the provided message parameters
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            // Apply filtering based on the message container specified in messageParams
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username 
                && u.RecipientDeleted == false), 
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
                && u.SenderDeleted== false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username 
                && u.RecipientDeleted==false && u.DateRead == null)
            };
            // Project the filtered messages to MessageDto using AutoMapper
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            // Create a paged list of MessageDto using the provided message parameters
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.pageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    m => m.RecipientUsername == currentUserName && m.RecipientDeleted==false &&  m.SenderUsername == recipientUsername || 
                    m.RecipientUsername == recipientUsername && m.SenderDeleted==false &&  m.SenderUsername == currentUserName
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();
            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUserName).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;

                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}