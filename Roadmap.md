PHASE 0: Environment Setup & Project Structure

1. Development Environment Installation:
   a. Visual Studio 2022 Community Edition

   - Download from official Microsoft website
   - Install with these workloads:
     - ASP.NET and web development
     - .NET desktop development
     - Data storage and processing

   b. MySQL Server & Workbench

   - Download MySQL Community Server
   - Download MySQL Workbench
   - Install and configure root password

   c. Additional Tools

   - Install Postman
   - Install Git
   - Create GitHub account if needed

2. Project Setup:
   a. Create new solution 'HippProject'
   b. Create projects:

   ```
   HippProject/
   ├── src/
   │   ├── Hipp.API
   │   │   - ASP.NET Core Web API project
   │   │   - Entry point of the application
   │   │
   │   ├── Hipp.Application
   │   │   - Class Library
   │   │   - Application logic, services
   │   │
   │   ├── Hipp.Domain
   │   │   - Class Library
   │   │   - Domain entities, interfaces
   │   │
   │   ├── Hipp.Infrastructure
   │   │   - Class Library
   │   │   - Data access, external services
   │   │
   │   └── Hipp.Common
   │       - Class Library
   │       - Shared utilities, constants
   │
   └── tests/
       ├── Hipp.UnitTests
       └── Hipp.IntegrationTests
   ```

3. Initial Dependencies (NuGet Packages):
   a. Hipp.API:

   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
   <PackageReference Include="Swashbuckle.AspNetCore" />
   <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
   ```

   b. Hipp.Application:

   ```xml
   <PackageReference Include="AutoMapper" />
   <PackageReference Include="FluentValidation" />
   <PackageReference Include="MediatR" />
   ```

   c. Hipp.Infrastructure:

   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
   <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" />
   <PackageReference Include="Dapper" />
   ```

4. Initial Configuration:
   a. Create appsettings.json structure:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=HippDb;User=root;Password=yourpassword;"
     },
     "JwtSettings": {
       "SecretKey": "your-secret-key",
       "Issuer": "your-issuer",
       "Audience": "your-audience",
       "ExpirationInMinutes": 60
     }
   }
   ```

5. Basic Project Structure Setup:
   a. Domain Layer:

   ```
   Hipp.Domain/
   ├── Entities/
   ├── Interfaces/
   ├── Events/
   └── Exceptions/
   ```

   b. Application Layer:

   ```
   Hipp.Application/
   ├── Common/
   │   ├── Behaviors/
   │   └── Interfaces/
   ├── DTOs/
   ├── Services/
   └── Mappings/
   ```

   c. Infrastructure Layer:

   ```
   Hipp.Infrastructure/
   ├── Data/
   │   ├── Context/
   │   └── Repositories/
   ├── Identity/
   └── Services/
   ```

6. Initial Setup Tasks:
   a. Configure Git:

   ```bash
   git init
   git add .
   git commit -m "Initial project setup"
   ```

   b. Create .gitignore file
   c. Create initial README.md
   d. Setup solution folder structure

PHASE 1: Identity and Authentication System

1. Domain Layer Setup:
   a. Create Base Entities:

   ```csharp
   // Domain/Entities/ApplicationUser.cs
   public class ApplicationUser : IdentityUser
   {
       public string FirstName { get; set; }
       public string LastName { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime? LastLogin { get; set; }
   }

   // Role-specific entities
   public class Menaxher { ... }
   public class Komercialist { ... }
   public class Shofer { ... }
   public class Etiketues { ... }
   ```

   b. Create Interfaces:

   ```csharp
   public interface IUserRepository
   {
       Task<ApplicationUser> GetByIdAsync(string id);
       Task<bool> CreateAsync(ApplicationUser user, string password);
       // Other methods
   }
   ```

2. Infrastructure Layer Implementation:
   a. Create DbContext:

   ```csharp
   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
   {
       public DbSet<Menaxher> Menaxhers { get; set; }
       public DbSet<Komercialist> Komercialists { get; set; }
       public DbSet<Shofer> Shofers { get; set; }
       public DbSet<Etiketues> Etiketueses { get; set; }

       protected override void OnModelCreating(ModelBuilder builder)
       {
           base.OnModelCreating(builder);
           // Configure relationships
       }
   }
   ```

   b. Create Stored Procedures:

   ```sql
   -- User operations
   CREATE PROCEDURE CreateUser ...
   CREATE PROCEDURE UpdateUser ...
   CREATE PROCEDURE GetUserById ...
   ```

   c. Implement Repositories:

   ```csharp
   public class UserRepository : IUserRepository
   {
       private readonly ApplicationDbContext _context;
       private readonly IDbConnection _connection;

       // Implement interface methods using Dapper for stored procedures
   }
   ```

3. Application Layer Setup:
   a. Create DTOs:

   ```csharp
   public class LoginDto
   {
       public string Email { get; set; }
       public string Password { get; set; }
   }

   public class RegisterUserDto
   {
       public string Email { get; set; }
       public string Password { get; set; }
       public string FirstName { get; set; }
       public string LastName { get; set; }
       public string Role { get; set; }
   }
   ```

   b. Implement Services:

   ```csharp
   public interface IAuthService
   {
       Task<string> LoginAsync(LoginDto model);
       Task<bool> RegisterAsync(RegisterUserDto model);
   }

   public class AuthService : IAuthService
   {
       private readonly UserManager<ApplicationUser> _userManager;
       private readonly ITokenService _tokenService;
       // Implement methods
   }
   ```

4. API Layer Implementation:
   a. Configure Startup.cs:

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       // Configure JWT
       // Configure Identity
       // Configure DI
   }
   ```

   b. Create Controllers:

   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class AuthController : ControllerBase
   {
       private readonly IAuthService _authService;

       [HttpPost("login")]
       public async Task<IActionResult> Login(LoginDto model) { ... }

       [HttpPost("register")]
       [Authorize(Roles = "Admin")]
       public async Task<IActionResult> Register(RegisterUserDto model) { ... }
   }
   ```

5. Testing Setup:
   a. Create test projects
   b. Write unit tests for services
   c. Write integration tests for API endpoints

6. Initial Admin Seeding:
   a. Create database migrations
   b. Create admin user seed
   c. Create roles seed

7. Manual Testing:

   a. Test with Swagger:

   - Login endpoint
   - Register endpoint
   - Role-based access

   b. Test with Postman:

   - Create collection
   - Test all endpoints
   - Verify JWT functionality

PHASE 2: Product Management and Menaxher Functionality

1. Cloud Storage Setup:
   a. Cloudinary Integration:

   ```csharp
   // Configuration in .env
   CLOUDINARY_CLOUD_NAME=your_cloud_name
   CLOUDINARY_API_KEY=your_api_key
   CLOUDINARY_API_SECRET=your_api_secret

   // IImageService interface
   public interface IImageService
   {
       Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file);
       Task DeleteImageAsync(string publicId);
   }
   ```

   b. Required NuGet Packages:

   ```xml
   <PackageReference Include="CloudinaryDotNet" Version="1.25.0" />
   ```

2. Domain Layer Setup:
   a. Create Product Entities:

   ```csharp
   public class Product
   {
       public string Id { get; set; }
       public string ProductCode { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public string Ingredients { get; set; }
       public string ImageUrl { get; set; }        // Cloudinary URL
       public string ImagePublicId { get; set; }   // Cloudinary Public ID
       public decimal TotalQuantity { get; set; }
       public decimal UnlabeledQuantity { get; set; }
       public decimal LabeledQuantity { get; set; }
       public bool IsPriority { get; set; }
       public string MenaxherId { get; set; }
       public Menaxher Menaxher { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime? UpdatedAt { get; set; }
   }

   public class ProductInventoryLog
   {
       public string Id { get; set; }
       public string ProductId { get; set; }
       public string MenaxherId { get; set; }
       public decimal Quantity { get; set; }
       public string Type { get; set; } // "IN" or "OUT"
       public DateTime CreatedAt { get; set; }
       public string Notes { get; set; }
   }
   ```

   b. Create Interfaces:

   ```csharp
   public interface IProductRepository
   {
       Task<Product> GetByIdAsync(string id);
       Task<IEnumerable<Product>> GetAllAsync();
       Task<IEnumerable<Product>> GetUnlabeledProductsAsync();
       Task<bool> CreateAsync(Product product);
       Task<bool> UpdateAsync(Product product);
       Task<bool> DeleteAsync(string id);
       Task<bool> UpdateQuantitiesAsync(string id, decimal labeled, decimal unlabeled);
   }
   ```

3. Infrastructure Layer:
   a. Update DbContext:

   ```csharp
   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
   {
       public DbSet<Product> Products { get; set; }
       public DbSet<ProductInventoryLog> ProductInventoryLogs { get; set; }

       protected override void OnModelCreating(ModelBuilder builder)
       {
           builder.Entity<Product>()
               .HasOne(p => p.Menaxher)
               .WithMany(m => m.Products)
               .HasForeignKey(p => p.MenaxherId);
       }
   }
   ```

   b. Create Stored Procedures:

   ```sql
   CREATE PROCEDURE CreateProduct(
       IN p_Id VARCHAR(36),
       IN p_ProductCode VARCHAR(50),
       IN p_Name VARCHAR(100),
       IN p_Description TEXT,
       IN p_Ingredients TEXT,
       IN p_ImageUrl VARCHAR(500),
       IN p_ImagePublicId VARCHAR(100),
       IN p_TotalQuantity DECIMAL(18,2),
       IN p_MenaxherId VARCHAR(36)
   )
   BEGIN
       INSERT INTO Products (
           Id, ProductCode, Name, Description, Ingredients,
           ImageUrl, ImagePublicId, TotalQuantity, UnlabeledQuantity,
           MenaxherId, CreatedAt
       )
       VALUES (
           p_Id, p_ProductCode, p_Name, p_Description, p_Ingredients,
           p_ImageUrl, p_ImagePublicId, p_TotalQuantity, p_TotalQuantity,
           p_MenaxherId, UTC_TIMESTAMP()
       );
   END
   ```

4. Application Layer:
   a. Create DTOs:

   ```csharp
   public class ProductDto
   {
       public string Id { get; set; }
       public string ProductCode { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public string Ingredients { get; set; }
       public string ImageUrl { get; set; }
       public decimal TotalQuantity { get; set; }
       public decimal UnlabeledQuantity { get; set; }
       public decimal LabeledQuantity { get; set; }
       public bool IsPriority { get; set; }
   }

   public class CreateProductDto
   {
       public string ProductCode { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public string Ingredients { get; set; }
       public IFormFile Image { get; set; }
       public decimal InitialQuantity { get; set; }
       public bool IsPriority { get; set; }
   }
   ```

   b. Create Services:

   ```csharp
   public interface IProductService
   {
       Task<ProductDto> GetByIdAsync(string id);
       Task<IEnumerable<ProductDto>> GetAllAsync();
       Task<ProductDto> CreateAsync(CreateProductDto dto, string menaxherId);
       Task<ProductDto> UpdateAsync(string id, UpdateProductDto dto);
       Task<bool> DeleteAsync(string id);
       Task<bool> UpdateQuantitiesAsync(string id, UpdateQuantitiesDto dto);
   }

   public class ProductService : IProductService
   {
       private readonly IProductRepository _productRepository;
       private readonly IImageService _imageService;
       private readonly IMapper _mapper;

       // Implementation
   }
   ```

5. API Layer:
   a. Create Controllers:

   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class ProductsController : ControllerBase
   {
       private readonly IProductService _productService;
       private readonly IImageService _imageService;

       [HttpPost]
       [Authorize(Roles = "Menaxher")]
       public async Task<ActionResult<ProductDto>> Create([FromForm] CreateProductDto dto)

       [HttpPut("{id}")]
       [Authorize(Roles = "Menaxher")]
       public async Task<ActionResult<ProductDto>> Update(string id, [FromForm] UpdateProductDto dto)

       [HttpDelete("{id}")]
       [Authorize(Roles = "Menaxher")]
       public async Task<ActionResult> Delete(string id)
   }
   ```

6. Testing:
   a. Unit Tests:

   - Product service tests
   - Image service tests (with mocked Cloudinary)
   - Validation tests

   b. Integration Tests:

   - Product API endpoints
   - Image upload to Cloudinary
   - Error handling for failed uploads

7. Manual Testing with Postman:
   a. Create test collection for Products
   b. Test image upload scenarios
   c. Test CRUD operations
   d. Test role-based access
   e. Verify Cloudinary integration

PHASE 3: Labeling System and Etiketues Functionality

1. Domain Layer Setup:
   a. Create Labeling Entities:

   ```csharp
   public class LabelingTask
   {
       public string Id { get; set; }
       public string ProductId { get; set; }
       public Product Product { get; set; }
       public string EtiketuesId { get; set; }
       public Etiketues Etiketues { get; set; }
       public DateTime StartTime { get; set; }
       public DateTime? EndTime { get; set; }
       public decimal QuantityLabeled { get; set; }
       public string Status { get; set; } // "Started", "Finished", "Verified"
       public bool IsVerified { get; set; }
       public string VerifiedById { get; set; }
       public Menaxher VerifiedBy { get; set; }
       public DateTime? VerifiedAt { get; set; }
       public string Notes { get; set; }
   }

   public class LabelingTaskLog
   {
       public string Id { get; set; }
       public string TaskId { get; set; }
       public string Action { get; set; } // "Started", "Finished", "Verified"
       public string UserId { get; set; }
       public DateTime Timestamp { get; set; }
       public string Details { get; set; }
   }
   ```

   b. Create Interfaces:

   ```csharp
   public interface ILabelingTaskRepository
   {
       Task<LabelingTask> GetByIdAsync(string id);
       Task<IEnumerable<LabelingTask>> GetActiveTasksAsync();
       Task<IEnumerable<LabelingTask>> GetTasksByEtiketuesAsync(string etiketuesId);
       Task<IEnumerable<LabelingTask>> GetUnverifiedTasksAsync();
       Task<bool> CreateAsync(LabelingTask task);
       Task<bool> UpdateAsync(LabelingTask task);
       Task<bool> VerifyTaskAsync(string id, string menaxherId);
   }
   ```

2. Infrastructure Layer:
   a. Update DbContext:

   ```csharp
   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
   {
       public DbSet<LabelingTask> LabelingTasks { get; set; }
       public DbSet<LabelingTaskLog> LabelingTaskLogs { get; set; }

       protected override void OnModelCreating(ModelBuilder builder)
       {
           builder.Entity<LabelingTask>()
               .HasOne(lt => lt.Product)
               .WithMany()
               .HasForeignKey(lt => lt.ProductId);

           builder.Entity<LabelingTask>()
               .HasOne(lt => lt.Etiketues)
               .WithMany(e => e.LabelingTasks)
               .HasForeignKey(lt => lt.EtiketuesId);
       }
   }
   ```

   b. Create Stored Procedures:

   ```sql
   DELIMITER //

   CREATE PROCEDURE StartLabelingTask(
       IN p_TaskId VARCHAR(36),
       IN p_ProductId VARCHAR(36),
       IN p_EtiketuesId VARCHAR(36)
   )
   BEGIN
       START TRANSACTION;

       INSERT INTO LabelingTasks (Id, ProductId, EtiketuesId, StartTime, Status)
       VALUES (p_TaskId, p_ProductId, p_EtiketuesId, NOW(), 'Started');

       INSERT INTO LabelingTaskLogs (Id, TaskId, Action, UserId, Timestamp)
       VALUES (UUID(), p_TaskId, 'Started', p_EtiketuesId, NOW());

       COMMIT;
   END //

   CREATE PROCEDURE FinishLabelingTask(
       IN p_TaskId VARCHAR(36),
       IN p_QuantityLabeled DECIMAL(18,2)
   )
   BEGIN
       -- Implementation
   END //

   DELIMITER ;
   ```

3. Application Layer:
   a. Create DTOs:

   ```csharp
   public class LabelingTaskDto
   {
       public string Id { get; set; }
       public string ProductId { get; set; }
       public string ProductName { get; set; }
       public decimal QuantityLabeled { get; set; }
       public string Status { get; set; }
       public DateTime StartTime { get; set; }
       public DateTime? EndTime { get; set; }
   }

   public class StartLabelingTaskDto
   {
       public string ProductId { get; set; }
   }

   public class FinishLabelingTaskDto
   {
       public decimal QuantityLabeled { get; set; }
       public string Notes { get; set; }
   }
   ```

   b. Create Services:

   ```csharp
   public interface ILabelingService
   {
       Task<LabelingTaskDto> StartTaskAsync(string etiketuesId, StartLabelingTaskDto dto);
       Task<LabelingTaskDto> FinishTaskAsync(string taskId, FinishLabelingTaskDto dto);
       Task<LabelingTaskDto> VerifyTaskAsync(string taskId, string menaxherId);
       Task<IEnumerable<LabelingTaskDto>> GetActiveTasksAsync();
       Task<IEnumerable<LabelingTaskDto>> GetUnverifiedTasksAsync();
   }

   public class LabelingService : ILabelingService
   {
       private readonly ILabelingTaskRepository _labelingTaskRepository;
       private readonly IProductRepository _productRepository;
       private readonly IMapper _mapper;

       // Implementation
   }
   ```

4. API Layer:
   a. Create Controllers:

   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class LabelingController : ControllerBase
   {
       private readonly ILabelingService _labelingService;

       [HttpPost("start")]
       [Authorize(Roles = "Etiketues")]
       public async Task<ActionResult<LabelingTaskDto>> StartTask(StartLabelingTaskDto dto)

       [HttpPost("{taskId}/finish")]
       [Authorize(Roles = "Etiketues")]
       public async Task<ActionResult<LabelingTaskDto>> FinishTask(string taskId, FinishLabelingTaskDto dto)

       [HttpPost("{taskId}/verify")]
       [Authorize(Roles = "Menaxher")]
       public async Task<ActionResult<LabelingTaskDto>> VerifyTask(string taskId)

       [HttpGet("active")]
       [Authorize(Roles = "Etiketues")]
       public async Task<ActionResult<IEnumerable<LabelingTaskDto>>> GetActiveTasks()

       [HttpGet("unverified")]
       [Authorize(Roles = "Menaxher")]
       public async Task<ActionResult<IEnumerable<LabelingTaskDto>>> GetUnverifiedTasks()
   }
   ```

5. Business Rules Implementation:
   a. Priority Rules:

   ```csharp
   public class LabelingBusinessRules
   {
       public static bool CanStartLabeling(Product product)
       {
           if (product.IsPriority)
               return true;

           return !AnyPriorityProductsNeedLabeling();
       }

       public static bool ValidateQuantity(decimal unlabeledQuantity, decimal requestedQuantity)
       {
           return requestedQuantity <= unlabeledQuantity;
       }
   }
   ```

6. Testing:
   a. Unit Tests:

   - Labeling service tests
   - Business rules tests
   - Validation tests

   b. Integration Tests:

   - Labeling API endpoints
   - Priority rules
   - Quantity validation

7. Manual Testing:
   a. Test Scenarios:
   - Start labeling task
   - Finish labeling task
   - Verify completed tasks
   - Priority product rules
   - Multiple active tasks

PHASE 4: Client Management and Komercialist Functionality

1. Domain Layer Setup:
   a. Create Client Entities:

   ```csharp
   public class Client
   {
       public string Id { get; set; }
       public string BusinessName { get; set; }
       public string Address { get; set; }
       public string Region { get; set; }
       public string SubRegion { get; set; }
       public double Latitude { get; set; }
       public double Longitude { get; set; }
       public string ContactPerson { get; set; }
       public string ContactPhone { get; set; }
       public string KomercialistId { get; set; }
       public Komercialist Komercialist { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime? UpdatedAt { get; set; }
       public ICollection<Order> Orders { get; set; }
   }

   public class Region
   {
       public string Id { get; set; }
       public string Name { get; set; }
       public ICollection<SubRegion> SubRegions { get; set; }
   }

   public class SubRegion
   {
       public string Id { get; set; }
       public string RegionId { get; set; }
       public string Name { get; set; }
       public Region Region { get; set; }
   }
   ```

   b. Create Interfaces:

   ```csharp
   public interface IClientRepository
   {
       Task<Client> GetByIdAsync(string id);
       Task<IEnumerable<Client>> GetByRegionAsync(string region);
       Task<IEnumerable<Client>> GetByKomercialistAsync(string komercialistId);
       Task<bool> CreateAsync(Client client);
       Task<bool> UpdateAsync(Client client);
       Task<bool> DeleteAsync(string id);
   }

   public interface IRegionRepository
   {
       Task<IEnumerable<Region>> GetAllRegionsAsync();
       Task<IEnumerable<SubRegion>> GetSubRegionsByRegionAsync(string regionId);
   }
   ```

2. Infrastructure Layer:
   a. Update DbContext:

   ```csharp
   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
   {
       public DbSet<Client> Clients { get; set; }
       public DbSet<Region> Regions { get; set; }
       public DbSet<SubRegion> SubRegions { get; set; }

       protected override void OnModelCreating(ModelBuilder builder)
       {
           builder.Entity<Client>()
               .HasOne(c => c.Komercialist)
               .WithMany(k => k.Clients)
               .HasForeignKey(c => c.KomercialistId);

           builder.Entity<SubRegion>()
               .HasOne(sr => sr.Region)
               .WithMany(r => r.SubRegions)
               .HasForeignKey(sr => sr.RegionId);
       }
   }
   ```

   b. Create Stored Procedures:

   ```sql
   DELIMITER //

   CREATE PROCEDURE CreateClient(
       IN p_Id VARCHAR(36),
       IN p_BusinessName VARCHAR(100),
       IN p_Address VARCHAR(200),
       IN p_Region VARCHAR(50),
       IN p_SubRegion VARCHAR(50),
       IN p_Latitude DOUBLE,
       IN p_Longitude DOUBLE,
       IN p_ContactPerson VARCHAR(100),
       IN p_ContactPhone VARCHAR(20),
       IN p_KomercialistId VARCHAR(36)
   )
   BEGIN
       INSERT INTO Clients (
           Id, BusinessName, Address, Region, SubRegion,
           Latitude, Longitude, ContactPerson, ContactPhone,
           KomercialistId, CreatedAt
       )
       VALUES (
           p_Id, p_BusinessName, p_Address, p_Region, p_SubRegion,
           p_Latitude, p_Longitude, p_ContactPerson, p_ContactPhone,
           p_KomercialistId, NOW()
       );
   END //

   CREATE PROCEDURE GetClientsByRegion(
       IN p_Region VARCHAR(50)
   )
   BEGIN
       SELECT * FROM Clients
       WHERE Region = p_Region;
   END //

   DELIMITER ;
   ```

3. Application Layer:
   a. Create DTOs:

   ```csharp
   public class ClientDto
   {
       public string Id { get; set; }
       public string BusinessName { get; set; }
       public string Address { get; set; }
       public string Region { get; set; }
       public string SubRegion { get; set; }
       public LocationDto Location { get; set; }
       public string ContactPerson { get; set; }
       public string ContactPhone { get; set; }
   }

   public class CreateClientDto
   {
       public string BusinessName { get; set; }
       public string Address { get; set; }
       public string Region { get; set; }
       public string SubRegion { get; set; }
       public double Latitude { get; set; }
       public double Longitude { get; set; }
       public string ContactPerson { get; set; }
       public string ContactPhone { get; set; }
   }

   public class LocationDto
   {
       public double Latitude { get; set; }
       public double Longitude { get; set; }
   }
   ```

   b. Create Services:

   ```csharp
   public interface IClientService
   {
       Task<ClientDto> GetByIdAsync(string id);
       Task<IEnumerable<ClientDto>> GetByRegionAsync(string region);
       Task<ClientDto> CreateAsync(string komercialistId, CreateClientDto dto);
       Task<ClientDto> UpdateAsync(string id, UpdateClientDto dto);
       Task<bool> DeleteAsync(string id);
   }

   public class ClientService : IClientService
   {
       private readonly IClientRepository _clientRepository;
       private readonly IGoogleMapsService _googleMapsService;
       private readonly IMapper _mapper;

       // Implementation
   }

   public interface IGoogleMapsService
   {
       Task<LocationDto> GeocodeAddressAsync(string address);
   }
   ```

4. API Layer:
   a. Create Controllers:

   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class ClientsController : ControllerBase
   {
       private readonly IClientService _clientService;

       [HttpGet]
       [Authorize(Roles = "Komercialist,Menaxher")]
       public async Task<ActionResult<IEnumerable<ClientDto>>> GetByRegion([FromQuery] string region)

       [HttpGet("{id}")]
       [Authorize(Roles = "Komercialist,Menaxher,Shofer")]
       public async Task<ActionResult<ClientDto>> GetById(string id)

       [HttpPost]
       [Authorize(Roles = "Komercialist")]
       public async Task<ActionResult<ClientDto>> Create(CreateClientDto dto)

       [HttpPut("{id}")]
       [Authorize(Roles = "Komercialist")]
       public async Task<ActionResult<ClientDto>> Update(string id, UpdateClientDto dto)
   }
   ```

5. Google Maps Integration:
   a. Implement Google Maps Service:

   ```csharp
   public class GoogleMapsService : IGoogleMapsService
   {
       private readonly string _apiKey;
       private readonly HttpClient _httpClient;

       public async Task<LocationDto> GeocodeAddressAsync(string address)
       {
           // Implementation using Google Maps Geocoding API
       }
   }
   ```

6. Testing:
   a. Unit Tests:

   - Client service tests
   - Google Maps service tests
   - Validation tests

   b. Integration Tests:

   - Client API endpoints
   - Geocoding functionality
   - Region-based queries

7. Manual Testing:
   a. Test Scenarios:
   - Create client with address geocoding
   - Update client information
   - Query clients by region
   - Verify location data

PHASE 5: Order Management and Delivery System

1. Domain Layer Setup:
   2| a. Create Order Entities:
   3| `csharp
4|      public class Order
5|      {
6|          public string Id { get; set; }
7|          public string ClientId { get; set; }
8|          public Client Client { get; set; }
9|          public string KomercialistId { get; set; }
10|          public Komercialist Komercialist { get; set; }
11|          public DateTime OrderDate { get; set; }
12|          public string Status { get; set; } // Pending, OnDelivery, Delivered, Cancelled
13|          public string PaymentMethod { get; set; }
14|          public string CancellationReason { get; set; }
15|          public ICollection<OrderItem> OrderItems { get; set; }
16|          public string DeliveryRouteId { get; set; }
17|          public DeliveryRoute DeliveryRoute { get; set; }
18|          public int DeliveryOrder { get; set; } // Order within route
19|      }
20|
21|      public class OrderItem
22|      {
23|          public string Id { get; set; }
24|          public string OrderId { get; set; }
25|          public Order Order { get; set; }
26|          public string ProductId { get; set; }
27|          public Product Product { get; set; }
28|          public decimal Quantity { get; set; }
29|          public decimal Price { get; set; }
30|      }
31|
32|      public class DeliveryRoute
33|      {
34|          public string Id { get; set; }
35|          public string Region { get; set; }
36|          public string SubRegion { get; set; }
37|          public DateTime DeliveryDate { get; set; }
38|          public string Status { get; set; }
39|          public string ShoferId { get; set; }
40|          public Shofer Shofer { get; set; }
41|          public int OrderCount { get; set; }
42|          public ICollection<Order> Orders { get; set; }
43|      }
44|      `
   45|
   46| b. Create Interfaces:
   47| `csharp
48|      public interface IOrderRepository
49|      {
50|          Task<Order> GetByIdAsync(string id);
51|          Task<IEnumerable<Order>> GetByRegionAndDateAsync(string region, DateTime date);
52|          Task<IEnumerable<Order>> GetByRouteAsync(string routeId);
53|          Task<bool> CreateAsync(Order order);
54|          Task<bool> UpdateStatusAsync(string id, string status, string reason = null);
55|      }
56|
57|      public interface IDeliveryRouteRepository
58|      {
59|          Task<DeliveryRoute> GetByIdAsync(string id);
60|          Task<IEnumerable<DeliveryRoute>> GetByDateAsync(DateTime date);
61|          Task<bool> CreateAsync(DeliveryRoute route);
62|          Task<bool> AssignDriverAsync(string routeId, string shoferId);
63|          Task<bool> UpdateOrderSequenceAsync(string routeId, Dictionary<string, int> orderSequence);
64|      }
65|      `
   66|
   67|2. Infrastructure Layer:
   68| a. Update DbContext:
   69| `csharp
70|      public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
71|      {
72|          public DbSet<Order> Orders { get; set; }
73|          public DbSet<OrderItem> OrderItems { get; set; }
74|          public DbSet<DeliveryRoute> DeliveryRoutes { get; set; }
75|
76|          protected override void OnModelCreating(ModelBuilder builder)
77|          {
78|              builder.Entity<Order>()
79|                  .HasOne(o => o.DeliveryRoute)
80|                  .WithMany(dr => dr.Orders)
81|                  .HasForeignKey(o => o.DeliveryRouteId);
82|
83|              builder.Entity<OrderItem>()
84|                  .HasOne(oi => oi.Order)
85|                  .WithMany(o => o.OrderItems)
86|                  .HasForeignKey(oi => oi.OrderId);
87|          }
88|      }
89|      `
   90|
   91| b. Create Stored Procedures:
   92| `sql
93|      DELIMITER //
94|
95|      CREATE PROCEDURE CreateOrder(
96|          IN p_Id VARCHAR(36),
97|          IN p_ClientId VARCHAR(36),
98|          IN p_KomercialistId VARCHAR(36),
99|          IN p_Region VARCHAR(50)
100|      )
101|      BEGIN
102|          DECLARE v_OrderCount INT;
103|
104|          -- Get order count for region and today
105|          SELECT COUNT(*) INTO v_OrderCount
106|          FROM Orders o
107|          WHERE o.Region = p_Region
108|          AND DATE(o.OrderDate) = CURDATE();
109|
110|          -- Create order
111|          INSERT INTO Orders (Id, ClientId, KomercialistId, OrderDate, Status)
112|          VALUES (p_Id, p_ClientId, p_KomercialistId, NOW(), 'Pending');
113|
114|          -- Auto-assign to route based on count
115|          IF v_OrderCount <= 60 THEN
116|              -- Logic for 2 routes
117|          ELSE
118|              -- Logic for 3 routes
119|          END IF;
120|      END //
121|
122|      CREATE PROCEDURE GenerateDeliveryRoutes(
123|          IN p_Region VARCHAR(50),
124|          IN p_DeliveryDate DATE
125|      )
126|      BEGIN
127|          -- Implementation for route generation
128|      END //
129|
130|      DELIMITER ;
131|      `
   132|
   133|3. Application Layer:
   134| a. Create DTOs:
   135| `csharp
136|      public class OrderDto
137|      {
138|          public string Id { get; set; }
139|          public ClientDto Client { get; set; }
140|          public DateTime OrderDate { get; set; }
141|          public string Status { get; set; }
142|          public string PaymentMethod { get; set; }
143|          public List<OrderItemDto> Items { get; set; }
144|      }
145|
146|      public class CreateOrderDto
147|      {
148|          public string ClientId { get; set; }
149|          public List<OrderItemCreateDto> Items { get; set; }
150|      }
151|
152|      public class DeliveryRouteDto
153|      {
154|          public string Id { get; set; }
155|          public string Region { get; set; }
156|          public string SubRegion { get; set; }
157|          public DateTime DeliveryDate { get; set; }
158|          public List<OrderDto> Orders { get; set; }
159|          public int TotalOrders { get; set; }
160|      }
161|      `
   162|
   163| b. Create Services:
   164| `csharp
165|      public interface IOrderService
166|      {
167|          Task<OrderDto> CreateOrderAsync(string komercialistId, CreateOrderDto dto);
168|          Task<OrderDto> UpdateStatusAsync(string id, string status, string reason);
169|          Task<IEnumerable<OrderDto>> GetByRouteAsync(string routeId);
170|      }
171|
172|      public interface IDeliveryRouteService
173|      {
174|          Task<DeliveryRouteDto> GetRouteAsync(string id);
175|          Task<IEnumerable<DeliveryRouteDto>> GetAvailableRoutesAsync();
176|          Task<DeliveryRouteDto> AssignDriverAsync(string routeId, string shoferId);
177|          Task<DeliveryRouteDto> UpdateOrderSequenceAsync(string routeId, UpdateRouteSequenceDto dto);
178|      }
179|      `
   180|
   181|4. API Layer:
   182| a. Create Controllers:
   183| `csharp
184|      [ApiController]
185|      [Route("api/[controller]")]
186|      [Authorize]
187|      public class OrdersController : ControllerBase
188|      {
189|          [HttpPost]
190|          [Authorize(Roles = "Komercialist")]
191|          public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
192|
193|          [HttpPut("{id}/status")]
194|          [Authorize(Roles = "Shofer")]
195|          public async Task<ActionResult<OrderDto>> UpdateStatus(string id, UpdateOrderStatusDto dto)
196|      }
197|
198|      [ApiController]
199|      [Route("api/[controller]")]
200|      [Authorize]
201|      public class DeliveryRoutesController : ControllerBase
202|      {
203|          [HttpGet("available")]
204|          [Authorize(Roles = "Shofer")]
205|          public async Task<ActionResult<IEnumerable<DeliveryRouteDto>>> GetAvailableRoutes()
206|
207|          [HttpPost("{id}/assign")]
208|          [Authorize(Roles = "Shofer")]
209|          public async Task<ActionResult<DeliveryRouteDto>> AssignDriver(string id)
210|
211|          [HttpPut("{id}/sequence")]
212|          [Authorize(Roles = "Shofer")]
213|          public async Task<ActionResult<DeliveryRouteDto>> UpdateSequence(string id, UpdateRouteSequenceDto dto)
214|      }
215|      `
   216|
   217|5. Business Rules Implementation:
   218| a. Route Generation Rules:
   219| `csharp
220|      public class RouteGenerationService
221|      {
222|          public async Task GenerateRoutesForRegion(string region, DateTime deliveryDate)
223|          {
224|              var orders = await _orderRepository.GetByRegionAndDateAsync(region, deliveryDate);
225|              var orderCount = orders.Count();
226|
227|              if (orderCount <= 60)
228|              {
229|                  await CreateTwoRoutes(orders, region, deliveryDate);
230|              }
231|              else
232|              {
233|                  await CreateThreeRoutes(orders, region, deliveryDate);
234|              }
235|          }
236|      }
237|      `
   238|
   239|6. Testing:
   240| a. Unit Tests:
   241| - Order service tests
   242| - Route generation tests
   243| - Business rules tests
   244|
   245| b. Integration Tests:
   246| - Order creation flow
   247| - Route assignment
   248| - Status updates
   249|
   250|7. Manual Testing:
   251| a. Test Scenarios:
   252| - Create orders
   253| - Route generation
   254| - Driver assignment
   255| - Order sequence updates
   256| - Status updates
   257|```

PHASE 6: Reporting and Analytics System

1. Domain Layer Setup:
   a. Create Report Entities:

   ```csharp
   public class DailyReport
   {
       public string Id { get; set; }
       public DateTime Date { get; set; }
       public string ReportType { get; set; }
       public string GeneratedBy { get; set; }
       public DateTime GeneratedAt { get; set; }
       public string JsonData { get; set; }
   }

   public class UserPerformance
   {
       public string Id { get; set; }
       public string UserId { get; set; }
       public string UserRole { get; set; }
       public DateTime Date { get; set; }
       public int CompletedTasks { get; set; }
       public decimal SuccessRate { get; set; }
       public string Metrics { get; set; } // JSON string of role-specific metrics
   }

   public class InventorySnapshot
   {
       public string Id { get; set; }
       public DateTime Date { get; set; }
       public string ProductId { get; set; }
       public decimal TotalQuantity { get; set; }
       public decimal LabeledQuantity { get; set; }
       public decimal UnlabeledQuantity { get; set; }
   }
   ```

   b. Create Interfaces:

   ```csharp
   public interface IReportRepository
   {
       Task<DailyReport> GetReportAsync(DateTime date, string type);
       Task<IEnumerable<UserPerformance>> GetUserPerformanceAsync(
           string userId, DateTime startDate, DateTime endDate);
       Task<IEnumerable<InventorySnapshot>> GetInventorySnapshotsAsync(
           string productId, DateTime startDate, DateTime endDate);
       Task SaveReportAsync(DailyReport report);
   }
   ```

2. Infrastructure Layer:
   a. Create Stored Procedures:

   ```sql
   DELIMITER //

   CREATE PROCEDURE GenerateKomercialistReport(
       IN p_Date DATE
   )
   BEGIN
       SELECT
           k.Id as KomercialistId,
           COUNT(o.Id) as TotalOrders,
           COUNT(CASE WHEN o.Status = 'Delivered' THEN 1 END) as DeliveredOrders,
           COUNT(CASE WHEN o.Status = 'Cancelled' THEN 1 END) as CancelledOrders
       FROM Komercialists k
       LEFT JOIN Orders o ON k.Id = o.KomercialistId
       WHERE DATE(o.OrderDate) = p_Date
       GROUP BY k.Id;
   END //

   CREATE PROCEDURE GenerateEtiketuesReport(
       IN p_Date DATE
   )
   BEGIN
       SELECT
           e.Id as EtiketuesId,
           COUNT(lt.Id) as TotalTasks,
           SUM(lt.QuantityLabeled) as TotalLabeled
       FROM Etiketueses e
       LEFT JOIN LabelingTasks lt ON e.Id = lt.EtiketuesId
       WHERE DATE(lt.StartTime) = p_Date
       GROUP BY e.Id;
   END //

   CREATE PROCEDURE GenerateShoferReport(
       IN p_Date DATE
   )
   BEGIN
       SELECT
           s.Id as ShoferId,
           COUNT(dr.Id) as TotalRoutes,
           COUNT(o.Id) as TotalDeliveries,
           AVG(CASE WHEN o.Status = 'Delivered' THEN 1 ELSE 0 END) as SuccessRate
       FROM Shofers s
       LEFT JOIN DeliveryRoutes dr ON s.Id = dr.ShoferId
       LEFT JOIN Orders o ON dr.Id = o.DeliveryRouteId
       WHERE DATE(dr.DeliveryDate) = p_Date
       GROUP BY s.Id;
   END //

   DELIMITER ;
   ```

3. Application Layer:
   a. Create DTOs:

   ```csharp
   public class DailyReportDto
   {
       public DateTime Date { get; set; }
       public string ReportType { get; set; }
       public object Data { get; set; }
   }

   public class KomercialistReportDto
   {
       public string KomercialistId { get; set; }
       public string Name { get; set; }
       public int TotalOrders { get; set; }
       public int DeliveredOrders { get; set; }
       public int CancelledOrders { get; set; }
       public decimal SuccessRate { get; set; }
   }

   public class EtiketuesReportDto
   {
       public string EtiketuesId { get; set; }
       public string Name { get; set; }
       public int TotalTasks { get; set; }
       public decimal TotalLabeled { get; set; }
       public decimal AverageSpeed { get; set; }
   }
   ```

   b. Create Services:

   ```csharp
   public interface IReportingService
   {
       Task<DailyReportDto> GenerateDailyReportAsync(DateTime date, string type);
       Task<IEnumerable<UserPerformanceDto>> GetUserPerformanceAsync(
           string userId, DateTime startDate, DateTime endDate);
       Task<IEnumerable<InventorySnapshotDto>> GetInventoryReportAsync(
           string productId, DateTime startDate, DateTime endDate);
   }

   public class ReportingService : IReportingService
   {
       private readonly IReportRepository _reportRepository;
       private readonly IMapper _mapper;

       public async Task<DailyReportDto> GenerateDailyReportAsync(DateTime date, string type)
       {
           switch (type.ToLower())
           {
               case "komercialist":
                   return await GenerateKomercialistReportAsync(date);
               case "etiketues":
                   return await GenerateEtiketuesReportAsync(date);
               case "shofer":
                   return await GenerateShoferReportAsync(date);
               default:
                   throw new ArgumentException("Invalid report type");
           }
       }
   }
   ```

4. API Layer:
   a. Create Controllers:

   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize(Roles = "Admin,Menaxher")]
   public class ReportsController : ControllerBase
   {
       private readonly IReportingService _reportingService;

       [HttpGet("daily/{type}")]
       public async Task<ActionResult<DailyReportDto>> GetDailyReport(
           string type, [FromQuery] DateTime date)

       [HttpGet("performance/{userId}")]
       public async Task<ActionResult<IEnumerable<UserPerformanceDto>>> GetUserPerformance(
           string userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)

       [HttpGet("inventory/{productId}")]
       public async Task<ActionResult<IEnumerable<InventorySnapshotDto>>> GetInventoryReport(
           string productId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
   }
   ```

5. Scheduled Tasks:
   a. Create Background Services:

   ```csharp
   public class DailyReportGenerationService : BackgroundService
   {
       protected override async Task ExecuteAsync(CancellationToken stoppingToken)
       {
           while (!stoppingToken.IsCancellationRequested)
           {
               // Generate reports at midnight
               await GenerateDailyReportsAsync();
               await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
           }
       }
   }
   ```

6. Testing:
   a. Unit Tests:

   - Report generation logic
   - Data aggregation
   - Performance calculations

   b. Integration Tests:

   - Report API endpoints
   - Scheduled tasks
   - Data consistency

7. Manual Testing:
   a. Test Scenarios:
   - Generate different types of reports
   - Verify calculations
   - Check data aggregation
   - Test date ranges

PRE-PRODUCTION CHECKLIST:

1. Database Cleanup:

   - [ ] Remove HardDeleteUser stored procedure
   - [ ] Verify all test data has been cleaned up
   - [ ] Ensure only production-ready stored procedures remain

2. Security Review:

   - [ ] Review and rotate all secrets/keys
   - [ ] Ensure proper role-based access controls
   - [ ] Verify all endpoints are properly secured

3. Performance:

   - [ ] Review and optimize database indexes
   - [ ] Verify stored procedure performance
   - [ ] Run load tests

4. Documentation:
   - [ ] Update API documentation
   - [ ] Document all stored procedures
   - [ ] Create maintenance guides

```

```
