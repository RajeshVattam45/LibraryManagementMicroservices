using HostelService.Domain.Entites;
using HostelService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Domain.Factories
{
    public static class RoomFactory
    {
        public static Room Create ( RoomTypeEnum type, CreateRoomDtoData data )
        {
            return type switch
            {
                RoomTypeEnum.Standard => new Room
                {
                    HostelId = data.HostelId,
                    RoomNumber = data.RoomNumber,
                    FloorNumber = data.FloorNumber,
                    RoomType = "Standard",
                    TotalBeds = data.TotalBeds,
                    OccupiedBeds = data.OccupiedBeds,
                    FeePerBed = data.FeePerBed
                },

                RoomTypeEnum.AC => new Room
                {
                    HostelId = data.HostelId,
                    RoomNumber = data.RoomNumber,
                    FloorNumber = data.FloorNumber,
                    RoomType = "AC",
                    TotalBeds = data.TotalBeds,
                    OccupiedBeds = data.OccupiedBeds,
                    FeePerBed = data.FeePerBed + 200
                },

                RoomTypeEnum.Deluxe => new Room
                {
                    HostelId = data.HostelId,
                    RoomNumber = data.RoomNumber,
                    FloorNumber = data.FloorNumber,
                    RoomType = "Deluxe",
                    TotalBeds = data.TotalBeds,
                    OccupiedBeds = data.OccupiedBeds,
                    FeePerBed = data.FeePerBed + 500
                },

                _ => throw new Exception ( "Invalid room type" )
            };
        }
    }
}
