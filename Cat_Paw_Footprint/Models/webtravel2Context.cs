using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cat_Paw_Footprint.Models;

public partial class webtravel2Context : DbContext
{
    public webtravel2Context()
    {
    }

    public webtravel2Context(DbContextOptions<webtravel2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<CouponPics> CouponPics { get; set; }

    public virtual DbSet<Coupons> Coupons { get; set; }

    public virtual DbSet<CustomerBlacklist> CustomerBlacklist { get; set; }

    public virtual DbSet<CustomerCouponsRecords> CustomerCouponsRecords { get; set; }

    public virtual DbSet<CustomerLevels> CustomerLevels { get; set; }

    public virtual DbSet<CustomerLoginHistory> CustomerLoginHistory { get; set; }

    public virtual DbSet<CustomerOrderFeedback> CustomerOrderFeedback { get; set; }

    public virtual DbSet<CustomerOrderMemberInfo> CustomerOrderMemberInfo { get; set; }

    public virtual DbSet<CustomerOrders> CustomerOrders { get; set; }

    public virtual DbSet<CustomerProfile> CustomerProfile { get; set; }

    public virtual DbSet<CustomerSupportFeedback> CustomerSupportFeedback { get; set; }

    public virtual DbSet<CustomerSupportMessages> CustomerSupportMessages { get; set; }

    public virtual DbSet<CustomerSupportTickets> CustomerSupportTickets { get; set; }

    public virtual DbSet<CustomerTripProjects> CustomerTripProjects { get; set; }

    public virtual DbSet<Customers> Customers { get; set; }

    public virtual DbSet<DateDimension> DateDimension { get; set; }

    public virtual DbSet<Districts> Districts { get; set; }

    public virtual DbSet<EmployeeProfile> EmployeeProfile { get; set; }

    public virtual DbSet<EmployeeRoles> EmployeeRoles { get; set; }

    public virtual DbSet<Employees> Employees { get; set; }

    public virtual DbSet<FAQCategorys> FAQCategorys { get; set; }

    public virtual DbSet<FAQs> FAQs { get; set; }

    public virtual DbSet<HotelKeywords> HotelKeywords { get; set; }

    public virtual DbSet<HotelPics> HotelPics { get; set; }

    public virtual DbSet<Hotels> Hotels { get; set; }

    public virtual DbSet<Keywords> Keywords { get; set; }

    public virtual DbSet<LocationKeywords> LocationKeywords { get; set; }

    public virtual DbSet<LocationPics> LocationPics { get; set; }

    public virtual DbSet<Locations> Locations { get; set; }

    public virtual DbSet<NewsPics> NewsPics { get; set; }

    public virtual DbSet<NewsTable> NewsTable { get; set; }

    public virtual DbSet<OrderPaymentInfo> OrderPaymentInfo { get; set; }

    public virtual DbSet<OrderStatus> OrderStatus { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatus { get; set; }

    public virtual DbSet<ProductAnalysis> ProductAnalysis { get; set; }

    public virtual DbSet<ProductPics> ProductPics { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<Products_Hotels> Products_Hotels { get; set; }

    public virtual DbSet<Products_Locations> Products_Locations { get; set; }

    public virtual DbSet<Products_Promotions> Products_Promotions { get; set; }

    public virtual DbSet<Products_Restaurants> Products_Restaurants { get; set; }

    public virtual DbSet<Products_Transportations> Products_Transportations { get; set; }

    public virtual DbSet<Promotions> Promotions { get; set; }

    public virtual DbSet<Regions> Regions { get; set; }

    public virtual DbSet<RestaurantKeywords> RestaurantKeywords { get; set; }

    public virtual DbSet<RestaurantPics> RestaurantPics { get; set; }

    public virtual DbSet<Restaurants> Restaurants { get; set; }

    public virtual DbSet<SemiSelfProducts> SemiSelfProducts { get; set; }

    public virtual DbSet<Semi_Hotels> Semi_Hotels { get; set; }

    public virtual DbSet<Semi_Locations> Semi_Locations { get; set; }

    public virtual DbSet<Semi_Transportations> Semi_Transportations { get; set; }

    public virtual DbSet<SupportAnalysis> SupportAnalysis { get; set; }

    public virtual DbSet<TicketPriority> TicketPriority { get; set; }

    public virtual DbSet<TicketStatus> TicketStatus { get; set; }

    public virtual DbSet<TicketTypes> TicketTypes { get; set; }

    public virtual DbSet<TransportKeywords> TransportKeywords { get; set; }

    public virtual DbSet<TransportPics> TransportPics { get; set; }

    public virtual DbSet<Transportations> Transportations { get; set; }

    public virtual DbSet<TripProjectDetails> TripProjectDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-GD2BR52T\\SQLEXPRESS;Initial Catalog=web-travel2;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CouponPics>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Coupon).WithMany()
                .HasForeignKey(d => d.CouponID)
                .HasConstraintName("FK__CouponPic__Coupo__151B244E");
        });

        modelBuilder.Entity<Coupons>(entity =>
        {
            entity.HasKey(e => e.CouponID).HasName("PK__Coupons__384AF1DA962AEEC5");

            entity.HasIndex(e => e.CouponCode, "UQ__Coupons__D3490800F4D03A6F").IsUnique();

            entity.Property(e => e.CouponCode).HasMaxLength(50);
            entity.Property(e => e.DiscountValue).HasColumnType("numeric(7, 2)");
        });

        modelBuilder.Entity<CustomerBlacklist>(entity =>
        {
            entity.HasKey(e => e.BlacklistID).HasName("PK__Customer__AFDBF43883D62D6E");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerBlacklist)
                .HasPrincipalKey(p => p.CustomerID)
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK__CustomerB__Custo__160F4887");
        });

        modelBuilder.Entity<CustomerCouponsRecords>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Coupon).WithMany()
                .HasForeignKey(d => d.CouponID)
                .HasConstraintName("FK__CustomerC__Coupo__17036CC0");

            entity.HasOne(d => d.Customer).WithMany()
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK__CustomerC__Custo__17F790F9");
        });

        modelBuilder.Entity<CustomerLevels>(entity =>
        {
            entity.HasKey(e => e.Level).HasName("PK__Customer__AAF8996343B0DC7B");

            entity.Property(e => e.Level).ValueGeneratedNever();
            entity.Property(e => e.LevelName).HasMaxLength(20);
        });

        modelBuilder.Entity<CustomerLoginHistory>(entity =>
        {
            entity.HasKey(e => e.LoginLogID).HasName("PK__Customer__D42E7ACC91490DF1");

            entity.Property(e => e.LoginLogID).ValueGeneratedNever();
            entity.Property(e => e.LoginIP).HasMaxLength(45);

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerLoginHistory)
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK__CustomerL__Custo__18EBB532");
        });

        modelBuilder.Entity<CustomerOrderFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackID).HasName("PK__Customer__6A4BEDF6B94D8C76");

            entity.HasOne(d => d.Order).WithMany(p => p.CustomerOrderFeedback)
                .HasForeignKey(d => d.OrderID)
                .HasConstraintName("FK__CustomerO__Order__19DFD96B");
        });

        modelBuilder.Entity<CustomerOrderMemberInfo>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CustomerName).HasMaxLength(50);
            entity.Property(e => e.IDNumber).HasMaxLength(20);

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderID)
                .HasConstraintName("FK__CustomerO__Order__1BC821DD");
        });

        modelBuilder.Entity<CustomerOrders>(entity =>
        {
            entity.HasKey(e => e.OrderID).HasName("PK__Customer__C3905BAFC1E5D4B0");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerOrders)
                .HasPrincipalKey(p => p.CustomerID)
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK__CustomerO__Custo__1CBC4616");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.CustomerOrders)
                .HasForeignKey(d => d.OrderStatusID)
                .HasConstraintName("FK__CustomerO__Order__1DB06A4F");

            entity.HasOne(d => d.Product).WithMany(p => p.CustomerOrders)
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__CustomerO__Produ__1EA48E88");
        });

        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.HasKey(e => e.CustomerProfilesID).HasName("PK__Customer__13B52926385BD265");

            entity.HasIndex(e => e.CustomerID, "UQ__Customer__A4AE64B9F2B78F1D").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CustomerID).IsRequired();
            entity.Property(e => e.CustomerName).HasMaxLength(50);
            entity.Property(e => e.CustomerProfileCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IDNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithOne(p => p.CustomerProfile)
                .HasForeignKey<CustomerProfile>(d => d.CustomerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__Custo__1F98B2C1");
        });

        modelBuilder.Entity<CustomerSupportFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackID).HasName("PK__Customer__6A4BEDF6A2776794");

            entity.HasOne(d => d.Ticket).WithMany(p => p.CustomerSupportFeedback)
                .HasForeignKey(d => d.TicketID)
                .HasConstraintName("FK__CustomerS__Ticke__2180FB33");
        });

        modelBuilder.Entity<CustomerSupportMessages>(entity =>
        {
            entity.HasKey(e => e.MessageID).HasName("PK__Customer__C87C037CEEF7F82E");

            entity.HasIndex(e => e.TicketID, "UQ__Customer__712CC626BE2040FE").IsUnique();

            entity.Property(e => e.AttachmentURL).HasMaxLength(500);

            entity.HasOne(d => d.Ticket).WithMany(p => p.Messages)
				.HasForeignKey(d => d.TicketID)
		.HasConstraintName("FK__CustomerS__Ticke__22751F6C");
		});

        modelBuilder.Entity<CustomerSupportTickets>(entity =>
        {
            entity.HasKey(e => e.TicketID).HasName("PK__Customer__712CC627B75D65D4");

            entity.Property(e => e.Subject).HasMaxLength(200);

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerSupportTickets)
                .HasPrincipalKey(p => p.CustomerID)
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK__CustomerS__Custo__236943A5");

            entity.HasOne(d => d.Employee).WithMany(p => p.CustomerSupportTickets)
                .HasForeignKey(d => d.EmployeeID)
                .HasConstraintName("FK__CustomerS__Emplo__245D67DE");

            entity.HasOne(d => d.Priority).WithMany(p => p.CustomerSupportTickets)
                .HasForeignKey(d => d.PriorityID)
                .HasConstraintName("FK__CustomerS__Prior__25518C17");

            entity.HasOne(d => d.Status).WithMany(p => p.CustomerSupportTickets)
                .HasForeignKey(d => d.StatusID)
                .HasConstraintName("FK__CustomerS__Statu__2645B050");

            entity.HasOne(d => d.TicketType).WithMany(p => p.CustomerSupportTickets)
                .HasForeignKey(d => d.TicketTypeID)
                .HasConstraintName("FK__CustomerS__Ticke__2739D489");
        });

        modelBuilder.Entity<CustomerTripProjects>(entity =>
        {
            entity.HasKey(e => e.ProjectID).HasName("PK__Customer__761ABED0C9A3562A");

            entity.Property(e => e.ProjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<Customers>(entity =>
        {
            entity.HasKey(e => e.CustomerID).HasName("PK__Customer__A4AE64B82A50E373");

            entity.HasIndex(e => e.Account, "UQ__Customer__B0C3AC468D583DE7").IsUnique();

            entity.Property(e => e.Account).HasMaxLength(50);
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(200);

            entity.HasOne(d => d.LevelNavigation).WithMany(p => p.Customers)
                .HasForeignKey(d => d.Level)
                .HasConstraintName("FK__Customers__Level__208CD6FA");
        });

        modelBuilder.Entity<DateDimension>(entity =>
        {
            entity.HasKey(e => e.DateID).HasName("PK__DateDime__A426F253D3C63C50");

            entity.Property(e => e.DayName).HasMaxLength(10);
        });

        modelBuilder.Entity<Districts>(entity =>
        {
            entity.HasKey(e => e.DistrictID).HasName("PK__District__85FDA4A63BF24E8A");

            entity.Property(e => e.DistrictName).HasMaxLength(20);
        });

        modelBuilder.Entity<EmployeeProfile>(entity =>
        {
            entity.HasKey(e => e.EmployeeProfileID).HasName("PK__Employee__7CC01C3EA0B9E04D");

            entity.HasIndex(e => e.EmployeeID, "UQ__Employee__7AD04FF047B12E87").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeID).IsRequired();
            entity.Property(e => e.EmployeeName).HasMaxLength(50);
            entity.Property(e => e.EmployeeProfileCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IDNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithOne(p => p.EmployeeProfile)
                .HasForeignKey<EmployeeProfile>(d => d.EmployeeID)
                .HasConstraintName("FK__EmployeeP__Emplo__282DF8C2");
        });

        modelBuilder.Entity<EmployeeRoles>(entity =>
        {
            entity.HasKey(e => e.RoleID).HasName("PK__Employee__8AFACE3AA240E365");

            entity.Property(e => e.RoleID).ValueGeneratedNever();
            entity.Property(e => e.RoleName).HasMaxLength(20);
        });

        modelBuilder.Entity<Employees>(entity =>
        {
            entity.HasKey(e => e.EmployeeID).HasName("PK__Employee__7AD04FF1804EF882");

            entity.HasIndex(e => e.Account, "UQ__Employee__B0C3AC463B045A6C").IsUnique();

            entity.Property(e => e.Account).HasMaxLength(50);
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password).HasMaxLength(200);

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleID)
                .HasConstraintName("FK__Employees__RoleI__29221CFB");
        });

        modelBuilder.Entity<FAQCategorys>(entity =>
        {
            entity.HasKey(e => e.CategoryID).HasName("PK__FAQCateg__19093A2BAD7796F5");

            entity.Property(e => e.CategoryName).HasMaxLength(200);
        });

        modelBuilder.Entity<FAQs>(entity =>
        {
            entity.HasKey(e => e.FAQID).HasName("PK__FAQs__4B89D1E24C3A6A93");

            entity.Property(e => e.Question).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.FAQs)
                .HasForeignKey(d => d.CategoryID)
                .HasConstraintName("FK__FAQs__CategoryID__2A164134");
        });

        modelBuilder.Entity<HotelKeywords>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Hotel).WithMany()
                .HasForeignKey(d => d.HotelID)
                .HasConstraintName("FK__HotelKeyw__Hotel__2B0A656D");

            entity.HasOne(d => d.Keyword).WithMany()
                .HasForeignKey(d => d.KeywordID)
                .HasConstraintName("FK__HotelKeyw__Keywo__2BFE89A6");
        });

        modelBuilder.Entity<HotelPics>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Hotel).WithMany()
                .HasForeignKey(d => d.HotelID)
                .HasConstraintName("FK__HotelPics__Hotel__2CF2ADDF");
        });

        modelBuilder.Entity<Hotels>(entity =>
        {
            entity.HasKey(e => e.HotelID).HasName("PK__Hotels__46023BBF1005A475");

            entity.Property(e => e.HotelAddr).HasMaxLength(200);
            entity.Property(e => e.HotelCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.HotelLat).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.HotelLng).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.HotelName).HasMaxLength(200);
            entity.Property(e => e.Rating).HasColumnType("numeric(2, 1)");

            entity.HasOne(d => d.District).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.DistrictID)
                .HasConstraintName("FK__Hotels__District__2DE6D218");

            entity.HasOne(d => d.Region).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.RegionID)
                .HasConstraintName("FK_Hotels_Regions");
        });

        modelBuilder.Entity<Keywords>(entity =>
        {
            entity.HasKey(e => e.KeywordID).HasName("PK__Keywords__37C135C138B67486");

            entity.Property(e => e.KeywordID).ValueGeneratedNever();
            entity.Property(e => e.Keyword).HasMaxLength(15);
        });

        modelBuilder.Entity<LocationKeywords>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Keyword).WithMany()
                .HasForeignKey(d => d.KeywordID)
                .HasConstraintName("FK__LocationK__Keywo__2FCF1A8A");

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationID)
                .HasConstraintName("FK__LocationK__Locat__30C33EC3");
        });

        modelBuilder.Entity<LocationPics>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationID)
                .HasConstraintName("FK__LocationP__Locat__31B762FC");
        });

        modelBuilder.Entity<Locations>(entity =>
        {
            entity.HasKey(e => e.LocationID).HasName("PK__Location__E7FEA4777BCBA51B");

            entity.Property(e => e.LocationAddr).HasMaxLength(200);
            entity.Property(e => e.LocationCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LocationLat).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.LocationLng).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.LocationName).HasMaxLength(50);
            entity.Property(e => e.Rating).HasColumnType("numeric(2, 1)");

            entity.HasOne(d => d.District).WithMany(p => p.Locations)
                .HasForeignKey(d => d.DistrictID)
                .HasConstraintName("FK__Locations__Distr__32AB8735");

            entity.HasOne(d => d.Region).WithMany(p => p.Locations)
                .HasForeignKey(d => d.RegionID)
                .HasConstraintName("FK_Locations_Regions");
        });

        modelBuilder.Entity<NewsPics>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.News).WithMany()
                .HasForeignKey(d => d.NewsID)
                .HasConstraintName("FK__NewsPics__NewsID__3493CFA7");
        });

        modelBuilder.Entity<NewsTable>(entity =>
        {
            entity.HasKey(e => e.NewsID).HasName("PK__NewsTabl__954EBDD3A419A542");

            entity.Property(e => e.NewsTitle).HasMaxLength(200);

            entity.HasOne(d => d.Employee).WithMany(p => p.NewsTable)
                .HasPrincipalKey(p => p.EmployeeID)
                .HasForeignKey(d => d.EmployeeID)
                .HasConstraintName("FK__NewsTable__Emplo__3587F3E0");
        });

        modelBuilder.Entity<OrderPaymentInfo>(entity =>
        {
            entity.HasKey(e => e.PaymentID).HasName("PK__OrderPay__9B556A58EDC4925A");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderPaymentInfo)
                .HasForeignKey(d => d.OrderID)
                .HasConstraintName("FK__OrderPaym__Order__367C1819");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.OrderPaymentInfo)
                .HasForeignKey(d => d.PaymentStatusID)
                .HasConstraintName("FK__OrderPaym__Payme__37703C52");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusID).HasName("PK__OrderSta__BC674F410981CD78");

            entity.Property(e => e.StatusDesc).HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PayMentStatusID).HasName("PK__PaymentS__F177127F257E414E");

            entity.Property(e => e.StatusDesc).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductAnalysis>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__ProductAn__Produ__3864608B");
        });

        modelBuilder.Entity<ProductPics>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__ProductPi__Produ__395884C4");
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(e => e.ProductID).HasName("PK__Products__B40CC6EDDACA8D2D");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProductDesc).HasMaxLength(200);
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.ProductNote).HasMaxLength(200);

            entity.HasOne(d => d.Region).WithMany(p => p.Products)
                .HasForeignKey(d => d.RegionID)
                .HasConstraintName("FK_Products_Regions");
        });

        modelBuilder.Entity<Products_Hotels>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Hotel).WithMany()
                .HasForeignKey(d => d.HotelID)
                .HasConstraintName("FK__Products___Hotel__3B40CD36");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Products___Produ__3C34F16F");
        });

        modelBuilder.Entity<Products_Locations>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationID)
                .HasConstraintName("FK__Products___Locat__3D2915A8");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Products___Produ__3E1D39E1");
        });

        modelBuilder.Entity<Products_Promotions>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Products___Produ__3F115E1A");

            entity.HasOne(d => d.Promo).WithMany()
                .HasForeignKey(d => d.PromoID)
                .HasConstraintName("FK__Products___Promo__40058253");
        });

        modelBuilder.Entity<Products_Restaurants>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Products___Produ__40F9A68C");

            entity.HasOne(d => d.Restaurant).WithMany()
                .HasForeignKey(d => d.RestaurantID)
                .HasConstraintName("FK__Products___Resta__42E1EEFE");
        });

        modelBuilder.Entity<Products_Transportations>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Products___Produ__44CA3770");

            entity.HasOne(d => d.Transport).WithMany()
                .HasForeignKey(d => d.TransportID)
                .HasConstraintName("FK__Products___Trans__45BE5BA9");
        });

        modelBuilder.Entity<Promotions>(entity =>
        {
            entity.HasKey(e => e.PromoID).HasName("PK__Promotio__33D334D0D8C5EB2C");

            entity.Property(e => e.DiscountValue).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PromoName).HasMaxLength(200);
        });

        modelBuilder.Entity<Regions>(entity =>
        {
            entity.HasKey(e => e.RegionID);

            entity.Property(e => e.RegionID).ValueGeneratedNever();
            entity.Property(e => e.RegionName).HasMaxLength(10);
        });

        modelBuilder.Entity<RestaurantKeywords>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Keyword).WithMany()
                .HasForeignKey(d => d.KeywordID)
                .HasConstraintName("FK__Restauran__Keywo__46B27FE2");

            entity.HasOne(d => d.Restaurant).WithMany()
                .HasForeignKey(d => d.RestaurantID)
                .HasConstraintName("FK__Restauran__Resta__47A6A41B");
        });

        modelBuilder.Entity<RestaurantPics>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Restaurant).WithMany()
                .HasForeignKey(d => d.RestaurantID)
                .HasConstraintName("FK__Restauran__Resta__489AC854");
        });

        modelBuilder.Entity<Restaurants>(entity =>
        {
            entity.HasKey(e => e.RestaurantID).HasName("PK__Restaura__87454CB50407FEEF");

            entity.Property(e => e.Rating).HasColumnType("numeric(2, 1)");
            entity.Property(e => e.RestaurantAddr).HasMaxLength(200);
            entity.Property(e => e.RestaurantLat).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.RestaurantLng).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.RestaurantName).HasMaxLength(50);

            entity.HasOne(d => d.District).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.DistrictID)
                .HasConstraintName("FK__Restauran__Distr__498EEC8D");

            entity.HasOne(d => d.Region).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.RegionID)
                .HasConstraintName("FK_Restaurants_Regions");
        });

        modelBuilder.Entity<SemiSelfProducts>(entity =>
        {
            entity.HasKey(e => e.ProductID).HasName("PK__SemiSelf__B40CC6ED1BE67600");

            entity.Property(e => e.ProductCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<Semi_Hotels>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Hotel).WithMany()
                .HasForeignKey(d => d.HotelID)
                .HasConstraintName("FK__Semi_Hote__Hotel__4B7734FF");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Semi_Hote__Produ__4C6B5938");
        });

        modelBuilder.Entity<Semi_Locations>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationID)
                .HasConstraintName("FK__Semi_Loca__Locat__4D5F7D71");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Semi_Loca__Produ__4E53A1AA");
        });

        modelBuilder.Entity<Semi_Transportations>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK__Semi_Tran__Produ__4F47C5E3");

            entity.HasOne(d => d.Transport).WithMany()
                .HasForeignKey(d => d.TransportID)
                .HasConstraintName("FK__Semi_Tran__Trans__503BEA1C");
        });

        modelBuilder.Entity<SupportAnalysis>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.TopCategory).HasMaxLength(200);

            entity.HasOne(d => d.Date).WithMany()
                .HasForeignKey(d => d.DateID)
                .HasConstraintName("FK__SupportAn__DateI__51300E55");

            entity.HasOne(d => d.Ticket).WithMany()
                .HasForeignKey(d => d.TicketID)
                .HasConstraintName("FK__SupportAn__Ticke__5224328E");
        });

        modelBuilder.Entity<TicketPriority>(entity =>
        {
            entity.HasKey(e => e.PriorityID).HasName("PK__TicketPr__D0A3D0DE6273C0C0");

            entity.Property(e => e.PriorityDesc).HasMaxLength(100);
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.StatusID).HasName("PK__TicketSt__C8EE2043F7460F01");

            entity.Property(e => e.StatusDesc).HasMaxLength(100);
        });

        modelBuilder.Entity<TicketTypes>(entity =>
        {
            entity.HasKey(e => e.TicketTypeID).HasName("PK__TicketTy__6CD6845185DEFCB2");

            entity.Property(e => e.TicketTypeName).HasMaxLength(200);
        });

        modelBuilder.Entity<TransportKeywords>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Keyword).WithMany()
                .HasForeignKey(d => d.KeywordID)
                .HasConstraintName("FK__Transport__Keywo__531856C7");

            entity.HasOne(d => d.Transport).WithMany()
                .HasForeignKey(d => d.TransportID)
                .HasConstraintName("FK__Transport__Trans__540C7B00");
        });

        modelBuilder.Entity<TransportPics>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Transport).WithMany()
                .HasForeignKey(d => d.TransportID)
                .HasConstraintName("FK__Transport__Trans__55009F39");
        });

        modelBuilder.Entity<Transportations>(entity =>
        {
            entity.HasKey(e => e.TransportID).HasName("PK__Transpor__19E9A17DE87E642A");

            entity.Property(e => e.Rating).HasColumnType("numeric(2, 1)");
            entity.Property(e => e.TransportCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TransportName).HasMaxLength(30);
        });

        modelBuilder.Entity<TripProjectDetails>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.TripType).HasMaxLength(50);

            entity.HasOne(d => d.Hotel).WithMany()
                .HasForeignKey(d => d.HotelID)
                .HasConstraintName("FK__TripProje__Hotel__55F4C372");

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationID)
                .HasConstraintName("FK__TripProje__Locat__56E8E7AB");

            entity.HasOne(d => d.Project).WithMany()
                .HasForeignKey(d => d.ProjectID)
                .HasConstraintName("FK__TripProje__Proje__57DD0BE4");

            entity.HasOne(d => d.Restaurant).WithMany()
                .HasForeignKey(d => d.RestaurantID)
                .HasConstraintName("FK__TripProje__Resta__58D1301D");

            entity.HasOne(d => d.Transport).WithMany()
                .HasForeignKey(d => d.TransportID)
                .HasConstraintName("FK__TripProje__Trans__59C55456");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
