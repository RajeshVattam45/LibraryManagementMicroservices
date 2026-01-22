using HostelService.Application.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Tests.Tests.Hostel
{
    public class RoomsTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public RoomsTests ( TestWebApplicationFactory factory )
        {
            _client = factory.CreateClient ();
        }

        // Note - You cannot create a Room without a Hostel.
        // Create a hostel first, then create a room for that hostel.
        private async Task<HostelDto> CreateHostelAsync ( )
        {
            var dto = new CreateHostelDto
            {
                HostelName = $"Hostel {Guid.NewGuid ()}",
                HostelType = "Boys",
                Description = "Test",
                AddressLine1 = "Street",
                AddressLine2 = "Area",
                City = "Hyderabad",
                State = "TS",
                Pincode = "500001",
                TotalFloors = 3,
                WardenName = "Rajesh",
                ContactNumber = "8789098789"
            };

            var response = await _client.PostAsync (
                "/api/hostels",
                new StringContent (
                    JsonConvert.SerializeObject ( dto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            // TEMP: Debug if still failing
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync ();
                throw new Exception ( $"Hostel create failed: {error}" );
            }

            response.EnsureSuccessStatusCode ();

            return JsonConvert.DeserializeObject<HostelDto> (
                await response.Content.ReadAsStringAsync ()
            )!;
        }

        // helper method - Creates a room for the given hostelId
        private async Task<RoomDto> CreateRoomAsync ( int hostelId )
        {
            var dto = new CreateRoomDto
            {
                HostelId = hostelId,
                RoomNumber = $"R-{Guid.NewGuid ().ToString ().Substring ( 0, 4 )}",
                FloorNumber = 1,
                RoomType = "Standard",
                TotalBeds = 4,
                OccupiedBeds = 0,
                FeePerBed = 1500
            };

            var response = await _client.PostAsync (
                "/api/rooms",
                new StringContent (
                    JsonConvert.SerializeObject ( dto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            response.EnsureSuccessStatusCode ();

            return JsonConvert.DeserializeObject<RoomDto> (
                await response.Content.ReadAsStringAsync ()
            )!;
        }

        // Test to create a room
        [Fact]
        public async Task CreateRoom_Returns_Created ( )
        {
            var hostel = await CreateHostelAsync ();

            var room = await CreateRoomAsync ( hostel.Id );

            Assert.NotNull ( room );
            Assert.True ( room.Id > 0 );
        }

        // Test to get a room by ID
        [Fact]
        public async Task GetRoomById_Returns_OK_When_Room_Exists ( )
        {
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            var response = await _client.GetAsync ( $"/api/rooms/{room.Id}" );

            Assert.Equal ( HttpStatusCode.OK, response.StatusCode );
        }

        // Test to get a room by invalid ID
        [Fact]
        public async Task GetRoomById_Returns_NotFound_When_Room_Does_Not_Exist ( )
        {
            var response = await _client.GetAsync ( "/api/rooms/99999" );

            Assert.Equal ( HttpStatusCode.NotFound, response.StatusCode );
        }

        // Test to get rooms by hostel ID
        [Fact]
        public async Task GetRoomsByHostel_Returns_List ( )
        {
            var hostel = await CreateHostelAsync ();
            await CreateRoomAsync ( hostel.Id );

            var response = await _client.GetAsync ( $"/api/rooms/hostel/{hostel.Id}" );

            Assert.Equal ( HttpStatusCode.OK, response.StatusCode );

            var rooms = JsonConvert.DeserializeObject<List<RoomDto>> (
                await response.Content.ReadAsStringAsync ()
            );

            Assert.NotEmpty ( rooms );
        }

        // Test to update a room
        [Fact]
        public async Task UpdateRoom_Returns_NoContent ( )
        {
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            var updateDto = new UpdateRoomDto
            {
                RoomNumber = "Updated-101",
                FloorNumber = 2,
                RoomType = "AC",
                TotalBeds = 4,
                OccupiedBeds = 1,
                FeePerBed = 2000,
                IsActive = true
            };

            var response = await _client.PutAsync (
                $"/api/rooms/{room.Id}",
                new StringContent (
                    JsonConvert.SerializeObject ( updateDto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            Assert.Equal ( HttpStatusCode.NoContent, response.StatusCode );
        }

        // Test to delete a room
        [Fact]
        public async Task DeleteRoom_Returns_NoContent ( )
        {
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            var response = await _client.DeleteAsync ( $"/api/rooms/{room.Id}" );

            Assert.Equal ( HttpStatusCode.NoContent, response.StatusCode );
        }

    }
}
