# Cloud-Based Image Sharing Application with Azure
This project implements a cloud-based image sharing application built using C# and .NET, 
with image storage and retrieval powered by Microsoft Azure. The application demonstrates 
how cloud storage services can be integrated into a backend system to enable scalable 
media upload, access, and management.
## Project Overview
This application allows users to upload, store, and access images through a cloud-backed backend system.
Images are handled by the application layer and stored securely in cloud storage, enabling scalability
and remote access.

The project focuses on demonstrating:
- Integration between a .NET backend and cloud storage services
- Secure and efficient image upload and retrieval
- Separation of application logic and cloud infrastructure
- A simple, extensible architecture suitable for real-world deployment
## Architecture & Cloud Components

The application follows a client–server architecture with cloud-backed storage:

- **Application Layer (C# / .NET)**
  - Handles image uploads, retrieval, and user interactions
  - Implements role-based access logic for admins, approvers, and users

- **Cloud Storage Layer (Azure)**
  - Stores uploaded images securely
  - Enables scalable and durable cloud storage

This separation allows the application to scale independently of storage
and supports future enhancements such as moderation workflows and analytics.
## Key Features & User Roles

### Role-Based Access Control
The application supports multiple user roles to manage access and content securely:

- **Admin**
  - Manages user accounts and roles
  - Can add, modify, or remove users
  - Oversees system-level configurations

- **Approver**
  - Reviews uploaded images
  - Approves or rejects images before they become accessible
  - Ensures content quality and compliance

- **Standard User**
  - Uploads images to the platform
  - Views approved images based on assigned permissions

---

### Content Moderation & Usage Tracking
- Images follow an approval workflow before being published
- User-specific image views are tracked
- View counts provide basic usage analytics and engagement insights

These features demonstrate how access control, moderation workflows, and simple analytics
can be integrated into a cloud-based application.
## Tech Stack
- C#
- .NET
- ASP.NET (if applicable)
- Microsoft Azure (Cloud Storage)
- Visual Studio
- ## How to Run
1. Clone the repository
2. Open the solution file (`.sln`) in Visual Studio
3. Restore NuGet packages
4. Configure Azure storage connection settings
5. Build and run the application locally

