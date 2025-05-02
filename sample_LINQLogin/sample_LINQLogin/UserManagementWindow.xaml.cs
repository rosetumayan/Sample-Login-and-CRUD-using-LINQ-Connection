using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace sample_LINQLogin
{
    public partial class UserManagementWindow : Window
    {
        // 1. Initialize the DataContext for LINQ to SQL to set up the connection to your database (format: write your dbml filename then add DataContext)
        SampleLINQDataContext db = new SampleLINQDataContext(Properties.Settings.Default.LoginPracticeConnectionString);

        // 2. This variable will hold the selected user for update/delete in your CRUD operations
        private user_Table _selectedUser;
        

        public UserManagementWindow()
        {
            InitializeComponent();
            // 6. Call the LoadUsers method to load the users when the window is initialized
            LoadUsers();
        }


        // 3. Load the users from the database into the DataGrid when the window is loaded
        private void LoadUsers()
        {
            try
            {
                // 4. Clear the DataGrid before loading new data
                //CRUD Operation: Read
                var users = from u in db.user_Tables select u;
                userDataGrid.ItemsSource = users.ToList();
               
                statusTextBlock.Text = "Users loaded successfully.";
            }
            catch (Exception ex)
            {
                // 5. Handle any exceptions that occur during the loading of users
                MessageBox.Show("Error loading users: " + ex.Message);
                statusTextBlock.Text = "Error loading users.";
            }
        }

        // 7. Click your addNewUserButton to open this event handler that implements the logic to add a new user
        //CRUD Operation: Create
        private void addNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            // 8. Get the values from the text boxes and combo box
            string userId = addUserIdTextBox.Text.Trim();
            string password = addPasswordTextBox.Text.Trim(); // Remember to hash this!
            string userName = addUserNameTextBox.Text.Trim();
            string selectedRole = (addRoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // 9. Check if the user ID, password, user name, and role are not empty
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(selectedRole))
            {
                // 10. Check if the user ID already exists in the database
                if (!db.user_Tables.Any(u => u.UserID == userId))
                {
                    // 11. Create a new user object and set its properties
                    var newUser = new user_Table
                    {
                        // 12. Set the properties of the new user object
                        UserID = userId,
                        UserPass = password, 
                        UserName = userName,
                        UserRole = selectedRole 
                    };

                    // 13. Insert the new user object into the database using InsertOnSubmit method
                    db.user_Tables.InsertOnSubmit(newUser);

                    try
                    {
                        // 14. Submit the changes to the database using SubmitChanges method
                        db.SubmitChanges();

                        // 15. Reload the users to reflect the changes in the DataGrid
                        LoadUsers();

                        // 16. Clear the text boxes and combo box after successful addition
                        statusTextBlock.Text = $"User '{userId}' added successfully with Role: '{selectedRole}'.";
                        addUserIdTextBox.Clear();
                        addPasswordTextBox.Clear();
                        addUserNameTextBox.Clear();
                        addRoleComboBox.SelectedIndex = -1; // Clear selection
                    }
                    catch (Exception ex)
                    {
                        // 17. Handle any exceptions that occur during the addition of the user
                        MessageBox.Show("Error adding user: " + ex.Message);
                        statusTextBlock.Text = "Error adding user.";
                    }
                }
                else
                {
                    // 18. Show a message if the user ID already exists
                    MessageBox.Show($"User with ID '{userId}' already exists.");
                    statusTextBlock.Text = "User already exists.";
                }
            }
            else
            {
                // 19. Show a message if any of the fields are empty 
                MessageBox.Show("User ID, password, user name, and role cannot be empty.");
                statusTextBlock.Text = "User ID, password, user name, and role cannot be empty.";
            }
        }

        // 20. Click your DataGrid in the XAML file to trigger this event handler. It is intended to handle the event when the selection in the DataGrid changes.
        
        private void userDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 21. Check if an item is selected in the DataGrid and if it is of type user_Table then declare a variable to hold the selected user [selectedUser]
            if (userDataGrid.SelectedItem != null && userDataGrid.SelectedItem is user_Table selectedUser)
            {
                // 22. Set the selected user to the _selectedUser variable
                _selectedUser = selectedUser;

                // 23. Populate the text boxes with the selected user's information
                updateUserIdTextBox.Text = _selectedUser.UserID;
                updatePasswordTextBox.Text = _selectedUser.UserPass;
                updateUserNameTextBox.Text = _selectedUser.UserName;
                updateRoleComboBox.SelectedItem = _selectedUser.UserRole;

                // 24. Populate the view text boxes with the selected user's information
                viewUserIdTextBox.Text = _selectedUser.UserID;
                viewPasswordTextBox.Text = _selectedUser.UserPass;
                viewUserNameTextBox.Text = _selectedUser.UserName;
                viewRoleTextBox.Text = _selectedUser.UserRole;

                // 25. Enable the save and delete buttons
                saveUserButton.IsEnabled = true;
                deleteSelectedUserButton.IsEnabled = true;
            }

            // 26. If no item is selected, clear the text boxes and disable the buttons
            else
            {
                selectedUser = null;
                updateUserIdTextBox.Clear();
                updatePasswordTextBox.Clear();
                updateUserNameTextBox.Clear();
                updateRoleComboBox.SelectedIndex = -1;
                viewUserIdTextBox.Clear();
                viewPasswordTextBox.Clear();
                viewUserNameTextBox.Clear();
                viewRoleTextBox.Clear();
                saveUserButton.IsEnabled = false;
                deleteSelectedUserButton.IsEnabled = false;
            }
        }

        // 27. Click your saveUserButton to open this event handler that implements the logic to update a user
        //CRUD Operation: Update
        private void saveUserButton_Click(object sender, RoutedEventArgs e)
        {
            // 28. Check if a user is selected for update
            if (_selectedUser != null)
            {
                // 29. Get the values from the text boxes and combo box
                _selectedUser.UserPass = updatePasswordTextBox.Text.Trim(); 
                _selectedUser.UserName = updateUserNameTextBox.Text.Trim();
                string selectedRole = (updateRoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                // 30. Check if the selected role is not empty
                if (!string.IsNullOrEmpty(selectedRole))
                {
                    // 31. Update the selected user's properties
                    _selectedUser.UserRole = selectedRole; // Update the UserRole

                    try
                    {
                        // 32. Submit the changes to the database using SubmitChanges method
                        db.SubmitChanges();

                        // 33. Reload the users to reflect the changes in the DataGrid
                        LoadUsers();

                        // 34. Clear the text boxes and combo box after successful update
                        statusTextBlock.Text = $"User '{_selectedUser.UserID}' updated successfully with Role: '{selectedRole}'.";
                        updateUserIdTextBox.Clear();
                        updatePasswordTextBox.Clear();
                        updateUserNameTextBox.Clear();
                        updateRoleComboBox.SelectedIndex = -1;
                        viewUserIdTextBox.Clear();
                        viewPasswordTextBox.Clear();
                        viewUserNameTextBox.Clear();
                        viewRoleTextBox.Clear();
                        saveUserButton.IsEnabled = false;
                        _selectedUser = null; // Clear the selected user
                    }
                    catch (Exception ex)
                    {
                        // 35. Handle any exceptions that occur during the update of the user
                        MessageBox.Show("Error updating user: " + ex.Message);
                        statusTextBlock.Text = "Error updating user.";
                    }
                }
                else
                {
                    // 36. Show a message if the selected role is empty
                    MessageBox.Show("Please select a role to update.");
                    statusTextBlock.Text = "Please select a role to update.";
                }
            }
            else
            {
                // 37. Show a message if no user is selected for update
                MessageBox.Show("No user selected for update.");
                statusTextBlock.Text = "No user selected for update.";
            }
        }

        // 38. Click your deleteSelectedUserButton to open this event handler that implements the logic to delete a user
        //CRUD Operation: Delete
        private void deleteSelectedUserButton_Click(object sender, RoutedEventArgs e)
        {
            // 39. Check if a user is selected for deletion and if it is of type user_Table, then declare a variable to hold the selected user [selectedUser]
            if (userDataGrid.SelectedItem != null && userDataGrid.SelectedItem is user_Table selectedUser)
            {
                // 39. Set the selected user to the _selectedUser variable
                _selectedUser = selectedUser;

                // 41. Confirm the deletion with the user
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete user '{_selectedUser.UserID}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                // 42. If the user confirms, delete the selected user
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 43. Delete the selected user from the database using DeleteOnSubmit method then submit the changes using SubmitChanges method
                        string deletedUserID = _selectedUser.UserID; // Store the UserID before setting _selectedUser to null

                        db.user_Tables.DeleteOnSubmit(_selectedUser);
                        db.SubmitChanges();

                        //44. Recreate the DataContext to clear its cache
                        db = new SampleLINQDataContext(Properties.Settings.Default.LoginPracticeConnectionString);

                        // 45. Reload the users to reflect the changes in the DataGrid
                        LoadUsers();

                        // 46. Clear the text boxes and combo box after successful deletion
                        statusTextBlock.Text = $"User '{deletedUserID}' deleted successfully."; // Use the stored UserID
                        deleteSelectedUserButton.IsEnabled = false;
                        _selectedUser = null;
                        updateUserIdTextBox.Clear();
                        updatePasswordTextBox.Clear();
                        updateUserNameTextBox.Clear();
                        updateRoleComboBox.SelectedIndex = -1;
                        viewUserIdTextBox.Clear();
                        viewPasswordTextBox.Clear();
                        viewUserNameTextBox.Clear();
                        viewRoleTextBox.Clear();
                        saveUserButton.IsEnabled = false;
                    }
                    catch (Exception ex)
                    {
                        // 47. Handle any exceptions that occur during the deletion of the user 
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        statusTextBlock.Text = "Error deleting user.";
                        
                    }
                }
            }
            else
            {
                // 48. Show a message if no user is selected for deletion
                MessageBox.Show("No user selected for deletion.");
                statusTextBlock.Text = "No user selected for deletion.";
            }
        }

        // 49. Click your viewUserButton to open this event handler that implements the logic to view a user
        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            // 50. Create a new instance of MainWindow and show it, then close the current window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        // 51. Click your backButton to open this event handler that implements the logic to go back to the admin home
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // 52. Create a new instance of adminHome and show it, then close the current window
            adminHome adminHome = new adminHome();
            adminHome.Show();
            this.Close();
        }
    }
}