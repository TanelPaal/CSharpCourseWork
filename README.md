# Tic-Tac-Two Game Project

**Student:** Tanel Paal  
**Uni-ID:** tanpaa  
**Email:** tanpaa@taltech.ee  
**Student Code:** 222775IADB  
**Course:** C# Programming (3rd Semester)

## Project Overview
This project implements a variant of Tic-Tac-Toe with additional gameplay mechanics, developed as part of the C# course in the third semester. The solution includes both a console application and a web application with real-time multiplayer capabilities.

### Key Features
- Extended Tic-Tac-Toe gameplay with movable game area
- Multiple board size configurations
- Save/Load game functionality
- AI opponent with random move generation
- Database and JSON file storage support
- Real-time multiplayer web interface using SignalR

## Implementation Details

### Console Application
- Menu-driven interface with configuration options
- Game state persistence using both DB and JSON storage
- AI opponent implementation
- Custom game configuration creation

### Web Application
- Real-time multiplayer using SignalR (beyond course requirements)
- Asynchronous game state updates
- User session management
- Interactive game board interface
- AI opponent support in web interface

### Data Access Layer
- Dual storage implementation (Database/JSON)
- Repository pattern implementation
- Entity Framework Core integration

## Future Improvements
1. Optimize nested code structures for better performance
2. Complete JSON file handling in web application
3. Remove obsolete code and refactor for cleaner architecture
4. Enhance error handling and user feedback

## Technical Highlights
- SignalR implementation for real-time updates
- Repository pattern with interchangeable storage solutions
- Async/await pattern in web application
- Clean separation of concerns between layers

## Learning Outcomes
- Advanced C# programming concepts
- ASP.NET Core development
- Real-time web application development
- Database and file system integration
- Software architecture patterns

## Notes
Some features like JSON file handling in the web application remain to be completed. The project demonstrates both required course elements and additional self-implemented features like SignalR integration.
