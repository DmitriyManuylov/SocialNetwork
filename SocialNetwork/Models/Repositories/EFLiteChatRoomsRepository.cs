using SocialNetwork.Models.Exceptions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using SocialNetwork.Models.ViewModels.LiteChatViewModels;
using SocialNetwork.Models.LiteChatModels;

namespace SocialNetwork.Models.Repositories
{
    public class EFLiteChatRoomsRepository: ILiteChatRoomsRepository
    {
        private SocialNetworkDbContext _dbContext;
        public EFLiteChatRoomsRepository(SocialNetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<LiteChatRoom> Rooms => _dbContext.Rooms.ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="LiteChatException"></exception>
        public SimpleMessage SendMessageToLiteChatRoom(int roomId, InSimpleMessageViewModel message)
        {
            LiteChatRoom room = _dbContext.Rooms.FirstOrDefault(room => room.Id == roomId);
            if (room == null) throw new LiteChatException("Комната чата не существует");
            SimpleMessage simpleMessage = new SimpleMessage()
                                            { 
                                                SenderName = message.Sender,
                                                Text = message.Text,
                                                DateTime = DateTime.Now,
                                                RoomId = roomId,
                                                Room = room
                                            };
            _dbContext.SimpleMessages.Add(simpleMessage);
            _dbContext.SaveChanges();
            return simpleMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        /// <exception cref="LiteChatException"></exception>
        public LiteChatRoom CreateLiteChatRoom(string roomName)
        {
            if (roomName == null) throw new LiteChatException("Не задано название комнаты");
            LiteChatRoom room = _dbContext.Rooms.FirstOrDefault(room => room.Name == roomName);
            if (room != null) throw new LiteChatException("Комната чата с таким названием уже существует");
            room = new LiteChatRoom() { Name = roomName };
            _dbContext.Rooms.Add(room);
            _dbContext.SaveChanges();
            return room;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        /// <exception cref="LiteChatException"></exception>
        public LiteChatRoom GetLiteChatRoomById(int roomId)
        {
            LiteChatRoom liteChatRoom = _dbContext.Rooms.FirstOrDefault(room => room.Id == roomId);
            if (liteChatRoom == null) throw new LiteChatException("Комната чата не существует");
            return liteChatRoom;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        /// <exception cref="LiteChatException"></exception>
        public List<OutSimpleMessageViewModel> GetLiteChatMessagesToView(int roomId)
        {
            LiteChatRoom room = _dbContext.Rooms.FirstOrDefault(room => room.Id == roomId);
            if (room == null) throw new LiteChatException("Комната чата не существует");
            List<OutSimpleMessageViewModel> messages;
            messages = _dbContext.SimpleMessages.Where(message => message.RoomId == roomId).Select(message => new OutSimpleMessageViewModel()
            {
                Id = message.Id,
                RoomId = room.Id,
                Sender = message.SenderName,
                Text = message.Text,
                DateTime = message.DateTime.ToString("f")
            }).ToList();        
            return messages;
        }
    }
}
