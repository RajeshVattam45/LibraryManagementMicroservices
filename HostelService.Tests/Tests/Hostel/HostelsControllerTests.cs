using HostelService.Application.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Tests.Tests.Hostel
{
    public class HostelsControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HostelsControllerTests ( TestWebApplicationFactory factory )
        {
            _client = factory.CreateClient();
        }

        private async Task<HostelDto> CreateHostelAsync (
            string hostelName = "Test Hostel",
            string contactNumber = "9000000000",
            string hostelType = "Boys" )
        {
            var dto = new CreateHostelDto
            {
                HostelName = hostelName,
                HostelType = hostelType,
                Description = "Test",
                AddressLine1 = "Street",
                AddressLine2 = "Area",
                City = "Hyderabad",
                State = "TS",
                Pincode = "500001",
                TotalFloors = 3,
                WardenName = "Rajesh",
                ContactNumber = contactNumber
            };

            var response = await _client.PostAsync (
                "/api/hostels",
                new StringContent (
                    JsonConvert.SerializeObject ( dto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            response.EnsureSuccessStatusCode ();

            return JsonConvert.DeserializeObject<HostelDto> (
                await response.Content.ReadAsStringAsync ()
            )!;
        }

        [Fact]
        public async Task GetAll_ReturnsSuccess ( )
        {
            // Act
            var response = await _client.GetAsync ( "/api/hostels" );

            // Assert
            response.EnsureSuccessStatusCode ();
            var content = await response.Content.ReadAsStringAsync ();
            Assert.NotNull ( content );
        }

        [Fact]
        public async Task GetById_Return_Not_Found()
        {
            // ( Arrange ) set
            var nonExistingId = 74777575748;

            // Act (calling the endpoint)
            var response = await _client.GetAsync ( $"api/hostels/{nonExistingId}" );
            
            // Assert (validate)
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode );
        }

        [Fact]
        public async Task GetById_Returns_OK_When_Hostel_Exists ( )
        {
            // Arrange - First, create a hostel to ensure it exists
            var createDto = new CreateHostelDto
            {
                HostelName = $"Test Hostel {Guid.NewGuid ()}",
                HostelType = "Boys",
                Description = "Test description",
                AddressLine1 = "Line 1",
                AddressLine2 = "Line 2",
                City = "Hyderabad",
                State = "TS",
                Pincode = "500001",
                TotalFloors = 3,
                WardenName = "Test Warden",
                ContactNumber = "5456765678"
            };

            // Create the hostel
            var createContent = new StringContent (
                JsonConvert.SerializeObject ( createDto ),
                Encoding.UTF8,
                "application/json"
            );

            // Send create request
            var createResponse = await _client.PostAsync ( "/api/hostels", createContent );
            createResponse.EnsureSuccessStatusCode ();

            // Get the created hostel's ID
            var createdJson = await createResponse.Content.ReadAsStringAsync ();
            // Deserialize to get the ID
            var createdHostel = JsonConvert.DeserializeObject<HostelDto> ( createdJson );

            // Act - Now, get the hostel by ID
            var response = await _client.GetAsync ( $"/api/hostels/{createdHostel.Id}" );

            // Assert - Verify the response
            Assert.Equal ( HttpStatusCode.OK, response.StatusCode );
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_Hostel_Exists ( )
        {
            var hostel = await CreateHostelAsync ();

            var updateDto = new UpdateHostelDto
            {
                Id = hostel.Id,
                HostelName = "Updated Hostel",
                HostelType = "Boys",
                City = "Hyderabad",
                TotalFloors = 5,
                WardenName = "Updated Warden",
                ContactNumber = "7777777777"
            };

            var response = await _client.PutAsync (
                $"/api/hostels/{hostel.Id}",
                JsonContent.Create ( updateDto )
            );

            Assert.Equal ( HttpStatusCode.NoContent, response.StatusCode );
        }

        [Fact]
        public async Task Create_Returns_Created_When_Request_Is_Valid ( )
        {
            // Act
            var hostel = await CreateHostelAsync ();

            // Assert
            Assert.NotNull ( hostel );
            Assert.True ( hostel.Id > 0 );
        }

        [Fact]
        public async Task Create_Returns_BadRequest_When_Model_Is_Invalid ( )
        {
            // Arrange (HostelName missing)
            var dto = new CreateHostelDto
            {
                HostelType = "Boys",
                City = "Hyderabad",
                TotalFloors = 2,
                ContactNumber = "9876543210"
            };

            var json = JsonConvert.SerializeObject ( dto );
            var content = new StringContent ( json, Encoding.UTF8, "application/json" );

            // Act
            var response = await _client.PostAsync ( "/api/hostels", content );

            // Assert
            Assert.Equal ( HttpStatusCode.BadRequest, response.StatusCode );
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_Request_Is_Valid ( )
        {
            // Arrange
            var dto = new UpdateHostelDto
            {
                Id = 1,
                HostelName = "Updated Hostel",
                HostelType = "Boys",
                City = "Hyderabad",
                TotalFloors = 5,
                WardenName = "Updated Warden",
                ContactNumber = "9876543211"
            };

            var json = JsonConvert.SerializeObject ( dto );
            var content = new StringContent ( json, Encoding.UTF8, "application/json" );

            // Act
            var response = await _client.PutAsync ( "/api/hostels/1", content );

            // Assert
            Assert.Equal ( HttpStatusCode.NoContent, response.StatusCode );
        }

        [Fact]
        public async Task Update_Returns_BadRequest_When_Id_Mismatch ( )
        {
            // Arrange
            var dto = new UpdateHostelDto
            {
                Id = 2, // mismatch
                HostelName = "Mismatch Hostel",
                HostelType = "Boys",
                City = "Hyd",
                TotalFloors = 3,
                ContactNumber = "9876543210"
            };

            var json = JsonConvert.SerializeObject ( dto );
            var content = new StringContent ( json, Encoding.UTF8, "application/json" );

            // Act
            var response = await _client.PutAsync ( "/api/hostels/1", content );

            // Assert
            Assert.Equal ( HttpStatusCode.BadRequest, response.StatusCode );
        }

        [Fact]
        public async Task Updare_Return_NorFounf_Record_DoesntExist()
        {
            var dto = new UpdateHostelDto
            {
                Id = 09892,
                HostelName = "Non Existing Hostel",
                HostelType = "Boys",
                City = "Hyd",
                TotalFloors = 3,
                ContactNumber = "9876543210"
            };

            var json = JsonConvert.SerializeObject ( dto );
            var content = new StringContent ( json, Encoding.UTF8, "application/json" );
            // Act
            var response = await _client.PutAsync ( "/api/hostels/09892", content );
            // Assert
            Assert.Equal ( HttpStatusCode.NotFound, response.StatusCode );
        }

        // Validating business logic.
        [Fact]
        public async Task Create_Returns_Error_When_HostelName_Already_Exists ( )
        {
            // Arrange - Create a hostel first
            var existingHostelName = "Unique Hostel Name";
            await CreateHostelAsync ( existingHostelName, "9000000000" );

            // Act - Try to create another hostel with the same name
            var dto = new CreateHostelDto
            {
                HostelName = existingHostelName,
                HostelType = "Boys",
                Description = "Test",
                AddressLine1 = "Street",
                AddressLine2 = "Area",
                City = "Hyderabad",
                State = "TS",
                Pincode = "500001",
                TotalFloors = 3,
                WardenName = "Rajesh",
                ContactNumber = "9000000098"
            };

            var response = await _client.PostAsync (
                "/api/hostels",
                new StringContent (
                    JsonConvert.SerializeObject ( dto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            // Assert
            Assert.Equal ( HttpStatusCode.Conflict, response.StatusCode );

            var body = await response.Content.ReadAsStringAsync ();
            Assert.Contains ( "already exists", body );
        }

        // Validate hostel type
        [Fact]
        public async Task Create_Returns_BadRequest_When_HostelType_Is_Invalid ( )
        {
            var dto = new CreateHostelDto
            {
                HostelName = $"Invalid Type Hostel {Guid.NewGuid ()}",
                HostelType = "Luxury", // Boys, Girls, Mixed.
                Description = "Test",
                AddressLine1 = "Street",
                AddressLine2 = "Area",
                City = "Hyderabad",
                State = "TS",
                Pincode = "500001",
                TotalFloors = 3,
                WardenName = "Rajesh",
                ContactNumber = "9000000004"
            };

            var response = await _client.PostAsync (
                "/api/hostels",
                new StringContent (
                    JsonConvert.SerializeObject ( dto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            Assert.Equal ( HttpStatusCode.BadRequest, response.StatusCode );
        }

        [Fact]
        public async Task Create_Returns_BadRequest_When_Pincode_Is_Invalid ( )
        {
            // Arrange
            var dto = new CreateHostelDto
            {
                HostelName = $"Invalid Pincode Hostel {Guid.NewGuid ()}",
                HostelType = "Boys",
                Description = "Test",
                AddressLine1 = "Street",
                AddressLine2 = "Area",
                City = "Hyderabad",
                State = "TS",
                Pincode = "50A@01", // 6 digits only, no letters/special chars
                TotalFloors = 3,
                WardenName = "Rajesh",
                ContactNumber = "9000000003"
            };

            // Act
            var response = await _client.PostAsync (
                "/api/hostels",
                new StringContent (
                    JsonConvert.SerializeObject ( dto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            // Assert
            Assert.Equal ( HttpStatusCode.BadRequest, response.StatusCode );
        }

        [Fact]
        public async Task Create_Returns_BadRequest_When_ContactNumber_Is_Invalid ( )
        {
            // Arrange
            var dto = new CreateHostelDto
            {
                HostelName = $"Invalid Phone Hostel {Guid.NewGuid ()}",
                HostelType = "Boys",
                Description = "Test",
                AddressLine1 = "Street",
                AddressLine2 = "Area",
                City = "Hyderabad",
                State = "TS",
                Pincode = "500001",
                TotalFloors = 3,
                WardenName = "Rajesh",
                ContactNumber = "9897*8999@" // 10 digits only, no letters/special chars
            };

            // Act
            var response = await _client.PostAsync (
                "/api/hostels",
                new StringContent (
                    JsonConvert.SerializeObject ( dto ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            // Assert
            Assert.Equal ( HttpStatusCode.BadRequest, response.StatusCode );
        }

    }
}
