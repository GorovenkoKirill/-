USE [master]
GO
/****** Object:  Database [GasCompanyDB]    Script Date: 26.05.2025 11:55:37 ******/
CREATE DATABASE [GasCompanyDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'GasCompanyDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\GasCompanyDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'GasCompanyDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\GasCompanyDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [GasCompanyDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [GasCompanyDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [GasCompanyDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [GasCompanyDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [GasCompanyDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [GasCompanyDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [GasCompanyDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [GasCompanyDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [GasCompanyDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [GasCompanyDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [GasCompanyDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [GasCompanyDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [GasCompanyDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [GasCompanyDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [GasCompanyDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [GasCompanyDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [GasCompanyDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [GasCompanyDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [GasCompanyDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [GasCompanyDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [GasCompanyDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [GasCompanyDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [GasCompanyDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [GasCompanyDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [GasCompanyDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [GasCompanyDB] SET  MULTI_USER 
GO
ALTER DATABASE [GasCompanyDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [GasCompanyDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [GasCompanyDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [GasCompanyDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [GasCompanyDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [GasCompanyDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [GasCompanyDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [GasCompanyDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [GasCompanyDB]
GO
/****** Object:  Table [dbo].[Apartments]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Apartments](
	[apartment_id] [int] IDENTITY(1,1) NOT NULL,
	[building_id] [int] NOT NULL,
	[apartment_number] [nvarchar](10) NOT NULL,
	[area] [decimal](10, 2) NOT NULL,
	[resident_count] [int] NOT NULL,
	[client_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[apartment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[log_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[action_type] [nvarchar](20) NOT NULL,
	[table_name] [nvarchar](50) NOT NULL,
	[record_id] [int] NOT NULL,
	[field_name] [nvarchar](50) NULL,
	[old_value] [nvarchar](max) NULL,
	[new_value] [nvarchar](max) NULL,
	[action_date] [datetime] NOT NULL,
	[ip_address] [nvarchar](50) NULL,
	[user_agent] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Buildings]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Buildings](
	[building_id] [int] IDENTITY(1,1) NOT NULL,
	[street_id] [int] NOT NULL,
	[house_number] [nvarchar](20) NOT NULL,
	[building_letter] [nvarchar](5) NULL,
	[apartments_count] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[building_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[client_id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [nvarchar](50) NOT NULL,
	[last_name] [nvarchar](50) NOT NULL,
	[middle_name] [nvarchar](50) NULL,
	[phone] [nvarchar](20) NOT NULL,
	[email] [nvarchar](100) NULL,
	[passport_number] [nvarchar](20) NULL,
	[passport_issued_by] [nvarchar](255) NULL,
	[registration_date] [date] NOT NULL,
	[is_active] [bit] NOT NULL,
	[created_by] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[client_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GasMeters]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GasMeters](
	[meter_id] [int] IDENTITY(1,1) NOT NULL,
	[apartment_id] [int] NOT NULL,
	[meter_type_id] [int] NOT NULL,
	[serial_number] [nvarchar](50) NOT NULL,
	[installation_date] [date] NOT NULL,
	[initial_reading] [decimal](12, 3) NOT NULL,
	[last_verification_date] [date] NOT NULL,
	[next_verification_date] [date] NOT NULL,
	[is_active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[meter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[serial_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[invoice_id] [int] IDENTITY(1,1) NOT NULL,
	[apartment_id] [int] NOT NULL,
	[period] [date] NOT NULL,
	[tariff_id] [int] NOT NULL,
	[consumption] [decimal](12, 3) NOT NULL,
	[amount] [decimal](12, 2) NOT NULL,
	[issue_date] [date] NOT NULL,
	[due_date] [date] NOT NULL,
	[status] [nvarchar](20) NOT NULL,
	[created_by] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[invoice_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeterReadings]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterReadings](
	[reading_id] [int] IDENTITY(1,1) NOT NULL,
	[meter_id] [int] NOT NULL,
	[reading_date] [date] NOT NULL,
	[current_reading] [decimal](12, 3) NOT NULL,
	[consumption] [decimal](12, 3) NOT NULL,
	[is_verified] [bit] NOT NULL,
	[verification_date] [date] NULL,
	[verified_by] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[reading_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeterTypes]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterTypes](
	[meter_type_id] [int] IDENTITY(1,1) NOT NULL,
	[type_name] [nvarchar](100) NOT NULL,
	[manufacturer] [nvarchar](100) NULL,
	[verification_period] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[meter_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[payment_id] [int] IDENTITY(1,1) NOT NULL,
	[invoice_id] [int] NOT NULL,
	[payment_date] [datetime] NOT NULL,
	[amount] [decimal](12, 2) NOT NULL,
	[payment_method] [nvarchar](50) NOT NULL,
	[receipt_number] [nvarchar](50) NULL,
	[operator_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[payment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Regions]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Regions](
	[region_id] [int] IDENTITY(1,1) NOT NULL,
	[region_name] [nvarchar](100) NOT NULL,
	[description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[region_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Streets]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Streets](
	[street_id] [int] IDENTITY(1,1) NOT NULL,
	[region_id] [int] NOT NULL,
	[street_name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[street_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUsers]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUsers](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[password_hash] [nvarchar](255) NOT NULL,
	[first_name] [nvarchar](50) NOT NULL,
	[last_name] [nvarchar](50) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[phone] [nvarchar](20) NULL,
	[role] [nvarchar](30) NOT NULL,
	[is_active] [bit] NOT NULL,
	[last_login] [datetime] NULL,
	[created_at] [datetime] NOT NULL,
	[modified_at] [datetime] NULL,
	[department] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tariffs]    Script Date: 26.05.2025 11:55:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tariffs](
	[tariff_id] [int] IDENTITY(1,1) NOT NULL,
	[tariff_name] [nvarchar](100) NOT NULL,
	[rate] [decimal](10, 4) NOT NULL,
	[start_date] [date] NOT NULL,
	[end_date] [date] NULL,
	[description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[tariff_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Apartments] ADD  DEFAULT ((1)) FOR [resident_count]
GO
ALTER TABLE [dbo].[AuditLog] ADD  DEFAULT (getdate()) FOR [action_date]
GO
ALTER TABLE [dbo].[Buildings] ADD  DEFAULT ((0)) FOR [apartments_count]
GO
ALTER TABLE [dbo].[Clients] ADD  DEFAULT (getdate()) FOR [registration_date]
GO
ALTER TABLE [dbo].[Clients] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[GasMeters] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (getdate()) FOR [issue_date]
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ('Не оплачено') FOR [status]
GO
ALTER TABLE [dbo].[MeterReadings] ADD  DEFAULT ((0)) FOR [is_verified]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (getdate()) FOR [payment_date]
GO
ALTER TABLE [dbo].[SystemUsers] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[SystemUsers] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Apartments]  WITH CHECK ADD FOREIGN KEY([building_id])
REFERENCES [dbo].[Buildings] ([building_id])
GO
ALTER TABLE [dbo].[Apartments]  WITH CHECK ADD FOREIGN KEY([client_id])
REFERENCES [dbo].[Clients] ([client_id])
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[SystemUsers] ([user_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Buildings]  WITH CHECK ADD FOREIGN KEY([street_id])
REFERENCES [dbo].[Streets] ([street_id])
GO
ALTER TABLE [dbo].[Clients]  WITH CHECK ADD  CONSTRAINT [FK_Clients_SystemUsers] FOREIGN KEY([created_by])
REFERENCES [dbo].[SystemUsers] ([user_id])
GO
ALTER TABLE [dbo].[Clients] CHECK CONSTRAINT [FK_Clients_SystemUsers]
GO
ALTER TABLE [dbo].[GasMeters]  WITH CHECK ADD FOREIGN KEY([apartment_id])
REFERENCES [dbo].[Apartments] ([apartment_id])
GO
ALTER TABLE [dbo].[GasMeters]  WITH CHECK ADD FOREIGN KEY([meter_type_id])
REFERENCES [dbo].[MeterTypes] ([meter_type_id])
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([apartment_id])
REFERENCES [dbo].[Apartments] ([apartment_id])
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([tariff_id])
REFERENCES [dbo].[Tariffs] ([tariff_id])
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_SystemUsers] FOREIGN KEY([created_by])
REFERENCES [dbo].[SystemUsers] ([user_id])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_SystemUsers]
GO
ALTER TABLE [dbo].[MeterReadings]  WITH CHECK ADD FOREIGN KEY([meter_id])
REFERENCES [dbo].[GasMeters] ([meter_id])
GO
ALTER TABLE [dbo].[MeterReadings]  WITH CHECK ADD  CONSTRAINT [FK_MeterReadings_SystemUsers] FOREIGN KEY([verified_by])
REFERENCES [dbo].[SystemUsers] ([user_id])
GO
ALTER TABLE [dbo].[MeterReadings] CHECK CONSTRAINT [FK_MeterReadings_SystemUsers]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD FOREIGN KEY([invoice_id])
REFERENCES [dbo].[Invoices] ([invoice_id])
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_SystemUsers] FOREIGN KEY([operator_id])
REFERENCES [dbo].[SystemUsers] ([user_id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_SystemUsers]
GO
ALTER TABLE [dbo].[Streets]  WITH CHECK ADD FOREIGN KEY([region_id])
REFERENCES [dbo].[Regions] ([region_id])
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD CHECK  (([action_type]='ACCESS' OR [action_type]='LOGOUT' OR [action_type]='LOGIN' OR [action_type]='DELETE' OR [action_type]='UPDATE' OR [action_type]='CREATE'))
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD CHECK  (([status]='Оплачено' OR [status]='Частично оплачено' OR [status]='Не оплачено'))
GO
ALTER TABLE [dbo].[MeterReadings]  WITH CHECK ADD  CONSTRAINT [CHK_Reading] CHECK  (([current_reading]>=(0) AND [consumption]>=(0)))
GO
ALTER TABLE [dbo].[MeterReadings] CHECK CONSTRAINT [CHK_Reading]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD CHECK  (([payment_method]='Электронный платеж' OR [payment_method]='Банковский перевод' OR [payment_method]='Банковская карта' OR [payment_method]='Наличные'))
GO
ALTER TABLE [dbo].[SystemUsers]  WITH CHECK ADD CHECK  (([role]='Менеджер' OR [role]='Техник' OR [role]='Бухгалтер' OR [role]='Оператор' OR [role]='Администратор'))
GO
USE [master]
GO
ALTER DATABASE [GasCompanyDB] SET  READ_WRITE 
GO
