# Application of Cloud Application Development in Smart Grids
This project was developed as part of the course Cloud Application Development in Smart Grids.
The application is a simplified clone of Stack Overflow, demonstrating the use of Microsoft Azure services such as Table Storage, Blob Storage, and Queue Storage, all integrated through the local Azure Storage Emulator.

The goal of this project is to gain hands-on experience with cloud-based data storage, message processing, and scalable application design by replicating key features of a modern community-driven Q&A platform.

---

![StackOverflow!](/screenshots/StackOverflow.png)

![HealthStatus!](/screenshots/HealthStatus.png)

## Installation:

1. **Install .NET SDK**:  
   Download and install the .NET SDK 8.0 for your operating system from [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

2. **Verify installation**:  
   Run the following command in your terminal to confirm:
   ```bash
   dotnet --list-sdks
   ```
3. **Install Node.js and npm**

   Download and install the **[Node.js LTS version](https://nodejs.org/en/download/)** — it comes with **npm** (Node Package Manager) preinstalled.

   Verify the installation by running the following commands in your terminal:

   ```bash
   node -v
   npm -v
   ```
   

## How to run:

1. **Clone the project**:
   ```bash
   git clone https://github.com/Acile067/Project_RCA_Stack_Overlow.git
   cd Project_RCA_Stack_Overlow
   ```

   Add `.env` in `backend/NotificationService` example:
   ```bash
   MAIL_USERNAME=aleksandarsasastefanjovana@gmail.com
   MAIL_PASSWORD=upzg sndq xnhh qyks
   MAIL_SERVER=smtp.gmail.com
   MAIL_PORT=587
   MAIL_USE_TLS=true
   MAIL_USE_SSL=false
   ```

3. **Dotnet CLI API**:
   
    ```bash
   cd backend/backend
   dotnet run
   ```

4. **Run UI**:
   
    ```bash
   health-status-ui
   npm i
   npm run dev
   ```

   ```bash
   stack-overflow-ui
   npm i
   npm run dev
   ```
    
