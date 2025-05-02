using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sample_LINQLogin
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 1. Initialize the DataContext for LINQ to SQL to set up the connection to your database (format: write your dbml filename then add DataContext)
        SampleLINQDataContext db = new SampleLINQDataContext(Properties.Settings.Default.LoginPracticeConnectionString);


        public MainWindow()
        {
            InitializeComponent();

        }

        // 2. Create a method to handle the login button click event
        private void login_btn_Click(object sender, RoutedEventArgs e)
        {
            // 3. Check if the username and password fields are empty then show a message box
            if (user_txt.Text == "" || pass_txt.Text == "")
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            
            else
            {
                // 11. Call the fetching Password and comparing Password methods to check if the password entered in the pass_txt field matches the password fetched from the database
                if (comparingPassword(fetchingPassword())==0)
                {
                    MessageBox.Show("Login Successful", "Welcome Back!", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    // 12. Query the database using LINQ to SQL to get the user role of the logged-in user
                    var users = (from u in db.user_Tables
                                 where u.UserID == user_txt.Text
                                 select u).FirstOrDefault();

                    // 13. Check the user role and open the corresponding home page
                    if (users.UserRole == "Admin")
                    {
                        // 14. Create an instance of the adminHome class and show it
                        // Make sure to add adminHome window to your project
                        adminHome adminHome = new adminHome();
                        adminHome.Show();
                        this.Close();
                    }
                    else if(users.UserRole == "Librarian")
                    {
                        // 15. Create an instance of the librarianHome class and show it
                        // Make sure to add librarianHome window to your project
                        librarianHome librarianHome = new librarianHome();
                        librarianHome.Show();
                        this.Close();
                    }
                    else if (users.UserRole == "Student")
                    {
                        //16. Create an instance of the studentHomepage class and show it
                        // Make sure to add studentHomepage window to your project
                        studentHomepage studentHomepage = new studentHomepage();
                        studentHomepage.Show();
                        this.Close();
                    }

                }

                // 17. If the password entered in the pass_txt field does not match the password fetched from the database, show a message box
                else
                    MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    
            }
        }
        // 4. Create a method to handle the exit button click event
        // This method will fetch the password from the database that matches the User ID entered in the user_txt field
        private string fetchingPassword()
        {
            // 5. Create a variable to hold the fetched password
            string fetchedPassword = "";

            // 6. Use LINQ to SQL to query the database for the password that matches the User ID entered in the user_txt field
            var users = (from u in db.user_Tables
                               where u.UserID == user_txt.Text
                                           select u).FirstOrDefault();

            // 7. Check if the user exists in the database
            if (users == null)
            {              
                return "";
            }

            // 8. If the user exists, assign the password to the uPass variable then return it
            fetchedPassword = users.UserPass;
            return fetchedPassword;
        }

        // 9. Create a method to compare the password entered in the pass_txt field with the password fetched from the database
        private int comparingPassword(string fetchedPassword) 
        {
            // 10. Check if the fetched password is matched with the password entered in the pass_txt field
            if (pass_txt.Text == fetchedPassword)
            {
                return 0;// matched
            }       
            else
            {
                return 1;// not matched
            }
                
        }

        
    }
}
