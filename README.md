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
