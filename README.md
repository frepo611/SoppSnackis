# SoppSnackis – Diskussionsforum

SoppSnackis is a modern discussion forum built with Razor Pages (.NET 8), featuring a Swedish user interface and an English REST API. The project is designed for flexibility, security, and extensibility, supporting both basic and advanced forum features.

---

## Features

### Forum & User Features
- **Public Subjects:** Browse and participate in various discussion subjects (e.g., Bilsnack, Båtsnack, Cykelsnack).
- **Threaded Posts:** Create posts and comment in a true tree structure, supporting parallel replies at any level.
- **User Registration & Login:** Anyone can register and participate.
- **Profile Pages:** Each user has a simple info page and can upload a profile picture.
- **Image Uploads:** Attach images to posts and comments.
- **Reactions:** Like or react to posts (e.g., thumbs up, heart).
- **Profanity Filter:** Inappropriate words are automatically censored.
- **Private Messaging:** Send private messages to other users.
- **Group Messaging:** Create groups, invite users, and send group messages (VG).
- **Quoting:** Quote previous posts in replies.
- **Reporting:** Report inappropriate posts for admin review.

### Admin Features
- **Admin Area:** Separate admin dashboard for managing subjects, users, reports, and groups.
- **Subject Management:** Create, edit, and delete forum subjects.
- **User Management:** View, promote/demote, or remove users.
- **Report Handling:** Review and resolve reported posts.
- **Group Management:** Manage groups and group memberships.

---

## Technical Details

- **Frontend:** ASP.NET Core Razor Pages (.NET 8), UI in Swedish.
- **Backend/API:** ASP.NET Core Web API (REST), endpoints and models in English.
- **Database:** MS SQL Server, using Entity Framework Core (Code First).
- **Authentication:** ASP.NET Core Identity with role-based access (User/Admin).
- **File Storage:** Images are stored in the file system, with paths saved in the database.
- **Validation:** Client-side validation with jQuery Validation (MIT License).
- **Architecture:** Separation of concerns between UI, API, and data access.
- **Extensible:** Designed for easy addition of new features (e.g., notifications, real-time updates).

---

## Getting Started

1. **Clone the repository**
2. **Configure the connection string** in `appsettings.json`
3. **Run EF Core migrations**
4. **Start the application**
5. **Access the forum** at `https://localhost:xxxx/`

---

## License

This project uses open source components (e.g., jQuery Validation under MIT License). See individual library folders for details.

---

## Contributing

Pull requests and suggestions are welcome! Please open an issue to discuss your ideas.

      