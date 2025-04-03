# Eventology API

This repository contains the **REST API** for **Eventology**, the cultural event management platform. The API is built using **C# ASP.NET Core** and serves as the communication bridge between the desktop (Windows Forms) and mobile applications.

## Features

- **Authentication & Authorization**: Secure login and role-based access control.
- **User Management**: Register and manage superadmins, organizers, and users.
- **Event Scheduling**: Create, update, and retrieve event data.
- **Venue Management**: Manage venue availability and prevent scheduling conflicts.
- **Ticketing**: Handle reservations and seating assignments.
- **Messaging System**: Support real-time chat functionality.
- **Incident Reporting**: Connect with Odoo for managing support tickets.

## Technologies Used

- **ASP.NET Core**: Framework for building the API.
- **Entity Framework Core**: ORM for database operations.
- **SQL Server**: Relational database used for storage.
- **JWT**: Authentication with JSON Web Tokens.

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Eventology/api.git
   ```
2. Open the solution in **Visual Studio**.
3. Configure your `appsettings.json` for database connection and JWT settings.
4. Run the application using IIS Express or Kestrel.

## Usage

This API is consumed by:
- The **Windows Forms** desktop application (administrators).
- The **Android** mobile application (users and organizers).

## Contributing

Contributions are welcome! Submit issues or open pull requests to improve functionality or fix bugs.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, contact: 148581386+rwxce@users.noreply.github.com
