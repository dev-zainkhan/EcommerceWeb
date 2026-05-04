# EcommerceWeb - Setup Guide

## Requirements
- Visual Studio 2022 or later
- .NET 8 SDK or later
- Internet connection (for NuGet packages)
- AWS RDS SQL Server database
- AWS S3 bucket

## First Time Setup

### Step 1 - Install .NET 8 SDK
Download from:
https://dotnet.microsoft.com/download/dotnet/8.0

### Step 2 - Open Project
Open `EcommerceWeb.sln` in Visual Studio

### Step 3 - Restore NuGet Packages
Either:
- Visual Studio will restore automatically on build
OR
- Right-click Solution → Restore NuGet Packages
OR
- Run in terminal:
  dotnet restore

### Step 4 - Update Connection String
Open `DAL/DbHelper.cs` and update:

Server=YOUR_RDS_ENDPOINT,1433;
Database=EcommerceDB;
User Id=YOUR_USERNAME;
Password=YOUR_PASSWORD;
TrustServerCertificate=True;

### Step 5 - Update AWS S3 Keys
Open `DAL/S3Helper.cs` and update:
_accessKey = "YOUR_ACCESS_KEY_ID";
_secretKey = "YOUR_SECRET_ACCESS_KEY";
_bucketName = "YOUR_BUCKET_NAME";

### Step 6 - Run Database Scripts
Run all SQL scripts from `Database/setup.sql`
in your AWS RDS database

### Step 7 - Run Project
Press F5 in Visual Studio

## NuGet Packages Used
- Microsoft.Data.SqlClient
- BCrypt.Net-Next  
- AWSSDK.S3

## Project Structure

EcommerceWeb/
├── Models/          → Data classes
│   ├── User/
│   ├── Product/
│   ├── Order/
│   ├── Cart/
│   └── Shipping/
├── DAL/             → Database access
│   ├── User/
│   ├── Product/
│   ├── Order/
│   ├── Cart/
│   ├── Shipping/
│   └── Profile/
├── BLL/             → Business logic
│   ├── Auth/
│   ├── Product/
│   ├── Order/
│   └── Cart/
├── Controllers/     → Handle requests
├── Views/           → HTML pages
└── wwwroot/         → Static files

## Default Roles
- Buyer  → Browse and purchase products
- Seller → List and manage products
- Admin  → Manage everything

## Features
- Login / Register with BCrypt password hashing
- Seller product listing with min 3 images
- Image slider on product cards
- AWS S3 image storage
- Add to cart with stock validation
- Sticky bottom cart bar
- Checkout with shipping address
- Order placement with ACID transactions
- User profile with picture upload
- Async/await on all database operations

## Notes
- All images stored on AWS S3
- Database hosted on AWS RDS SQL Server
- Passwords are BCrypt hashed
- All DB operations use async/await
- Orders use SQL transactions (ACID)