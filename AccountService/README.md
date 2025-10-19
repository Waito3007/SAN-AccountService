# Account Microservice - Architecture Design Document

## 📋 Tổng Quan

Account Microservice là dịch vụ đầu tiên trong hệ thống nền tảng thương mại điện tử SANGEAR, quản lý toàn bộ thông tin người dùng, xác thực, phân quyền và hồ sơ tài khoản.

## 🏗️ Kiến Trúc Tổng Thể

### Clean Architecture Pattern

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│              (API Controllers, Middlewares)              │
└───────────────────────┬─────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────┐
│                   Application Layer                      │
│        (Use Cases, DTOs, Interfaces, Services)          │
└───────────────────────┬─────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────┐
│                     Domain Layer                         │
│         (Entities, Value Objects, Enums, Events)        │
└───────────────────────┬─────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────┐
│                 Infrastructure Layer                     │
│    (Database, External Services, Caching, Messaging)    │
└─────────────────────────────────────────────────────────┘
```

## 🔑 Phân Quyền Theo Permission (PBAC) – Cập nhật trọng tâm

Thay vì phân cấp cứng theo Role, hệ thống sử dụng Permission làm hạt nhân. Role chỉ là tập hợp các Permission. JWT chứa danh sách permission codes để ủy quyền nhanh ở API Gateway và từng service.

- Khái niệm cốt lõi:
    - Permission (quyền): Đơn vị ủy quyền nhỏ nhất, có Code (số), Name, Resource, Action.
    - Role: Nhóm permission. User có thể có nhiều role; quyền thực tế = hợp các permission của tất cả role + quyền đặc thù gán trực tiếp (nếu có).
    - Policy: "PERM:{PermissionName}" hoặc "PERM:{Code}" để check tại runtime.

- Quy tắc JWT:
    - Claim "perms": danh sách số nguyên (VD: [1,2,3,14]).
    - Claim "roles": danh sách role codes/names (tùy chọn, phục vụ hiển thị/tra cứu).

- Mẫu áp dụng trong code (gợi ý):
    - Attribute: [HasPermission(PermissionCode.SoftDeleteUser)]
    - Policy: AddAuthorization(options => options.AddPolicy("PERM:SoftDeleteUser", ...))
    - Handler: lấy claim "perms" và so sánh với PermissionCode yêu cầu.

## 📁 Cấu Trúc Thư Mục Chi Tiết

```
AccountService/
│
├── src/
│   ├── AccountService.API/                    # Presentation Layer
│   │   ├── Controllers/
│   │   │   ├── AccountsController.cs
│   │   │   ├── AuthController.cs
│   │   │   ├── ProfilesController.cs
│   │   │   └── UsersController.cs
│   │   ├── Middlewares/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   ├── RequestLoggingMiddleware.cs
│   │   │   ├── RateLimitingMiddleware.cs
│   │   │   └── AuthenticationMiddleware.cs
│   │   ├── Filters/
│   │   │   ├── ValidationFilter.cs
│   │   │   └── AuthorizationFilter.cs
│   │   ├── Authorization/
│   │   │   ├── HasPermissionAttribute.cs            # Attribute check permission
│   │   │   └── PermissionPolicies.cs                # Đăng ký policy theo permission
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── ApplicationBuilderExtensions.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── AccountService.Application/            # Application Layer
│   │   ├── Common/
│   │   │   ├── Authorization/
│   │   │   │   ├── PermissionCode.cs                # Enum code các quyền
│   │   │   │   ├── PermissionNames.cs               # Hằng số tên quyền (Resource.Action)
│   │   │   │   ├── PermissionRequirement.cs         # IAuthorizationRequirement
│   │   │   │   └── PermissionAuthorizationHandler.cs# AuthorizationHandler
│   │   │   ├── Interfaces/
│   │   │   │   ├── IApplicationDbContext.cs
│   │   │   │   ├── IDateTime.cs
│   │   │   │   ├── ICurrentUserService.cs
│   │   │   │   └── IEmailService.cs
│   │   │   ├── Behaviours/
│   │   │   │   ├── ValidationBehaviour.cs
│   │   │   │   ├── LoggingBehaviour.cs
│   │   │   │   ├── PerformanceBehaviour.cs
│   │   │   │   └── UnhandledExceptionBehaviour.cs
│   │   │   ├── Mappings/
│   │   │   │   └── MappingProfile.cs
│   │   │   └── Models/
│   │   │       ├── PaginatedList.cs
│   │   │       ├── Result.cs
│   │   │       └── ServiceResponse.cs
│   │   ├── Features/
│   │   │   ├── Auth/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── Login/
│   │   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   │   ├── LoginCommandValidator.cs
│   │   │   │   │   │   └── LoginCommandHandler.cs
│   │   │   │   │   ├── Register/
│   │   │   │   │   │   ├── RegisterCommand.cs
│   │   │   │   │   │   ├── RegisterCommandValidator.cs
│   │   │   │   │   │   └── RegisterCommandHandler.cs
│   │   │   │   │   ├── RefreshToken/
│   │   │   │   │   ├── ResetPassword/
│   │   │   │   │   ├── ForgotPassword/
│   │   │   │   │   ├── VerifyEmail/
│   │   │   │   │   └── ChangePassword/
│   │   │   │   └── Queries/
│   │   │   │       └── ValidateToken/
│   │   │   ├── Users/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateUser/
│   │   │   │   │   ├── UpdateUser/
│   │   │   │   │   ├── DeleteUser/
│   │   │   │   │   ├── ActivateUser/
│   │   │   │   │   └── DeactivateUser/
│   │   │   │   └── Queries/
│   │   │   │       ├── GetUserById/
│   │   │   │       ├── GetUserByEmail/
│   │   │   │       ├── GetUsersWithPagination/
│   │   │   │       └── SearchUsers/
│   │   │   ├── Profiles/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── UpdateProfile/
│   │   │   │   │   ├── UpdateAvatar/
│   │   │   │   │   └── UpdatePreferences/
│   │   │   │   └── Queries/
│   │   │   │       ├── GetProfile/
│   │   │   │       └── GetProfileSettings/
│   │   │   └── Roles/
│   │   │       ├── Commands/
│   │   │       │   ├── AssignRole/
│   │   │       │   └── RemoveRole/
│   │   │       └── Queries/
│   │   │           ├── GetUserRoles/
│   │   │           └── GetRolePermissions/
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   ├── LoginResponseDto.cs
│   │   │   │   ├── RegisterRequestDto.cs
│   │   │   │   ├── TokenResponseDto.cs
│   │   │   │   └── RefreshTokenRequestDto.cs
│   │   │   ├── Users/
│   │   │   │   ├── UserDto.cs
│   │   │   │   ├── CreateUserDto.cs
│   │   │   │   ├── UpdateUserDto.cs
│   │   │   │   └── UserListDto.cs
│   │   │   └── Profiles/
│   │   │       ├── ProfileDto.cs
│   │   │       ├── UpdateProfileDto.cs
│   │   │       └── ProfileSettingsDto.cs
│   │   ├── Services/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── ITokenService.cs
│   │   │   └── IProfileService.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── AccountService.Domain/                 # Domain Layer
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── UserProfile.cs
│   │   │   ├── Role.cs
│   │   │   ├── Permission.cs
│   │   │   ├── RefreshToken.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── UserSession.cs
│   │   │   ├── PasswordHistory.cs
│   │   │   └── AuditLog.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── PhoneNumber.cs
│   │   │   ├── Address.cs
│   │   │   ├── FullName.cs
│   │   │   └── Money.cs
│   │   ├── Enums/
│   │   │   ├── UserStatus.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── Gender.cs
│   │   │   ├── AccountType.cs
│   │   │   ├── VerificationStatus.cs
│   │   │   ├── AuthProvider.cs
│   │   │   └── AuditAction.cs
│   │   ├── Events/
│   │   │   ├── UserCreatedEvent.cs
│   │   │   ├── UserUpdatedEvent.cs
│   │   │   ├── UserSoftDeletedEvent.cs
│   │   │   ├── UserRestoredEvent.cs
│   │   │   └── UserStatusChangedEvent.cs
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs
│   │   │   ├── InvalidEmailException.cs
│   │   │   ├── InvalidPhoneNumberException.cs
│   │   │   └── UserNotFoundException.cs
│   │   ├── Common/
│   │   │   ├── ISoftDelete.cs
│   │   │   └── AuditableEntity.cs
│   │   └── Specifications/
│   │       ├── UserSpecifications.cs
│   │       └── BaseSpecification.cs
│   │
│   └── AccountService.Infrastructure/         # Infrastructure Layer
│       ├── Persistence/
│       │   ├── Configurations/
│       │   │   ├── PermissionConfiguration.cs        # Seed permissions (Code/Name)
│       │   │   └── RolePermissionConfiguration.cs    # M:N Role-Permission
│       │   ├── Interceptors/
│       │   │   └── SoftDeleteInterceptor.cs          # Ghi DeletedBy, DeletedAt, DeletedReason
│       │   ├── Repositories/
│       │   │   └── SoftDeleteRepositoryExtensions.cs # Chuẩn hoá SoftDelete/Restore
│       │   └── ApplicationDbContext.cs
│       ├── Identity/
│       │   ├── IdentityService.cs
│       │   ├── TokenService.cs
│       │   ├── PasswordHasher.cs
│       │   └── JwtSettings.cs
│       ├── Services/
│       │   ├── DateTimeService.cs
│       │   ├── EmailService.cs
│       │   ├── SmsService.cs
│       │   └── CurrentUserService.cs
│       ├── Caching/
│       │   ├── RedisCacheService.cs
│       │   └── ICacheService.cs
│       ├── MessageBroker/
│       │   ├── RabbitMQ/
│       │   │   ├── RabbitMQService.cs
│       │   │   └── RabbitMQSettings.cs
│       │   └── IMessageBusService.cs
│       ├── ExternalServices/
│       │   ├── GoogleAuthService.cs
│       │   ├── FacebookAuthService.cs
│       │   └── CloudStorageService.cs
│       └── DependencyInjection.cs
│
├── tests/
│   ├── AccountService.UnitTests/
│   │   ├── Application/
│   │   ├── Domain/
│   │   └── Infrastructure/
│   ├── AccountService.IntegrationTests/
│   │   ├── Controllers/
│   │   └── Infrastructure/
│   └── AccountService.ArchitectureTests/
│
├── docs/
│   ├── API.md
│   ├── DEPLOYMENT.md
│   └── SECURITY.md
│
├── .dockerignore
├── .gitignore
├── Dockerfile
├── docker-compose.yml
└── AccountService.sln
```

## 🎯 Domain Layer - Chi Tiết

### Entities

#### User Entity
```csharp
public class User : AuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public Email Email { get; set; }
    public string PasswordHash { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public UserStatus Status { get; set; }
    public AccountType AccountType { get; set; }
    public AuthProvider AuthProvider { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LockedUntil { get; set; }
    public int FailedLoginAttempts { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
    public string? DeletedReason { get; set; }
    
    // Navigation Properties
    public UserProfile Profile { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public ICollection<UserSession> Sessions { get; set; }
    public ICollection<PasswordHistory> PasswordHistory { get; set; }
}
```

### Enums

#### UserStatus
```csharp
public enum UserStatus
{
    Pending = 0,           // Chờ xác thực email
    Active = 1,            // Đang hoạt động
    Inactive = 2,          // Không hoạt động
    Suspended = 3,         // Tạm khóa
    Locked = 4,            // Bị khóa
    Banned = 5             // Bị cấm
}
```

#### AccountType
```csharp
public enum AccountType
{
    Customer = 0,          // Khách hàng
    Seller = 1,            // Người bán
    Admin = 2,             // Quản trị viên
    SuperAdmin = 3,        // Quản trị viên cấp cao
    Employee = 4           // Nhân viên
}
```

#### Gender
```csharp
public enum Gender
{
    NotSpecified = 0,
    Male = 1,
    Female = 2,
    Other = 3
}
```

#### AuthProvider
```csharp
public enum AuthProvider
{
    Local = 0,             // Đăng ký thông thường
    Google = 1,
    Facebook = 2,
    Apple = 3,
    Microsoft = 4
}
```

#### VerificationStatus
```csharp
public enum VerificationStatus
{
    NotVerified = 0,
    Pending = 1,
    Verified = 2,
    Rejected = 3,
    Expired = 4
}
```

#### AuditAction
```csharp
public enum AuditAction
{
    Create = 0,
    Read = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    PasswordChange = 6,
    PasswordReset = 7,
    EmailVerification = 8,
    PhoneVerification = 9,
    TwoFactorEnabled = 10,
    TwoFactorDisabled = 11,
    AccountLocked = 12,
    AccountUnlocked = 13
}
```

#### PermissionCode (quyền lõi – ví dụ)
```csharp
public enum PermissionCode
{
    // User
    SoftDeleteUser = 1,
    CreateUser = 2,
    UpdateUser = 3,
    ReadUser = 4,
    RestoreUser = 5,
    ActivateUser = 6,
    DeactivateUser = 7,
    LockUser = 8,
    UnlockUser = 9,

    // Role
    ReadRole = 20,
    CreateRole = 21,
    UpdateRole = 22,
    DeleteRole = 23,
    AssignRoleToUser = 24,

    // Permission
    ReadPermission = 40,
    AssignPermissionsToRole = 41,

    // Audit
    ReadAuditLog = 60
}
```

> Gợi ý đặt tên PermissionName = "{Resource}.{Action}", ví dụ: "Users.SoftDelete" tương ứng Code = 1.

### Domain Events (cập nhật)

- UserCreatedEvent
- UserUpdatedEvent
- UserSoftDeletedEvent   // thay thế UserDeletedEvent
- UserRestoredEvent
- UserStatusChangedEvent
- UserLoggedInEvent
- PasswordChangedEvent
- EmailVerifiedEvent

## 🎨 Design Patterns Được Sử Dụng

### 1. **CQRS (Command Query Responsibility Segregation)**
Tách biệt operations đọc và ghi:
- **Commands**: Thay đổi state (Create, Update, Delete)
- **Queries**: Chỉ đọc dữ liệu

### 2. **Mediator Pattern**
Sử dụng MediatR để:
- Giảm coupling giữa các components
- Centralize request handling
- Dễ dàng implement behaviors (validation, logging, caching)

### 3. **Repository Pattern**
Abstraction layer cho data access:
- Tách biệt business logic và data access
- Dễ dàng testing và mock
- Có thể swap database implementation

### 4. **Unit of Work Pattern**
Quản lý transactions:
- Đảm bảo data consistency
- Group multiple operations

### 5. **Factory Pattern**
Tạo complex objects:
- UserFactory
- TokenFactory
- EmailFactory

### 6. **Strategy Pattern**
Cho authentication providers:
- LocalAuthStrategy
- GoogleAuthStrategy
- FacebookAuthStrategy

### 7. **Specification Pattern**
Cho complex queries:
- ActiveUsersSpecification
- UsersByRoleSpecification
- ExpiredTokensSpecification

### 8. **Observer Pattern (Domain Events)**
Publish domain events:
- UserCreatedEvent → Send welcome email
- PasswordChangedEvent → Notify user
- UserLoggedInEvent → Log activity

### 9. **Decorator Pattern**
Cho behaviors pipeline:
- Validation
- Logging
- Performance monitoring
- Exception handling

### 10. **Builder Pattern**
Xây dựng complex DTOs và responses

## 🔐 Security & Authorization Patterns (PBAC)

### Policy-based Authorization theo Permission
- Mỗi API action gắn attribute [HasPermission(PermissionCode.X)] hoặc [Authorize(Policy = "PERM:Users.SoftDelete")]
- AuthorizationHandler đọc claim "perms" (int[]) từ JWT và so sánh.
- Hỗ trợ cache permission theo user (Redis) để giảm hit DB; invalidate khi thay đổi role/permission.

Ví dụ code rút gọn:
```csharp
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(PermissionCode code)
    {
        Policy = $"PERM:{code}"; // đăng ký policy tương ứng khi startup
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var perms = context.User.FindFirst("perms")?.Value; // ví dụ: "1,2,3"
        if (!string.IsNullOrEmpty(perms))
        {
            var set = new HashSet<int>(perms.Split(',').Select(int.Parse));
            if (set.Contains((int)requirement.Code))
            {
                context.Succeed(requirement);
            }
        }
        return Task.CompletedTask;
    }
}
```

### Token chứa Permission
- Access Token chứa claim perms: "1,2,3,8".
- Refresh Token không cần perms, chỉ dùng để phát mới Access Token.

### Auditing & Soft Delete
- Mọi hành động nhạy cảm (soft delete, restore, lock/unlock) ghi AuditLog: ActorId, TargetId, Action, Reason, Timestamp, CorrelationId.
- Soft delete không xóa vật lý dữ liệu, chỉ đặt cờ IsDeleted, DeletedAt, DeletedByUserId, DeletedReason.
- Truy vấn mặc định filter IsDeleted = false; có thể IncludeDeleted khi cần cho admin.

## 📊 Database Schema (cập nhật)

### Core Tables

```sql
-- Users (cập nhật)
Users
├── Id (PK, UNIQUEIDENTIFIER)
├── Username (NVARCHAR(50), UNIQUE)
├── Email (NVARCHAR(255), UNIQUE)
├── PasswordHash (NVARCHAR(MAX))
├── PhoneNumber (NVARCHAR(20))
├── Status (INT)
├── AccountType (INT)
├── AuthProvider (INT)
├── EmailVerified (BIT)
├── PhoneVerified (BIT)
├── TwoFactorEnabled (BIT)
├── LastLoginAt (DATETIME2)
├── LockedUntil (DATETIME2)
├── FailedLoginAttempts (INT)
├── CreatedAt (DATETIME2)
├── CreatedBy (NVARCHAR(100))
├── LastModifiedAt (DATETIME2)
├── LastModifiedBy (NVARCHAR(100))
├── IsDeleted (BIT)
├── DeletedAt (DATETIME2)
├── DeletedByUserId (UNIQUEIDENTIFIER, NULL)
└── DeletedReason (NVARCHAR(500), NULL)

-- Roles
Roles
├── Id (PK, UNIQUEIDENTIFIER)
├── Code (NVARCHAR(50), UNIQUE)
├── Name (NVARCHAR(50), UNIQUE)
├── Description (NVARCHAR(255))
├── CreatedAt (DATETIME2)
└── LastModifiedAt (DATETIME2)

-- Permissions (cập nhật)
Permissions
├── Id (PK, UNIQUEIDENTIFIER)
├── Code (INT, UNIQUE)                -- ví dụ: 1 = Users.SoftDelete
├── Name (NVARCHAR(100), UNIQUE)      -- ví dụ: Users.SoftDelete
├── Description (NVARCHAR(255))
├── Resource (NVARCHAR(50))
├── Action (NVARCHAR(50))
└── IsActive (BIT)

-- RolePermissions (M:N)
RolePermissions
├── RoleId (FK)
├── PermissionId (FK)
└── PRIMARY KEY (RoleId, PermissionId)

-- UserRoles (M:N)
UserRoles
├── UserId (FK)
├── RoleId (FK)
└── PRIMARY KEY (UserId, RoleId)

-- AuditLogs (mở rộng lý do)
AuditLogs
├── Id (PK)
├── ActorUserId (FK Users)
├── TargetUserId (FK Users, NULL)
├── Action (INT)                      -- AuditAction
├── Reason (NVARCHAR(500), NULL)
├── Metadata (NVARCHAR(MAX), NULL)    -- JSON: {ip, userAgent, ...}
├── CreatedAt (DATETIME2)
└── CorrelationId (UNIQUEIDENTIFIER)
```

## 🔄 API Endpoints Design (cập nhật)

### Authentication Endpoints
```
POST   /api/v1/auth/register            # Đăng ký tài khoản mới
POST   /api/v1/auth/login               # Đăng nhập
POST   /api/v1/auth/logout              # Đăng xuất
POST   /api/v1/auth/refresh-token       # Làm mới token
POST   /api/v1/auth/forgot-password     # Quên mật khẩu
POST   /api/v1/auth/reset-password      # Reset mật khẩu
POST   /api/v1/auth/verify-email        # Xác thực email
POST   /api/v1/auth/resend-verification # Gửi lại email xác thực
POST   /api/v1/auth/change-password     # Đổi mật khẩu
POST   /api/v1/auth/google              # Đăng nhập Google
POST   /api/v1/auth/facebook            # Đăng nhập Facebook
```

### User Management Endpoints
```
GET    /api/v1/users                                # Lấy danh sách users (Admin)
GET    /api/v1/users/{id}                           # Lấy thông tin user theo ID
POST   /api/v1/users                                # Tạo user mới (Admin)    [PERM:CreateUser]
PUT    /api/v1/users/{id}                           # Cập nhật user           [PERM:UpdateUser]
POST   /api/v1/users/{id}/soft-delete               # Đánh dấu xoá (soft)     [PERM:SoftDeleteUser]
  Body: { reason: string }
POST   /api/v1/users/{id}/restore                   # Khôi phục user          [PERM:RestoreUser]
PATCH  /api/v1/users/{id}/activate                  # Kích hoạt user          [PERM:ActivateUser]
PATCH  /api/v1/users/{id}/deactivate                # Vô hiệu hoá user        [PERM:DeactivateUser]
PATCH  /api/v1/users/{id}/lock                      # Khoá user               [PERM:LockUser]
PATCH  /api/v1/users/{id}/unlock                    # Mở khoá user            [PERM:UnlockUser]
GET    /api/v1/users/search                         # Tìm kiếm users          [PERM:ReadUser]
GET    /api/v1/users/{id}/audit                     # Audit trail             [PERM:ReadAuditLog]
```

> Ghi chú: Không dùng HTTP DELETE để xoá người dùng. Nếu cần tương thích, map DELETE -> SoftDelete nội bộ (và trả về 202 Accepted).

### Role & Permission Endpoints (cập nhật)
```
GET    /api/v1/roles                                # Danh sách roles         [PERM:ReadRole]
POST   /api/v1/roles                                # Tạo role               [PERM:CreateRole]
PUT    /api/v1/roles/{id}                           # Cập nhật role           [PERM:UpdateRole]
DELETE /api/v1/roles/{id}                           # Xoá role                [PERM:DeleteRole]
POST   /api/v1/users/{id}/roles                     # Gán role cho user       [PERM:AssignRoleToUser]
DELETE /api/v1/users/{id}/roles/{roleId}            # Gỡ role khỏi user       [PERM:AssignRoleToUser]
GET    /api/v1/permissions                          # Danh mục permissions    [PERM:ReadPermission]
GET    /api/v1/roles/{id}/permissions               # Quyền của 1 role        [PERM:ReadPermission]
POST   /api/v1/roles/{id}/permissions               # Gán quyền cho role      [PERM:AssignPermissionsToRole]
DELETE /api/v1/roles/{id}/permissions               # Gỡ quyền khỏi role      [PERM:AssignPermissionsToRole]
```

## 📨 Message Queue Events (cập nhật)

```yaml
UserRegistered:
  Target: EmailService
  Action: Send welcome email
  Data: UserId, Email, Username

UserEmailVerified:
  Target: NotificationService
  Action: Send confirmation notification
  Data: UserId, Email

UserCreated:
  Target: [OrderService, CartService, WishlistService]
  Action: Initialize user data
  Data: UserId, Email, AccountType

UserUpdated:
  Target: [All Services]
  Action: Sync user information
  Data: UserId, UpdatedFields

UserSoftDeleted:
  Target: [All Services]
  Action: Mark user as inactive/hidden across services
  Data: UserId, DeletedAt, DeletedByUserId, DeletedReason

UserRestored:
  Target: [All Services]
  Action: Reactivate user across services
  Data: UserId, RestoredAt, RestoredByUserId

PasswordChanged:
  Target: EmailService
  Action: Send security notification
  Data: UserId, Email, Timestamp, IpAddress
```

## 🛡️ Error Handling Strategy

### Error Response Format
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "field": "Email",
      "message": "Email is already registered"
    }
  ],
  "timestamp": "2025-10-19T10:30:00Z",
  "traceId": "uuid-here"
}
```

### HTTP Status Codes
```
200 OK                  - Success
201 Created             - Resource created
204 No Content          - Success with no response body
400 Bad Request         - Validation error
401 Unauthorized        - Authentication failed
403 Forbidden           - No permission
404 Not Found           - Resource not found
409 Conflict            - Duplicate resource
422 Unprocessable       - Business logic error
429 Too Many Requests   - Rate limit exceeded
500 Internal Server     - Server error
503 Service Unavailable - Service down
```

## 🔧 Configuration & Settings (bổ sung)

### appsettings.json – Authorization
```json
{
  "Authorization": {
    "EmitPermissionsInJwt": true,
    "PermissionClaimName": "perms",
    "CacheUserPermissionMinutes": 5
  }
}
```

## 📊 Monitoring & Logging

### Logging Levels
```yaml
Structured Logging with Serilog:
  - Information: Normal operations
  - Warning: Unusual but handled situations
  - Error: Errors requiring attention
  - Critical: System-wide failures

Log Sinks:
  - Console (Development)
  - File (Rolling daily)
  - Elasticsearch (Production)
  - Application Insights (Cloud)
```

### Metrics to Track
```yaml
Business Metrics:
  - Daily Active Users (DAU)
  - New Registrations
  - Login Success Rate
  - Email Verification Rate

Technical Metrics:
  - API Response Time
  - Error Rate
  - Database Query Performance
  - Cache Hit Rate
  - Message Queue Depth
```

## ♻️ Soft Delete – Quy tắc nghiệp vụ chi tiết

- Soft delete thay đổi trạng thái user như sau:
    - IsDeleted = true, Status = Inactive hoặc Suspended (tùy chính sách), DeletedAt = now, DeletedByUserId = actor, DeletedReason = input.
    - Revoke toàn bộ RefreshToken còn hiệu lực, kết thúc Sessions đang mở.
    - Ẩn khỏi kết quả tìm kiếm mặc định. Admin có thể IncludeDeleted để truy xuất.
- Khôi phục:
    - IsDeleted = false, Status = Active (nếu đủ điều kiện), ghi Audit "Restore".
    - Không khôi phục token/session cũ.
- Truy vấn EF Core:
    - Global Query Filter: .HasQueryFilter(x => !x.IsDeleted) cho User và các aggregate khác nếu cần.
- Interceptor:
    - Chặn .Remove() trên User và chuyển thành cập nhật soft delete.

## 🔐 Seed & Mapping Quyền Mặc Định

- Permissions seed (ví dụ – rút gọn):
    - 1 Users.SoftDelete, 2 Users.Create, 3 Users.Update, 4 Users.Read, 5 Users.Restore
    - 20 Roles.Read, 21 Roles.Create, 22 Roles.Update, 23 Roles.Delete, 24 Roles.AssignToUser
    - 40 Permissions.Read, 41 Permissions.AssignToRole, 60 Audit.Read
- Roles mặc định:
    - SuperAdmin: tất cả permissions
    - Admin: nhóm Users.*, Roles.Read/AssignToUser, Permissions.Read
    - Support: Users.Read, Users.Update
    - Customer: Users.Read (self-scope)
- Lưu ý phạm vi (scope):
    - Có thể mở rộng Permission theo scope: Global, Own, Organization. Ví dụ: Users.Read.Own.

## 🧪 Testing Notes

- Unit tests cho PermissionAuthorizationHandler với các case:
    - Có/không có claim perms
    - perms thiếu quyền yêu cầu
    - perms có quyền yêu cầu
- Integration tests cho các endpoint soft-delete/restore và audit.
- Migration tests: đảm bảo query filter không lọc nhầm khi IncludeDeleted.

## 🚀 Deployment Strategy

### Docker Configuration
```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
```

### Environment-Specific Configs
```yaml
Development:
  - Local SQL Server
  - Local Redis
  - Mock Email Service
  
Staging:
  - Azure SQL Database
  - Azure Redis Cache
  - SendGrid Email
  
Production:
  - Azure SQL Database (Replica)
  - Azure Redis Cache (Cluster)
  - SendGrid Email
  - Application Insights
  - Azure Key Vault for secrets
```

## 📝 Best Practices Checklist

- Controllers không dùng DELETE cho User; dùng POST /soft-delete.
- Ghi đầy đủ AuditLog cho mọi hành động thay đổi trạng thái.
- JWT chỉ chứa perms cần thiết, không vượt quá giới hạn kích thước header.
- Khi cập nhật role/permission, revoke cache permission của user liên quan.

## 🎯 Next Steps (cập nhật)

1. Phase 1: Foundation
    - Seed bảng Permissions + RolePermissions.
    - Triển khai PermissionAuthorizationHandler + Attribute.
    - Thêm Global Query Filter cho soft delete.
2. Phase 2: Core Features
    - Implement SoftDeleteUserCommand, RestoreUserCommand, GetUserAuditTrailQuery.
    - Bổ sung endpoints mapping Role-Permission.
3. Phase 3: Advanced
    - Scope-based permissions (Own/Global), cache perms per user, policy naming convention.
    - SSO/OAuth: ánh xạ nhóm ngoài sang role nội bộ, hợp nhất permissions.
4. Phase 4: Testing & Docs
    - Test ma trận permission cho các endpoint.
    - Tài liệu hoá danh mục Permission và role mặc định trong docs/SECURITY.md.

---

**Document Version:** 1.1  
**Last Updated:** 2025-10-19  
**Author:** SANGEAR Development Team  
**Status:** ✅ Ready for Implementation (PBAC + Soft Delete)
