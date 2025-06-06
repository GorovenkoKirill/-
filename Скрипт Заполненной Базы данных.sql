USE [master]
GO
/****** Object:  Database [GasCompanyDB]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[Apartments]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[AuditLog]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[Buildings]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[Clients]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[GasMeters]    Script Date: 26.05.2025 11:54:46 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[MeterReadings]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[MeterTypes]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[Payments]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[Regions]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[Streets]    Script Date: 26.05.2025 11:54:46 ******/
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
/****** Object:  Table [dbo].[SystemUsers]    Script Date: 26.05.2025 11:54:46 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tariffs]    Script Date: 26.05.2025 11:54:46 ******/
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
SET IDENTITY_INSERT [dbo].[Apartments] ON 

INSERT [dbo].[Apartments] ([apartment_id], [building_id], [apartment_number], [area], [resident_count], [client_id]) VALUES (1, 1, N'1', CAST(60.50 AS Decimal(10, 2)), 2, 1)
INSERT [dbo].[Apartments] ([apartment_id], [building_id], [apartment_number], [area], [resident_count], [client_id]) VALUES (2, 1, N'5', CAST(75.20 AS Decimal(10, 2)), 3, 2)
INSERT [dbo].[Apartments] ([apartment_id], [building_id], [apartment_number], [area], [resident_count], [client_id]) VALUES (3, 2, N'2', CAST(45.00 AS Decimal(10, 2)), 1, 3)
INSERT [dbo].[Apartments] ([apartment_id], [building_id], [apartment_number], [area], [resident_count], [client_id]) VALUES (4, 3, N'12', CAST(80.00 AS Decimal(10, 2)), 4, 1)
INSERT [dbo].[Apartments] ([apartment_id], [building_id], [apartment_number], [area], [resident_count], [client_id]) VALUES (5, 4, N'8', CAST(55.70 AS Decimal(10, 2)), 2, 2)
SET IDENTITY_INSERT [dbo].[Apartments] OFF
GO
SET IDENTITY_INSERT [dbo].[AuditLog] ON 

INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (1, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-23T13:58:23.730' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (2, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-23T13:58:29.253' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (3, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-23T13:58:29.573' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (4, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-23T14:01:20.177' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (5, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-23T14:01:27.913' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (6, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-23T14:01:28.210' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (7, 2, N'LOGIN', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-23T14:01:47.447' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (8, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-23T14:01:52.570' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (9, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-23T14:01:52.907' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (10, 3, N'LOGIN', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-23T14:01:58.993' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (11, 3, N'LOGOUT', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:01.513' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (12, 3, N'LOGOUT', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:01.853' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (13, 4, N'LOGIN', N'SystemUsers', 4, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:08.990' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (14, 4, N'LOGOUT', N'SystemUsers', 4, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:12.153' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (15, 4, N'LOGOUT', N'SystemUsers', 4, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:12.480' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (16, 5, N'LOGIN', N'SystemUsers', 5, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:18.220' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (17, 5, N'LOGOUT', N'SystemUsers', 5, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:22.840' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (18, 5, N'LOGOUT', N'SystemUsers', 5, NULL, NULL, NULL, CAST(N'2025-05-23T14:02:23.150' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (19, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T13:29:45.973' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (20, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T13:34:39.400' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (21, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T13:53:27.327' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (22, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T13:58:49.600' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (23, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T13:59:51.110' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (24, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T13:59:51.480' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (25, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:34:19.420' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (26, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:34:39.443' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (27, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:36:25.343' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (28, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:37:05.377' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (29, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:42:02.680' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (30, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:43:26.177' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (31, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:45:01.260' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (32, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:45:13.357' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (33, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:51:30.103' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (34, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:52:03.820' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (35, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:52:59.183' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (36, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:53:38.583' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (37, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T14:53:39.027' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (38, 2, N'LOGIN', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T14:54:01.280' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (39, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T14:54:11.337' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (40, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T14:54:11.703' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (41, 3, N'LOGIN', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-24T14:54:23.833' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (42, 3, N'LOGOUT', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-24T14:54:28.173' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (43, 3, N'LOGOUT', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-24T14:54:28.597' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (44, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:08:30.067' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (45, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:16:13.287' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (46, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:17:07.963' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (47, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:30:04.230' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (48, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:32:00.467' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (49, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:32:00.810' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (50, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:32:30.663' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (51, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T15:55:57.313' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (52, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:22:49.437' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (53, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:26:12.947' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (54, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:40:32.990' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (55, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:40:53.300' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (56, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:52:41.103' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (57, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:53:29.903' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (58, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:55:00.963' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (59, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:55:16.567' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (60, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T16:56:47.417' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (61, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:01:45.790' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (62, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:07:31.967' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (63, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:07:58.070' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (64, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:08:59.780' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (65, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:10:30.240' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (66, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:10:56.600' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (67, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:15:09.610' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (68, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:15:33.507' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (69, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:17:16.747' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (70, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:17:46.577' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (71, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:22:28.867' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (72, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:22:46.147' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (73, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:26:04.360' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (74, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:26:58.767' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (75, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:27:37.890' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (76, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:27:48.987' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (77, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:54:14.970' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (78, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:56:08.737' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (79, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T17:56:47.760' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (80, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:18:37.683' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (81, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:18:47.607' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (82, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:20:09.470' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (83, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:22:00.377' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (84, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:23:11.480' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (85, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:39:04.020' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (86, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:49:37.137' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (87, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:52:19.097' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (88, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T18:53:17.173' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (89, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:01:23.157' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (90, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:01:52.427' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (91, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:02:20.917' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (92, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:02:26.937' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (93, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:03:32.667' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (94, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:03:51.407' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (95, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:07:17.280' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (96, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:08:22.620' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (97, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:08:29.987' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (98, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:08:33.693' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (99, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:08:36.477' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (100, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:08:39.273' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (101, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:08:45.500' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (102, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:10:03.690' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (103, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:10:20.267' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (104, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:10:26.887' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (105, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:10:32.537' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (106, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:10:36.177' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (107, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:10:38.703' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (108, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:10:53.623' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (109, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:14:16.283' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (110, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:14:32.097' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (111, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:14:32.490' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (112, 2, N'LOGIN', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T19:14:42.540' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (113, 2, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:14:51.577' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (114, 2, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:14:57.077' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (115, 2, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:15:00.210' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (116, 2, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:15:02.420' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (117, 2, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-24T19:15:04.407' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (118, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T19:15:33.603' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (119, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T19:15:34.063' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (120, 3, N'LOGIN', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-24T19:15:41.203' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (121, 3, N'LOGOUT', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-24T19:16:03.457' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (122, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:26:25.160' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (123, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:28:50.277' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (124, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T19:29:35.810' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (125, 2, N'LOGIN', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T19:59:48.023' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (126, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T19:59:56.730' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (127, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-24T19:59:57.150' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (128, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T20:00:04.893' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (129, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T20:00:24.100' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (130, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T20:00:24.503' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (131, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T21:01:22.790' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (132, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T21:07:22.747' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (133, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-24T21:12:36.447' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (134, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-25T10:15:54.980' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (135, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-25T10:16:23.537' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (136, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-25T10:16:30.087' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (137, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-25T10:17:47.713' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (138, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-26T09:24:33.213' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (139, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-26T09:26:06.330' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (140, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-26T09:26:06.420' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (141, 2, N'LOGIN', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-26T09:26:14.720' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (142, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-26T09:27:01.090' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (143, 2, N'LOGOUT', N'SystemUsers', 2, NULL, NULL, NULL, CAST(N'2025-05-26T09:27:01.140' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (144, 3, N'LOGIN', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-26T09:27:04.243' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (145, 3, N'LOGOUT', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-26T09:28:22.420' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (146, 3, N'LOGOUT', N'SystemUsers', 3, NULL, NULL, NULL, CAST(N'2025-05-26T09:28:22.520' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (147, 4, N'LOGIN', N'SystemUsers', 4, NULL, NULL, NULL, CAST(N'2025-05-26T09:28:28.187' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (148, 4, N'LOGOUT', N'SystemUsers', 4, NULL, NULL, NULL, CAST(N'2025-05-26T09:29:14.317' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (149, 4, N'LOGOUT', N'SystemUsers', 4, NULL, NULL, NULL, CAST(N'2025-05-26T09:29:14.367' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (150, 5, N'LOGIN', N'SystemUsers', 5, NULL, NULL, NULL, CAST(N'2025-05-26T09:29:16.823' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (151, 5, N'LOGOUT', N'SystemUsers', 5, NULL, NULL, NULL, CAST(N'2025-05-26T09:30:28.823' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (152, 5, N'LOGOUT', N'SystemUsers', 5, NULL, NULL, NULL, CAST(N'2025-05-26T09:30:28.873' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (153, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-26T09:30:37.677' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (154, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-26T09:39:47.023' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (155, 1, N'ACCESS', N'Reports', 0, NULL, NULL, NULL, CAST(N'2025-05-26T09:39:50.790' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (156, 1, N'LOGOUT', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-26T09:40:58.590' AS DateTime), NULL, NULL)
INSERT [dbo].[AuditLog] ([log_id], [user_id], [action_type], [table_name], [record_id], [field_name], [old_value], [new_value], [action_date], [ip_address], [user_agent]) VALUES (157, 1, N'LOGIN', N'SystemUsers', 1, NULL, NULL, NULL, CAST(N'2025-05-26T09:41:36.133' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[AuditLog] OFF
GO
SET IDENTITY_INSERT [dbo].[Buildings] ON 

INSERT [dbo].[Buildings] ([building_id], [street_id], [house_number], [building_letter], [apartments_count]) VALUES (1, 1, N'1', NULL, 20)
INSERT [dbo].[Buildings] ([building_id], [street_id], [house_number], [building_letter], [apartments_count]) VALUES (2, 1, N'3', N'A', 15)
INSERT [dbo].[Buildings] ([building_id], [street_id], [house_number], [building_letter], [apartments_count]) VALUES (3, 2, N'10', NULL, 30)
INSERT [dbo].[Buildings] ([building_id], [street_id], [house_number], [building_letter], [apartments_count]) VALUES (4, 3, N'5', NULL, 25)
INSERT [dbo].[Buildings] ([building_id], [street_id], [house_number], [building_letter], [apartments_count]) VALUES (5, 4, N'2', N'B', 18)
SET IDENTITY_INSERT [dbo].[Buildings] OFF
GO
SET IDENTITY_INSERT [dbo].[Clients] ON 

INSERT [dbo].[Clients] ([client_id], [first_name], [last_name], [middle_name], [phone], [email], [passport_number], [passport_issued_by], [registration_date], [is_active], [created_by]) VALUES (1, N'Ольга', N'Сидорова', N'Петровна', N'777-111-2233', N'olga.sidorova@example.com', N'1234567890', N'УВД', CAST(N'2025-05-22' AS Date), 1, 2)
INSERT [dbo].[Clients] ([client_id], [first_name], [last_name], [middle_name], [phone], [email], [passport_number], [passport_issued_by], [registration_date], [is_active], [created_by]) VALUES (2, N'Дмитрий', N'Васильев', N'Иванович', N'777-333-4455', N'dmitriy.vasiliev@example.com', N'0987654321', N'ОВД', CAST(N'2025-05-22' AS Date), 1, 2)
INSERT [dbo].[Clients] ([client_id], [first_name], [last_name], [middle_name], [phone], [email], [passport_number], [passport_issued_by], [registration_date], [is_active], [created_by]) VALUES (3, N'Елена', N'Михайлова', N'Сергеевна', N'777-555-6677', N'elena.mikhailova@example.com', N'9876123450', N'УФМС', CAST(N'2025-05-22' AS Date), 1, 2)
SET IDENTITY_INSERT [dbo].[Clients] OFF
GO
SET IDENTITY_INSERT [dbo].[GasMeters] ON 

INSERT [dbo].[GasMeters] ([meter_id], [apartment_id], [meter_type_id], [serial_number], [installation_date], [initial_reading], [last_verification_date], [next_verification_date], [is_active]) VALUES (1, 1, 1, N'SN12345', CAST(N'2020-01-15' AS Date), CAST(0.000 AS Decimal(12, 3)), CAST(N'2020-01-15' AS Date), CAST(N'2030-01-15' AS Date), 1)
INSERT [dbo].[GasMeters] ([meter_id], [apartment_id], [meter_type_id], [serial_number], [installation_date], [initial_reading], [last_verification_date], [next_verification_date], [is_active]) VALUES (2, 2, 2, N'SN67890', CAST(N'2018-05-20' AS Date), CAST(0.000 AS Decimal(12, 3)), CAST(N'2018-05-20' AS Date), CAST(N'2026-05-20' AS Date), 1)
INSERT [dbo].[GasMeters] ([meter_id], [apartment_id], [meter_type_id], [serial_number], [installation_date], [initial_reading], [last_verification_date], [next_verification_date], [is_active]) VALUES (3, 3, 1, N'SN11223', CAST(N'2021-03-10' AS Date), CAST(0.000 AS Decimal(12, 3)), CAST(N'2021-03-10' AS Date), CAST(N'2031-03-10' AS Date), 1)
SET IDENTITY_INSERT [dbo].[GasMeters] OFF
GO
SET IDENTITY_INSERT [dbo].[Invoices] ON 

INSERT [dbo].[Invoices] ([invoice_id], [apartment_id], [period], [tariff_id], [consumption], [amount], [issue_date], [due_date], [status], [created_by]) VALUES (1, 1, CAST(N'2024-01-01' AS Date), 1, CAST(150.000 AS Decimal(12, 3)), CAST(750.00 AS Decimal(12, 2)), CAST(N'2025-05-22' AS Date), CAST(N'2025-06-21' AS Date), N'Не оплачено', 4)
INSERT [dbo].[Invoices] ([invoice_id], [apartment_id], [period], [tariff_id], [consumption], [amount], [issue_date], [due_date], [status], [created_by]) VALUES (2, 2, CAST(N'2024-01-01' AS Date), 2, CAST(200.000 AS Decimal(12, 3)), CAST(900.00 AS Decimal(12, 2)), CAST(N'2025-05-22' AS Date), CAST(N'2025-06-21' AS Date), N'Не оплачено', 4)
INSERT [dbo].[Invoices] ([invoice_id], [apartment_id], [period], [tariff_id], [consumption], [amount], [issue_date], [due_date], [status], [created_by]) VALUES (3, 3, CAST(N'2024-01-01' AS Date), 1, CAST(100.000 AS Decimal(12, 3)), CAST(500.00 AS Decimal(12, 2)), CAST(N'2025-05-22' AS Date), CAST(N'2025-06-21' AS Date), N'Оплачено', 4)
SET IDENTITY_INSERT [dbo].[Invoices] OFF
GO
SET IDENTITY_INSERT [dbo].[MeterReadings] ON 

INSERT [dbo].[MeterReadings] ([reading_id], [meter_id], [reading_date], [current_reading], [consumption], [is_verified], [verification_date], [verified_by]) VALUES (1, 1, CAST(N'2024-01-31' AS Date), CAST(150.000 AS Decimal(12, 3)), CAST(150.000 AS Decimal(12, 3)), 1, CAST(N'2024-02-05' AS Date), 3)
INSERT [dbo].[MeterReadings] ([reading_id], [meter_id], [reading_date], [current_reading], [consumption], [is_verified], [verification_date], [verified_by]) VALUES (2, 2, CAST(N'2024-01-31' AS Date), CAST(200.000 AS Decimal(12, 3)), CAST(200.000 AS Decimal(12, 3)), 1, CAST(N'2024-02-05' AS Date), 3)
INSERT [dbo].[MeterReadings] ([reading_id], [meter_id], [reading_date], [current_reading], [consumption], [is_verified], [verification_date], [verified_by]) VALUES (3, 3, CAST(N'2024-01-31' AS Date), CAST(100.000 AS Decimal(12, 3)), CAST(100.000 AS Decimal(12, 3)), 1, CAST(N'2024-02-05' AS Date), 3)
SET IDENTITY_INSERT [dbo].[MeterReadings] OFF
GO
SET IDENTITY_INSERT [dbo].[MeterTypes] ON 

INSERT [dbo].[MeterTypes] ([meter_type_id], [type_name], [manufacturer], [verification_period]) VALUES (1, N'Газовый счетчик G4', N'Sagemcom', 10)
INSERT [dbo].[MeterTypes] ([meter_type_id], [type_name], [manufacturer], [verification_period]) VALUES (2, N'Газовый счетчик BK-G4', N'Elster', 8)
SET IDENTITY_INSERT [dbo].[MeterTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Payments] ON 

INSERT [dbo].[Payments] ([payment_id], [invoice_id], [payment_date], [amount], [payment_method], [receipt_number], [operator_id]) VALUES (1, 3, CAST(N'2024-10-02T00:00:00.000' AS DateTime), CAST(500.00 AS Decimal(12, 2)), N'Банковская карта', N'R123', 5)
SET IDENTITY_INSERT [dbo].[Payments] OFF
GO
SET IDENTITY_INSERT [dbo].[Regions] ON 

INSERT [dbo].[Regions] ([region_id], [region_name], [description]) VALUES (1, N'Центральный', N'Центральный район города')
INSERT [dbo].[Regions] ([region_id], [region_name], [description]) VALUES (2, N'Северный', N'Северный район города')
INSERT [dbo].[Regions] ([region_id], [region_name], [description]) VALUES (3, N'Южный', N'Южный район города')
SET IDENTITY_INSERT [dbo].[Regions] OFF
GO
SET IDENTITY_INSERT [dbo].[Streets] ON 

INSERT [dbo].[Streets] ([street_id], [region_id], [street_name]) VALUES (1, 1, N'Ленина')
INSERT [dbo].[Streets] ([street_id], [region_id], [street_name]) VALUES (2, 1, N'Гагарина')
INSERT [dbo].[Streets] ([street_id], [region_id], [street_name]) VALUES (3, 2, N'Советская')
INSERT [dbo].[Streets] ([street_id], [region_id], [street_name]) VALUES (4, 3, N'Кирова')
SET IDENTITY_INSERT [dbo].[Streets] OFF
GO
SET IDENTITY_INSERT [dbo].[SystemUsers] ON 

INSERT [dbo].[SystemUsers] ([user_id], [username], [password_hash], [first_name], [last_name], [email], [phone], [role], [is_active], [last_login], [created_at], [modified_at], [department]) VALUES (1, N'admin', N'admin', N'Иван', N'Иванов', N'ivan.ivanov@example.com', N'123-456-7890', N'Администратор', 1, CAST(N'2025-05-26T09:41:36.123' AS DateTime), CAST(N'2025-05-22T14:15:49.817' AS DateTime), NULL, N'Администрация')
INSERT [dbo].[SystemUsers] ([user_id], [username], [password_hash], [first_name], [last_name], [email], [phone], [role], [is_active], [last_login], [created_at], [modified_at], [department]) VALUES (2, N'manager', N'manager', N'Петр', N'Петров', N'petr.petrov@example.com', N'987-654-3210', N'Менеджер', 1, CAST(N'2025-05-26T09:26:14.717' AS DateTime), CAST(N'2025-05-22T14:15:49.817' AS DateTime), NULL, N'Отдел обслуживания')
INSERT [dbo].[SystemUsers] ([user_id], [username], [password_hash], [first_name], [last_name], [email], [phone], [role], [is_active], [last_login], [created_at], [modified_at], [department]) VALUES (3, N'technician', N'technician', N'Сергей', N'Сергеев', N'sergey.sergeev@example.com', N'555-123-4567', N'Техник', 1, CAST(N'2025-05-26T09:27:04.243' AS DateTime), CAST(N'2025-05-22T14:15:49.817' AS DateTime), NULL, N'Технический отдел')
INSERT [dbo].[SystemUsers] ([user_id], [username], [password_hash], [first_name], [last_name], [email], [phone], [role], [is_active], [last_login], [created_at], [modified_at], [department]) VALUES (4, N'accountant', N'accountant', N'Мария', N'Смирнова', N'maria.smirnova@example.com', N'555-987-6543', N'Бухгалтер', 1, CAST(N'2025-05-26T09:28:28.160' AS DateTime), CAST(N'2025-05-22T14:15:49.817' AS DateTime), NULL, N'Бухгалтерия')
INSERT [dbo].[SystemUsers] ([user_id], [username], [password_hash], [first_name], [last_name], [email], [phone], [role], [is_active], [last_login], [created_at], [modified_at], [department]) VALUES (5, N'operator', N'operator', N'Алексей', N'Кузнецов', N'alexey.kuznetsov@example.com', N'555-111-2222', N'Оператор', 1, CAST(N'2025-05-26T09:29:16.823' AS DateTime), CAST(N'2025-05-22T14:15:49.817' AS DateTime), NULL, N'Отдел продаж')
SET IDENTITY_INSERT [dbo].[SystemUsers] OFF
GO
SET IDENTITY_INSERT [dbo].[Tariffs] ON 

INSERT [dbo].[Tariffs] ([tariff_id], [tariff_name], [rate], [start_date], [end_date], [description]) VALUES (1, N'Тариф обычный', CAST(5.0000 AS Decimal(10, 4)), CAST(N'2023-01-01' AS Date), NULL, N'Обычный тариф для населения')
INSERT [dbo].[Tariffs] ([tariff_id], [tariff_name], [rate], [start_date], [end_date], [description]) VALUES (2, N'Тариф льготный', CAST(4.5000 AS Decimal(10, 4)), CAST(N'2023-01-01' AS Date), NULL, N'Льготный тариф для определенных категорий граждан')
SET IDENTITY_INSERT [dbo].[Tariffs] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__GasMeter__BED14FEE91A77BAD]    Script Date: 26.05.2025 11:54:47 ******/
ALTER TABLE [dbo].[GasMeters] ADD UNIQUE NONCLUSTERED 
(
	[serial_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__SystemUs__F3DBC572F4D34F96]    Script Date: 26.05.2025 11:54:47 ******/
ALTER TABLE [dbo].[SystemUsers] ADD UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
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
