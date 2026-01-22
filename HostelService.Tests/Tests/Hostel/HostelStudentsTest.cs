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
    public class HostelStudentsTest : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HostelStudentsTest (TestWebApplicationFactory factory )
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

        [Fact]
        public async Task CreateHostelStudent_Returns_Created ( )
        {
            // Arrange
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            var dto = new CreateHostelStudentDto
            {
                StudentId = 101,
                HostelId = hostel.Id,
                RoomId = room.Id,
                JoinDate = DateTime.UtcNow
            };

            // Act
            var response = await _client.PostAsync (
                "/api/hostelstudents",
                new StringContent ( JsonConvert.SerializeObject ( dto ), Encoding.UTF8, "application/json" )
            );

            // Assert
            Assert.Equal ( HttpStatusCode.Created, response.StatusCode );
        }

        [Fact]
        public async Task GetHostelStudentById_Returns_OK ( )
        {
            // Arrange
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            var createDto = new CreateHostelStudentDto
            {
                StudentId = 102,
                HostelId = hostel.Id,
                RoomId = room.Id,
                JoinDate = DateTime.UtcNow
            };

            var createResponse = await _client.PostAsync (
                "/api/hostelstudents",
                new StringContent ( JsonConvert.SerializeObject ( createDto ), Encoding.UTF8, "application/json" )
            );

            createResponse.EnsureSuccessStatusCode ();

            var created = JsonConvert.DeserializeObject<HostelStudentDto> (
                await createResponse.Content.ReadAsStringAsync ()
            )!;

            // Act
            var response = await _client.GetAsync ( $"/api/hostelstudents/{created.Id}" );

            // Assert
            Assert.Equal ( HttpStatusCode.OK, response.StatusCode );
        }

        [Fact]
        public async Task GetHostelStudents_ByHostel_Returns_OK ( )
        {
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            await _client.PostAsync (
                "/api/hostelstudents",
                new StringContent ( JsonConvert.SerializeObject ( new CreateHostelStudentDto
                {
                    StudentId = 103,
                    HostelId = hostel.Id,
                    RoomId = room.Id,
                    JoinDate = DateTime.UtcNow
                } ), Encoding.UTF8, "application/json" )
            );

            var response = await _client.GetAsync ( $"/api/hostelstudents/hostel/{hostel.Id}" );

            Assert.Equal ( HttpStatusCode.OK, response.StatusCode );
        }

        [Fact]
        public async Task UpdateHostelStudent_Returns_NoContent ( )
        {
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            var createResponse = await _client.PostAsync (
                "/api/hostelstudents",
                new StringContent ( JsonConvert.SerializeObject ( new CreateHostelStudentDto
                {
                    StudentId = 104,
                    HostelId = hostel.Id,
                    RoomId = room.Id,
                    JoinDate = DateTime.UtcNow
                } ), Encoding.UTF8, "application/json" )
            );

            var created = JsonConvert.DeserializeObject<HostelStudentDto> (
                await createResponse.Content.ReadAsStringAsync ()
            )!;

            var updateDto = new UpdateHostelStudentDto
            {
                RoomId = room.Id,
                LeaveDate = DateTime.UtcNow,
                IsActive = false
            };

            var response = await _client.PutAsync (
                $"/api/hostelstudents/{created.Id}",
                new StringContent ( JsonConvert.SerializeObject ( updateDto ), Encoding.UTF8, "application/json" )
            );

            Assert.Equal ( HttpStatusCode.NoContent, response.StatusCode );
        }

        [Fact]
        public async Task DeleteHostelStudent_Returns_NoContent ( )
        {
            var hostel = await CreateHostelAsync ();
            var room = await CreateRoomAsync ( hostel.Id );

            var createResponse = await _client.PostAsync (
                "/api/hostelstudents",
                new StringContent ( JsonConvert.SerializeObject ( new CreateHostelStudentDto
                {
                    StudentId = 105,
                    HostelId = hostel.Id,
                    RoomId = room.Id,
                    JoinDate = DateTime.UtcNow
                } ), Encoding.UTF8, "application/json" )
            );

            var created = JsonConvert.DeserializeObject<HostelStudentDto> (
                await createResponse.Content.ReadAsStringAsync ()
            )!;

            var response = await _client.DeleteAsync ( $"/api/hostelstudents/{created.Id}" );

            Assert.Equal ( HttpStatusCode.NoContent, response.StatusCode );
        }

    }
}
