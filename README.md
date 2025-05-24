# AzCommunicationFunction

# 📄 License

This project is licensed under a **Proprietary License**.  
By default, usage, distribution, or modification is **not permitted** without prior written permission.

The license is available in **multiple languages** (English, Vietnamese, Korean, Chinese, Russian).  
Please see the full license file here: [./LICENSE](https://github.com/ngtduc693/AzCommunicationFunction/blob/main/LICENSE)

For permission requests, contact: **ngtduc693@gmail.com**

# Description
This project belongs to **ngtduc693@gmail.com**, and only I hold full rights to use it.
It is an Azure Function designed to process messages from Azure Service Bus, with support for: Sending emails, Sending notifications, Logging communications to CosmosDB.

The system is built with dynamic configuration through Azure Key Vault and supports flexible template storage, including: In-memory, File-based, Database-backed

# System Architecture
The following diagram illustrates the core system architecture with actual code entities and Azure service integrations

![image](https://github.com/user-attachments/assets/75c31b87-71f2-48be-b8a6-1ccb18a27e1c)

## Configuration Architecture
The system uses a multi-layered configuration approach supporting development and production scenarios

![image](https://github.com/user-attachments/assets/37870a7a-d5b8-477b-b364-848fef1e1e6f)

## Communication Processing Flow
The following diagram shows how messages flow through the system using specific command and handler classes

![image](https://github.com/user-attachments/assets/2a731331-4f12-4b25-b729-d47a6fe4ca58)

## Key Azure Service Integrations
The system integrates with several Azure services to provide comprehensive communication capabilities:

- **Azure Service Bus**: Message queue (`communication-queue`) that triggers function execution
- **Azure Key Vault**: Secure storage for connection strings and sensitive configuration
- **Azure Communication Services**: Email delivery service with sender configuration
- **Azure Notification Hubs**: Push notification delivery to mobile devices and web clients
- **CosmosDB**: Document database for storing communication logs and optionally templates
The health check endpoint at `/api/health` provides monitoring capabilities to verify system status and Azure service connectivity.

## Usage Guide
### 1. Configuration
- Use the `local.settings.json` file to configure environment variables for local development.
- Sensitive information (connection strings, secrets) should be stored in Azure KeyVault.
- Main configuration parameters can be set in `appsettings.json` or via Azure App Configuration:

```json
{
  "KeyVault": {
    "VaultUrl": "<url>",
  },
  "ServiceBus": {
    "QueueName": "communication-queue",
    "ConnectionStringName": "ServiceBusConnection"
  },
  "CosmosDb": {
    "DatabaseName": "CommunicationDb",
    "ContainerName": "Logs",
    "PartitionKeyPath": "/PartitionKey"
  },
  "Email": {
    "SenderAddress": "noreply@yourdomain.com",
    "SenderName": "Communication Service",
    "RetryCount": 3,
    "TimeoutSeconds": 30
  },
  "Notification": {
    "HubName": "communication-hub",
    "RetryCount": 3,
    "TimeoutSeconds": 30
  },
  "Templates": {
    "TemplateStorageType": "InMemory",
    "TemplateBasePath": "Templates",
    "CacheTemplates": true,
    "CacheExpirationMinutes": 60
  }
}
```

### 2. Build & Run locally
```powershell
cd CommunicationFunction
# Build
 dotnet build
# Run local function
 func start
```

### 3. Deploy to Azure
- Use the publish command:
```powershell
cd CommunicationFunction
 dotnet publish --configuration Release
```
- Deploy the artifact to Azure Function App.

## Health Check
- Access the `/api/health` endpoint to check the function's health status.

## Notes
- Ensure all secrets and connection strings are correctly configured in KeyVault or local.settings.json.
- You can extend the system with more message types by adding corresponding handlers.
