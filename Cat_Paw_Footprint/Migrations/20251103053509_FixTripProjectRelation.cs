using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cat_Paw_Footprint.Migrations
{
    /// <inheritdoc />
    public partial class FixTripProjectRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CouponDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric(7,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DiscountCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CouponName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coupons__384AF1DA962AEEC5", x => x.CouponID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLevels",
                columns: table => new
                {
                    Level = table.Column<int>(type: "int", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__AAF8996343B0DC7B", x => x.Level);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTripProjects",
                columns: table => new
                {
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalDays = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__761ABED0C9A3562A", x => x.ProjectID);
                });

            migrationBuilder.CreateTable(
                name: "DateDimension",
                columns: table => new
                {
                    DateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Day = table.Column<int>(type: "int", nullable: true),
                    Week = table.Column<int>(type: "int", nullable: true),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    DayName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsWeekend = table.Column<bool>(type: "bit", nullable: true),
                    IsHoliday = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DateDime__A426F253D3C63C50", x => x.DateID);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistrictID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__District__85FDA4A63BF24E8A", x => x.DistrictID);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRoles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__8AFACE3AA240E365", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "FAQCategorys",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FAQCateg__19093A2BAD7796F5", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    KeywordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keyword = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Keywords__37C135C138B67486", x => x.KeywordID);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    OrderStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusDesc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderSta__BC674F410981CD78", x => x.OrderStatusID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatus",
                columns: table => new
                {
                    PayMentStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusDesc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentS__F177127F257E414E", x => x.PayMentStatusID);
                });

            migrationBuilder.CreateTable(
                name: "PendingPayments",
                columns: table => new
                {
                    PendingPaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmount = table.Column<int>(type: "int", nullable: false),
                    CouponJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingPayments", x => x.PendingPaymentId);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    PromoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PromoDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: true),
                    DiscountValue = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Promotio__33D334D0D8C5EB2C", x => x.PromoID);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    RegionID = table.Column<int>(type: "int", nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.RegionID);
                });

            migrationBuilder.CreateTable(
                name: "TicketPriority",
                columns: table => new
                {
                    PriorityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriorityDesc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TicketPr__D0A3D0DE6273C0C0", x => x.PriorityID);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatus",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusDesc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TicketSt__C8EE2043F7460F01", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                columns: table => new
                {
                    TicketTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketTypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TicketTy__6CD6845185DEFCB2", x => x.TicketTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Transportations",
                columns: table => new
                {
                    TransportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TransportDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransportPrice = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    TransportCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transpor__19E9A17DE87E642A", x => x.TransportID);
                });

            migrationBuilder.CreateTable(
                name: "CouponPics",
                columns: table => new
                {
                    CouponPicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponID = table.Column<int>(type: "int", nullable: false),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponPics", x => x.CouponPicID);
                    table.ForeignKey(
                        name: "FK__CouponPic__Coupo__151B244E",
                        column: x => x.CouponID,
                        principalTable: "Coupons",
                        principalColumn: "CouponID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    EmployeeCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__7AD04FF1804EF882", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK__Employees__RoleI__29221CFB",
                        column: x => x.RoleID,
                        principalTable: "EmployeeRoles",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    FAQID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsHot = table.Column<bool>(type: "bit", nullable: false),
                    HotOrder = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FAQs__4B89D1E24C3A6A93", x => x.FAQID);
                    table.ForeignKey(
                        name: "FK__FAQs__CategoryID__2A164134",
                        column: x => x.CategoryID,
                        principalTable: "FAQCategorys",
                        principalColumn: "CategoryID");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LevelUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    IsBlacklisted = table.Column<bool>(type: "bit", nullable: true),
                    CustomerCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__A4AE64B82A50E373", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK_Customers_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Customers__Level__208CD6FA",
                        column: x => x.Level,
                        principalTable: "CustomerLevels",
                        principalColumn: "Level",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    HotelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HotelAddr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HotelLat = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    HotelLng = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    HotelDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    DistrictID = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    HotelCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Hotels__46023BBF1005A475", x => x.HotelID);
                    table.ForeignKey(
                        name: "FK_Hotels_Regions",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID");
                    table.ForeignKey(
                        name: "FK__Hotels__District__2DE6D218",
                        column: x => x.DistrictID,
                        principalTable: "Districts",
                        principalColumn: "DistrictID");
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocationAddr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LocationLat = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    LocationLng = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    LocationDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationPrice = table.Column<int>(type: "int", nullable: true),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    DistrictID = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    LocationCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Location__E7FEA4777BCBA51B", x => x.LocationID);
                    table.ForeignKey(
                        name: "FK_Locations_Regions",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID");
                    table.ForeignKey(
                        name: "FK__Locations__Distr__32AB8735",
                        column: x => x.DistrictID,
                        principalTable: "Districts",
                        principalColumn: "DistrictID");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    ProductDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPrice = table.Column<int>(type: "int", nullable: true),
                    ProductNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxPeople = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    ProductImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timetable = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__B40CC6EDDACA8D2D", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Products_Regions",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID");
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    RestaurantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RestaurantCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RestaurantAddr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RestaurantLat = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    RestaurantLng = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    RestaurantDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    DistrictID = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Restaura__87454CB50407FEEF", x => x.RestaurantID);
                    table.ForeignKey(
                        name: "FK_Restaurants_Regions",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID");
                    table.ForeignKey(
                        name: "FK__Restauran__Distr__498EEC8D",
                        column: x => x.DistrictID,
                        principalTable: "Districts",
                        principalColumn: "DistrictID");
                });

            migrationBuilder.CreateTable(
                name: "SemiSelfProducts",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    ProductDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPrice = table.Column<int>(type: "int", nullable: true),
                    ProductType = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Views = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxPeople = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SemiSelf__B40CC6ED1BE67600", x => x.ProductID);
                    table.ForeignKey(
                        name: "SemiSelfProduct_Region_FK",
                        column: x => x.RegionID,
                        principalTable: "Regions",
                        principalColumn: "RegionID");
                });

            migrationBuilder.CreateTable(
                name: "TransportKeywords",
                columns: table => new
                {
                    TransportKeywordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportID = table.Column<int>(type: "int", nullable: true),
                    KeywordID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportKeywords", x => x.TransportKeywordID);
                    table.ForeignKey(
                        name: "FK__Transport__Keywo__531856C7",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                    table.ForeignKey(
                        name: "FK__Transport__Trans__540C7B00",
                        column: x => x.TransportID,
                        principalTable: "Transportations",
                        principalColumn: "TransportID");
                });

            migrationBuilder.CreateTable(
                name: "TransportPics",
                columns: table => new
                {
                    TransportPicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportID = table.Column<int>(type: "int", nullable: true),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportPics", x => x.TransportPicID);
                    table.ForeignKey(
                        name: "FK__Transport__Trans__55009F39",
                        column: x => x.TransportID,
                        principalTable: "Transportations",
                        principalColumn: "TransportID");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfile",
                schema: "dbo",
                columns: table => new
                {
                    EmployeeProfileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IDNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    EmployeeProfileCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__7CC01C3EA0B9E04D", x => x.EmployeeProfileID);
                    table.UniqueConstraint("AK_EmployeeProfile_EmployeeID", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK__EmployeeP__Emplo__282DF8C2",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCouponsRecords",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    CouponID = table.Column<int>(type: "int", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: true),
                    UsedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__CustomerC__Coupo__17036CC0",
                        column: x => x.CouponID,
                        principalTable: "Coupons",
                        principalColumn: "CouponID");
                    table.ForeignKey(
                        name: "FK__CustomerC__Custo__17F790F9",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerLoginHistory",
                columns: table => new
                {
                    LoginLogID = table.Column<int>(type: "int", nullable: false),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    LoginIP = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__D42E7ACC91490DF1", x => x.LoginLogID);
                    table.ForeignKey(
                        name: "FK__CustomerL__Custo__18EBB532",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerProfile",
                columns: table => new
                {
                    CustomerProfilesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IDNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerProfileCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__13B52926385BD265", x => x.CustomerProfilesID);
                    table.UniqueConstraint("AK_CustomerProfile_CustomerID", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK__CustomerP__Custo__1F98B2C1",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E32871B0789", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__ReadA__5C6CB6D7",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelKeywords",
                columns: table => new
                {
                    HotelKeywordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelID = table.Column<int>(type: "int", nullable: true),
                    KeywordID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelKeywords", x => x.HotelKeywordID);
                    table.ForeignKey(
                        name: "FK__HotelKeyw__Hotel__2B0A656D",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID");
                    table.ForeignKey(
                        name: "FK__HotelKeyw__Keywo__2BFE89A6",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                });

            migrationBuilder.CreateTable(
                name: "HotelPics",
                columns: table => new
                {
                    HotelPicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelID = table.Column<int>(type: "int", nullable: true),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelPics", x => x.HotelPicID);
                    table.ForeignKey(
                        name: "FK__HotelPics__Hotel__2CF2ADDF",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID");
                });

            migrationBuilder.CreateTable(
                name: "LocationKeywords",
                columns: table => new
                {
                    LocationKeywordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationID = table.Column<int>(type: "int", nullable: true),
                    KeywordID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationKeywords", x => x.LocationKeywordID);
                    table.ForeignKey(
                        name: "FK__LocationK__Keywo__2FCF1A8A",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                    table.ForeignKey(
                        name: "FK__LocationK__Locat__30C33EC3",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID");
                });

            migrationBuilder.CreateTable(
                name: "LocationPics",
                columns: table => new
                {
                    LocationPicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationID = table.Column<int>(type: "int", nullable: true),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationPics", x => x.LocationPicID);
                    table.ForeignKey(
                        name: "FK__LocationP__Locat__31B762FC",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID");
                });

            migrationBuilder.CreateTable(
                name: "ProductAnalysis",
                columns: table => new
                {
                    ProductAnalysisID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovalDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAnalysis", x => x.ProductAnalysisID);
                    table.ForeignKey(
                        name: "FK__ProductAn__Produ__3864608B",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "ProductPics",
                columns: table => new
                {
                    ProductPicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPics", x => x.ProductPicID);
                    table.ForeignKey(
                        name: "FK__ProductPi__Produ__395884C4",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Products_Hotels",
                columns: table => new
                {
                    ProductHotelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    HotelID = table.Column<int>(type: "int", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_Hotels", x => x.ProductHotelID);
                    table.ForeignKey(
                        name: "FK__Products___Hotel__3B40CD36",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID");
                    table.ForeignKey(
                        name: "FK__Products___Produ__3C34F16F",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Products_Keywords",
                columns: table => new
                {
                    ProductKeywordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    KeywordID = table.Column<int>(type: "int", nullable: true),
                    KeywordsKeywordID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_Keywords", x => x.ProductKeywordID);
                    table.ForeignKey(
                        name: "FK_Products_Keywords_Keywords_KeywordsKeywordID",
                        column: x => x.KeywordsKeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                    table.ForeignKey(
                        name: "ProKey_Keyword_FK",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                    table.ForeignKey(
                        name: "ProKey_Product_FK",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Products_Locations",
                columns: table => new
                {
                    ProductLocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: true),
                    DayNumber = table.Column<int>(type: "int", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_Locations", x => x.ProductLocationID);
                    table.ForeignKey(
                        name: "FK__Products___Locat__3D2915A8",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID");
                    table.ForeignKey(
                        name: "FK__Products___Produ__3E1D39E1",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Products_Promotions",
                columns: table => new
                {
                    ProductPromoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_Promotions", x => x.ProductPromoID);
                    table.ForeignKey(
                        name: "FK_ProductsPromotions_Promotion",
                        column: x => x.PromoID,
                        principalTable: "Promotions",
                        principalColumn: "PromoID");
                    table.ForeignKey(
                        name: "FK__Products___Produ__3F115E1A",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products_Transportations",
                columns: table => new
                {
                    ProductTransID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    TransportID = table.Column<int>(type: "int", nullable: true),
                    TransportInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransportTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_Transportations", x => x.ProductTransID);
                    table.ForeignKey(
                        name: "FK__Products___Produ__44CA3770",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK__Products___Trans__45BE5BA9",
                        column: x => x.TransportID,
                        principalTable: "Transportations",
                        principalColumn: "TransportID");
                });

            migrationBuilder.CreateTable(
                name: "Products_Restaurants",
                columns: table => new
                {
                    ProductRestaurantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    RestaurantID = table.Column<int>(type: "int", nullable: true),
                    MealType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_Restaurants", x => x.ProductRestaurantID);
                    table.ForeignKey(
                        name: "FK__Products___Produ__40F9A68C",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK__Products___Resta__42E1EEFE",
                        column: x => x.RestaurantID,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantID");
                });

            migrationBuilder.CreateTable(
                name: "RestaurantKeywords",
                columns: table => new
                {
                    RestaurantKeywordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantID = table.Column<int>(type: "int", nullable: true),
                    KeywordID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantKeywords", x => x.RestaurantKeywordID);
                    table.ForeignKey(
                        name: "FK__Restauran__Keywo__46B27FE2",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                    table.ForeignKey(
                        name: "FK__Restauran__Resta__47A6A41B",
                        column: x => x.RestaurantID,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantID");
                });

            migrationBuilder.CreateTable(
                name: "RestaurantPics",
                columns: table => new
                {
                    RestaurantPicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantID = table.Column<int>(type: "int", nullable: true),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantPics", x => x.RestaurantPicID);
                    table.ForeignKey(
                        name: "FK__Restauran__Resta__489AC854",
                        column: x => x.RestaurantID,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantID");
                });

            migrationBuilder.CreateTable(
                name: "TripProjectDetails",
                columns: table => new
                {
                    ProjectDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: true),
                    TripDate = table.Column<int>(type: "int", nullable: true),
                    TripSequence = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    StayMinute = table.Column<int>(type: "int", nullable: true),
                    TransportID = table.Column<int>(type: "int", nullable: true),
                    TripType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HotelID = table.Column<int>(type: "int", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: true),
                    RestaurantID = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripProjectDetails", x => x.ProjectDetailID);
                    table.ForeignKey(
                        name: "FK__TripProje__Hotel__55F4C372",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID");
                    table.ForeignKey(
                        name: "FK__TripProje__Locat__56E8E7AB",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID");
                    table.ForeignKey(
                        name: "FK__TripProje__Proje__57DD0BE4",
                        column: x => x.ProjectID,
                        principalTable: "CustomerTripProjects",
                        principalColumn: "ProjectID");
                    table.ForeignKey(
                        name: "FK__TripProje__Resta__58D1301D",
                        column: x => x.RestaurantID,
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantID");
                    table.ForeignKey(
                        name: "FK__TripProje__Trans__59C55456",
                        column: x => x.TransportID,
                        principalTable: "Transportations",
                        principalColumn: "TransportID");
                });

            migrationBuilder.CreateTable(
                name: "Semi_Hotels",
                columns: table => new
                {
                    SemiHotelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    HotelID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semi_Hotels", x => x.SemiHotelID);
                    table.ForeignKey(
                        name: "FK__Semi_Hote__Hotel__4B7734FF",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID");
                    table.ForeignKey(
                        name: "FK__Semi_Hote__Produ__4C6B5938",
                        column: x => x.ProductID,
                        principalTable: "SemiSelfProducts",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Semi_Keywords",
                columns: table => new
                {
                    ProductKeywordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    KeywordID = table.Column<int>(type: "int", nullable: true),
                    KeywordsKeywordID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semi_Keywords", x => x.ProductKeywordID);
                    table.ForeignKey(
                        name: "FK_Semi_Keywords_Keywords_KeywordsKeywordID",
                        column: x => x.KeywordsKeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                    table.ForeignKey(
                        name: "SemiKey_Keyword_FK",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID");
                    table.ForeignKey(
                        name: "SemiKey_Product_FK",
                        column: x => x.ProductID,
                        principalTable: "SemiSelfProducts",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Semi_Locations",
                columns: table => new
                {
                    SemiLocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semi_Locations", x => x.SemiLocationID);
                    table.ForeignKey(
                        name: "FK__Semi_Loca__Locat__4D5F7D71",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID");
                    table.ForeignKey(
                        name: "FK__Semi_Loca__Produ__4E53A1AA",
                        column: x => x.ProductID,
                        principalTable: "SemiSelfProducts",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "Semi_Transportations",
                columns: table => new
                {
                    SemiTransID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    TransportID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semi_Transportations", x => x.SemiTransID);
                    table.ForeignKey(
                        name: "FK__Semi_Tran__Produ__4F47C5E3",
                        column: x => x.ProductID,
                        principalTable: "SemiSelfProducts",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK__Semi_Tran__Trans__503BEA1C",
                        column: x => x.TransportID,
                        principalTable: "Transportations",
                        principalColumn: "TransportID");
                });

            migrationBuilder.CreateTable(
                name: "NewsTable",
                columns: table => new
                {
                    NewsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NewsContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpireTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NewsTabl__954EBDD3A419A542", x => x.NewsID);
                    table.ForeignKey(
                        name: "FK__NewsTable__Emplo__3587F3E0",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "EmployeeProfile",
                        principalColumn: "EmployeeID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerBlacklist",
                columns: table => new
                {
                    BlacklistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PermissionStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__AFDBF43883D62D6E", x => x.BlacklistID);
                    table.ForeignKey(
                        name: "FK__CustomerB__Custo__160F4887",
                        column: x => x.CustomerID,
                        principalTable: "CustomerProfile",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    OrderStatusID = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__C3905BAFC1E5D4B0", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK__CustomerO__Custo__1CBC4616",
                        column: x => x.CustomerID,
                        principalTable: "CustomerProfile",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK__CustomerO__Order__1DB06A4F",
                        column: x => x.OrderStatusID,
                        principalTable: "OrderStatus",
                        principalColumn: "OrderStatusID");
                    table.ForeignKey(
                        name: "FK__CustomerO__Produ__1EA48E88",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerSupportTickets",
                columns: table => new
                {
                    TicketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    EmployeeID = table.Column<int>(type: "int", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TicketTypeID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusID = table.Column<int>(type: "int", nullable: true),
                    PriorityID = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TicketCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__712CC627B75D65D4", x => x.TicketID);
                    table.ForeignKey(
                        name: "FK__CustomerS__Custo__236943A5",
                        column: x => x.CustomerID,
                        principalTable: "CustomerProfile",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK__CustomerS__Emplo__245D67DE",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK__CustomerS__Prior__25518C17",
                        column: x => x.PriorityID,
                        principalTable: "TicketPriority",
                        principalColumn: "PriorityID");
                    table.ForeignKey(
                        name: "FK__CustomerS__Statu__2645B050",
                        column: x => x.StatusID,
                        principalTable: "TicketStatus",
                        principalColumn: "StatusID");
                    table.ForeignKey(
                        name: "FK__CustomerS__Ticke__2739D489",
                        column: x => x.TicketTypeID,
                        principalTable: "TicketTypes",
                        principalColumn: "TicketTypeID");
                });

            migrationBuilder.CreateTable(
                name: "NewsPics",
                columns: table => new
                {
                    NewsPicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsID = table.Column<int>(type: "int", nullable: true),
                    NewsPic = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPics", x => x.NewsPicID);
                    table.ForeignKey(
                        name: "FK__NewsPics__NewsID__3493CFA7",
                        column: x => x.NewsID,
                        principalTable: "NewsTable",
                        principalColumn: "NewsID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrderFeedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    FeedbackRating = table.Column<int>(type: "int", nullable: true),
                    FeedbackComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__6A4BEDF6B94D8C76", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK__CustomerO__Order__19DFD96B",
                        column: x => x.OrderID,
                        principalTable: "CustomerOrders",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrderMemberInfo",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IDNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CustomerBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__CustomerO__Order__1BC821DD",
                        column: x => x.OrderID,
                        principalTable: "CustomerOrders",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateTable(
                name: "OrderPaymentInfo",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    PaymentAmount = table.Column<int>(type: "int", nullable: true),
                    PaymentStatusID = table.Column<int>(type: "int", nullable: true),
                    TransectionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderPay__9B556A58EDC4925A", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK__OrderPaym__Order__367C1819",
                        column: x => x.OrderID,
                        principalTable: "CustomerOrders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__OrderPaym__Payme__37703C52",
                        column: x => x.PaymentStatusID,
                        principalTable: "PaymentStatus",
                        principalColumn: "PayMentStatusID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerSupportFeedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketID = table.Column<int>(type: "int", nullable: true),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    FeedbackRating = table.Column<int>(type: "int", nullable: true),
                    FeedbackComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__6A4BEDF6A2776794", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK__CustomerS__Ticke__2180FB33",
                        column: x => x.TicketID,
                        principalTable: "CustomerSupportTickets",
                        principalColumn: "TicketID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerSupportMessages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketID = table.Column<int>(type: "int", nullable: true),
                    SenderID = table.Column<int>(type: "int", nullable: true),
                    ReceiverID = table.Column<int>(type: "int", nullable: true),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnreadCount = table.Column<int>(type: "int", nullable: true),
                    AttachmentURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__C87C037CEEF7F82E", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK__CustomerS__Ticke__22751F6C",
                        column: x => x.TicketID,
                        principalTable: "CustomerSupportTickets",
                        principalColumn: "TicketID");
                });

            migrationBuilder.CreateTable(
                name: "SupportAnalysis",
                columns: table => new
                {
                    DateID = table.Column<int>(type: "int", nullable: true),
                    TicketID = table.Column<int>(type: "int", nullable: true),
                    OpenedCount = table.Column<int>(type: "int", nullable: true),
                    ResolvedCount = table.Column<int>(type: "int", nullable: true),
                    RatingAverage = table.Column<int>(type: "int", nullable: true),
                    TopCategory = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__SupportAn__DateI__51300E55",
                        column: x => x.DateID,
                        principalTable: "DateDimension",
                        principalColumn: "DateID");
                    table.ForeignKey(
                        name: "FK__SupportAn__Ticke__5224328E",
                        column: x => x.TicketID,
                        principalTable: "CustomerSupportTickets",
                        principalColumn: "TicketID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouponPics_CouponID",
                table: "CouponPics",
                column: "CouponID");

            migrationBuilder.CreateIndex(
                name: "UQ__Coupons__D3490800F4D03A6F",
                table: "Coupons",
                column: "CouponCode",
                unique: true,
                filter: "[CouponCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBlacklist_CustomerID",
                table: "CustomerBlacklist",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCouponsRecords_CouponID",
                table: "CustomerCouponsRecords",
                column: "CouponID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCouponsRecords_CustomerID",
                table: "CustomerCouponsRecords",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLoginHistory_CustomerID",
                table: "CustomerLoginHistory",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderFeedback_OrderID",
                table: "CustomerOrderFeedback",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrderMemberInfo_OrderID",
                table: "CustomerOrderMemberInfo",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_CustomerID",
                table: "CustomerOrders",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_OrderStatusID",
                table: "CustomerOrders",
                column: "OrderStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_ProductID",
                table: "CustomerOrders",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__A4AE64B9F2B78F1D",
                table: "CustomerProfile",
                column: "CustomerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Level",
                table: "Customers",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__B0C3AC468D583DE7",
                table: "Customers",
                column: "Account",
                unique: true,
                filter: "[Account] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupportFeedback_TicketID",
                table: "CustomerSupportFeedback",
                column: "TicketID");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__712CC626BE2040FE",
                table: "CustomerSupportMessages",
                column: "TicketID",
                unique: true,
                filter: "[TicketID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupportTickets_CustomerID",
                table: "CustomerSupportTickets",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupportTickets_EmployeeID",
                table: "CustomerSupportTickets",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupportTickets_PriorityID",
                table: "CustomerSupportTickets",
                column: "PriorityID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupportTickets_StatusID",
                table: "CustomerSupportTickets",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupportTickets_TicketTypeID",
                table: "CustomerSupportTickets",
                column: "TicketTypeID");

            migrationBuilder.CreateIndex(
                name: "UQ__Employee__7AD04FF047B12E87",
                schema: "dbo",
                table: "EmployeeProfile",
                column: "EmployeeID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RoleID",
                table: "Employees",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "UQ__Employee__B0C3AC463B045A6C",
                table: "Employees",
                column: "Account",
                unique: true,
                filter: "[Account] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_CategoryID",
                table: "FAQs",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_HotelKeywords_HotelID",
                table: "HotelKeywords",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_HotelKeywords_KeywordID",
                table: "HotelKeywords",
                column: "KeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_HotelPics_HotelID",
                table: "HotelPics",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_DistrictID",
                table: "Hotels",
                column: "DistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_RegionID",
                table: "Hotels",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationKeywords_KeywordID",
                table: "LocationKeywords",
                column: "KeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationKeywords_LocationID",
                table: "LocationKeywords",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPics_LocationID",
                table: "LocationPics",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DistrictID",
                table: "Locations",
                column: "DistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_RegionID",
                table: "Locations",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPics_NewsID",
                table: "NewsPics",
                column: "NewsID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTable_EmployeeID",
                table: "NewsTable",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomerID",
                table: "Notifications",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaymentInfo_OrderID",
                table: "OrderPaymentInfo",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaymentInfo_PaymentStatusID",
                table: "OrderPaymentInfo",
                column: "PaymentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAnalysis_ProductID",
                table: "ProductAnalysis",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPics_ProductID",
                table: "ProductPics",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_RegionID",
                table: "Products",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Hotels_HotelID",
                table: "Products_Hotels",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Hotels_ProductID",
                table: "Products_Hotels",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Keywords_KeywordID",
                table: "Products_Keywords",
                column: "KeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Keywords_KeywordsKeywordID",
                table: "Products_Keywords",
                column: "KeywordsKeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Keywords_ProductID",
                table: "Products_Keywords",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Locations_LocationID",
                table: "Products_Locations",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Locations_ProductID",
                table: "Products_Locations",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Promotions_ProductID",
                table: "Products_Promotions",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Promotions_PromoID",
                table: "Products_Promotions",
                column: "PromoID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Restaurants_ProductID",
                table: "Products_Restaurants",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Restaurants_RestaurantID",
                table: "Products_Restaurants",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Transportations_ProductID",
                table: "Products_Transportations",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Transportations_TransportID",
                table: "Products_Transportations",
                column: "TransportID");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantKeywords_KeywordID",
                table: "RestaurantKeywords",
                column: "KeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantKeywords_RestaurantID",
                table: "RestaurantKeywords",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPics_RestaurantID",
                table: "RestaurantPics",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_DistrictID",
                table: "Restaurants",
                column: "DistrictID");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_RegionID",
                table: "Restaurants",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Hotels_HotelID",
                table: "Semi_Hotels",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Hotels_ProductID",
                table: "Semi_Hotels",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Keywords_KeywordID",
                table: "Semi_Keywords",
                column: "KeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Keywords_KeywordsKeywordID",
                table: "Semi_Keywords",
                column: "KeywordsKeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Keywords_ProductID",
                table: "Semi_Keywords",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Locations_LocationID",
                table: "Semi_Locations",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Locations_ProductID",
                table: "Semi_Locations",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Transportations_ProductID",
                table: "Semi_Transportations",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Semi_Transportations_TransportID",
                table: "Semi_Transportations",
                column: "TransportID");

            migrationBuilder.CreateIndex(
                name: "IX_SemiSelfProducts_RegionID",
                table: "SemiSelfProducts",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportAnalysis_DateID",
                table: "SupportAnalysis",
                column: "DateID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportAnalysis_TicketID",
                table: "SupportAnalysis",
                column: "TicketID");

            migrationBuilder.CreateIndex(
                name: "IX_TransportKeywords_KeywordID",
                table: "TransportKeywords",
                column: "KeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_TransportKeywords_TransportID",
                table: "TransportKeywords",
                column: "TransportID");

            migrationBuilder.CreateIndex(
                name: "IX_TransportPics_TransportID",
                table: "TransportPics",
                column: "TransportID");

            migrationBuilder.CreateIndex(
                name: "IX_TripProjectDetails_HotelID",
                table: "TripProjectDetails",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_TripProjectDetails_LocationID",
                table: "TripProjectDetails",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_TripProjectDetails_ProjectID",
                table: "TripProjectDetails",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_TripProjectDetails_RestaurantID",
                table: "TripProjectDetails",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_TripProjectDetails_TransportID",
                table: "TripProjectDetails",
                column: "TransportID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouponPics");

            migrationBuilder.DropTable(
                name: "CustomerBlacklist");

            migrationBuilder.DropTable(
                name: "CustomerCouponsRecords");

            migrationBuilder.DropTable(
                name: "CustomerLoginHistory");

            migrationBuilder.DropTable(
                name: "CustomerOrderFeedback");

            migrationBuilder.DropTable(
                name: "CustomerOrderMemberInfo");

            migrationBuilder.DropTable(
                name: "CustomerSupportFeedback");

            migrationBuilder.DropTable(
                name: "CustomerSupportMessages");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "HotelKeywords");

            migrationBuilder.DropTable(
                name: "HotelPics");

            migrationBuilder.DropTable(
                name: "LocationKeywords");

            migrationBuilder.DropTable(
                name: "LocationPics");

            migrationBuilder.DropTable(
                name: "NewsPics");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderPaymentInfo");

            migrationBuilder.DropTable(
                name: "PendingPayments");

            migrationBuilder.DropTable(
                name: "ProductAnalysis");

            migrationBuilder.DropTable(
                name: "ProductPics");

            migrationBuilder.DropTable(
                name: "Products_Hotels");

            migrationBuilder.DropTable(
                name: "Products_Keywords");

            migrationBuilder.DropTable(
                name: "Products_Locations");

            migrationBuilder.DropTable(
                name: "Products_Promotions");

            migrationBuilder.DropTable(
                name: "Products_Restaurants");

            migrationBuilder.DropTable(
                name: "Products_Transportations");

            migrationBuilder.DropTable(
                name: "RestaurantKeywords");

            migrationBuilder.DropTable(
                name: "RestaurantPics");

            migrationBuilder.DropTable(
                name: "Semi_Hotels");

            migrationBuilder.DropTable(
                name: "Semi_Keywords");

            migrationBuilder.DropTable(
                name: "Semi_Locations");

            migrationBuilder.DropTable(
                name: "Semi_Transportations");

            migrationBuilder.DropTable(
                name: "SupportAnalysis");

            migrationBuilder.DropTable(
                name: "TransportKeywords");

            migrationBuilder.DropTable(
                name: "TransportPics");

            migrationBuilder.DropTable(
                name: "TripProjectDetails");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "FAQCategorys");

            migrationBuilder.DropTable(
                name: "NewsTable");

            migrationBuilder.DropTable(
                name: "CustomerOrders");

            migrationBuilder.DropTable(
                name: "PaymentStatus");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "SemiSelfProducts");

            migrationBuilder.DropTable(
                name: "DateDimension");

            migrationBuilder.DropTable(
                name: "CustomerSupportTickets");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "CustomerTripProjects");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropTable(
                name: "Transportations");

            migrationBuilder.DropTable(
                name: "EmployeeProfile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "CustomerProfile");

            migrationBuilder.DropTable(
                name: "TicketPriority");

            migrationBuilder.DropTable(
                name: "TicketStatus");

            migrationBuilder.DropTable(
                name: "TicketTypes");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "EmployeeRoles");

            migrationBuilder.DropTable(
                name: "IdentityUser");

            migrationBuilder.DropTable(
                name: "CustomerLevels");
        }
    }
}
