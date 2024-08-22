# Investment Management System

## Introduction

The Investment Management System is a comprehensive application designed for managing mutual funds. It leverages the robust capabilities of the .NET framework to provide users with a secure and intuitive platform for investment management. Key features include secure user authentication and registration, browsing and selecting mutual funds, managing Systematic Investment Plans (SIPs), a user-centric dashboard, educational resources, a budget goal calculator, and more.

## Technologies Used

- **C#**: Programming language used for application development.
- **.NET Framework**: Framework used for building the application.
- **SQL Local Database**: Database used for storing user and investment data.

## Key Features

1. **User Registration**
   - **Description**: Allows users to create accounts to access the investment platform.
   - **Requirements**:
     - Collect user information such as name, email, phone number, etc.
     - Validate email addresses and ensure uniqueness.

2. **User Authentication**
   - **Description**: Authenticates users to ensure secure access to their accounts.
   - **Requirements**:
     - Username and password authentication.

3. **Making Investments**
   - **Description**: Enables users to invest funds based on their preferences.
   - **Requirements**:
     - Provide options to choose investment types (e.g., mutual funds).
     - Display details for each investment option (risk level, expected return).
     - Allow users to specify investment amount and duration.
     - Present cancellation policies clearly before finalizing investments.

4. **Modify Investment Details**
   - **Description**: Allows users to update their investment preferences.
   - **Requirements**:
     - Enable users to modify investment amounts.
     - Provide options to change investment duration.

5. **Cancel Investments**
   - **Description**: Allows users to cancel their investments as per defined policies.
   - **Requirements**:
     - Display cancellation policies clearly to users before confirming.
     - Process refunds if applicable according to cancellation terms.

6. **Redeeming after Cancellation**
   - **Description**: Defines procedures for users to redeem funds after canceling investments.
   - **Requirements**:
     - Specify any waiting periods or penalties for redemption.
     - Ensure funds are transferred securely back to the user’s account.

7. **Sending Timely Notifications/Alerts**
   - **Description**: Keeps users informed about their investments and accounting activities.
   - **Requirements**:
     - Provide alerts for upcoming investment maturity or important policy changes.

8. **Closure of the Investment**
   - **Description**: Facilitates the closure of investments upon maturity or at user request.
   - **Requirements**:
     - Automatically close investments upon reaching maturity, following predefined rules.

9. **Customer Support**
   - **Description**: Provides support to users regarding their accounts and investments.
   - **Requirements**:
     - Provide contact information for customer service (email, phone).

10. **Displaying Total Investments**
    - **Description**: Presents users with an overview of their investment portfolio.
    - **Requirements**:
      - Display the total invested amount.

11. **Profile Management**
    - **Description**: Allows users to update their personal information.
    - **Requirements**:
      - Enable users to modify email addresses and phone numbers securely.

## Usage

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/investment-management-system.git
   cd investment-management-system
   
2. Build and run the application using your preferred .NET development environment.

3. Follow the in-app instructions to register, authenticate, and manage investments.
