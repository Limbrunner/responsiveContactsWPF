using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace responsiveContacts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Vars
        readonly DataTable contacts = new DataTable(); // Datasource for DataGrid in Frontend
        bool editing = false; // Keep track of if the user is editing or not

        //private Rect buttonOriginalRect;
        //private Rect orginalWindowRect;

        // Main
        public MainWindow()
        {
            InitializeComponent();
        }

        // Grid data
        public void InitializeContactsGrid()
        {
            // Create columns for grid
            contacts.Columns.Add("Vorname", typeof(string));
            contacts.Columns.Add("Nachname", typeof(string));
            contacts.Columns.Add("Email", typeof(string));
            contacts.Columns.Add("Telefon", typeof(string));

            // Add all columns to Grid
            foreach (DataColumn column in contacts.Columns)
            {
                // Create object that can show and contains column text
                DataGridTextColumn gridColumn = new DataGridTextColumn
                {
                    // Set Header and Binding to column name
                    Header = column.ColumnName,
                    Binding = new Binding(column.ColumnName),

                    // Set width of columns
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star) // relative to Grid size
                };

                // Adding
                contactsDataGrid.Columns.Add(gridColumn);
            }
        }

        // Load
        private void ResponsiveContacts_Loaded(object sender, RoutedEventArgs e)
        {
            // Trying some stuff with button size
            //this.SizeChanged += ResponsiveContacts_SizeChanged;
            //orginalWindowRect = new Rect(this.Left, this.Top, this.ActualWidth, this.ActualHeight);
            //buttonOriginalRect = new Rect(Canvas.GetLeft(newButton), Canvas.GetTop(newButton), newButton.ActualWidth, newButton.ActualHeight);

            // Initialize Grid data
            InitializeContactsGrid();

            // Activate automatic generation of columns
            //contactsDataGrid.AutoGenerateColumns = true;

            // Dummy data
            contacts.Rows.Add("Max", "Mustermann", "max@example.com", "123456789");
            contacts.Rows.Add("Anna", "Schmidt", "anna@example.com", "987654321");

            // Set readonly
            contactsDataGrid.IsReadOnly = true;

            // Set data source
            contactsDataGrid.ItemsSource = contacts.DefaultView;
        }

        // Helper for checking if input is numeric
        private bool IsNumeric(string text)
        {
            // Return true if conversion was a success, false if not. The result is not needed, hence _ used as placeholder
            return int.TryParse(text, out _);
        }

        // Only accept numbers in the phone textbox (for usability)
        private void phoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if the text is not a number
            if (!IsNumeric(e.Text))
            {
                // Mark the event so that the input is not accepted
                e.Handled = true;

                // Error msg
                MessageBox.Show("Bitte geben Sie nur Zahlen ein!");
            }
        }

        #region Eventhandlers for Buttons and Grid doubleclick QoL
        private void newButton_Click(object sender, RoutedEventArgs e)
        {   // To save the contact when clicking this button instead of the save button
            // contacts.Rows.Add(firstNameTextBox.Text, lastNameTextBox.Text, emailTextBox.Text, phoneTextBox.Text);

            // Clear textboxes
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            emailTextBox.Text = "";
            phoneTextBox.Text = "";
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            // If a contact is selected
            if (contactsDataGrid.SelectedItem != null)
            {
                // Select the data in the selected row and create object that contains the data
                DataRowView row = (DataRowView)contactsDataGrid.SelectedItem;

                // Put the contacts informating in the textboxes
                firstNameTextBox.Text = row["Vorname"].ToString();
                lastNameTextBox.Text = row["Nachname"].ToString();
                emailTextBox.Text = row["Email"].ToString();
                phoneTextBox.Text = row["Telefon"].ToString();

                // Set editing to know if a contact is being edited
                editing = true;
            }
            else
            {
                // If no contact is selected error msg
                MessageBox.Show("Bitte Kontakt auswählen!");
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Add or edit a contact
            // Check if a contact is being edited and if a contact is being selected
            if (editing && contactsDataGrid.SelectedItem != null)
            {
                // Select the data in the selected row and create object that contains the data
                DataRowView selectedRow = (DataRowView)contactsDataGrid.SelectedItem;

                // Edit the contact
                selectedRow["Vorname"] = firstNameTextBox.Text;
                selectedRow["Nachname"] = lastNameTextBox.Text;
                selectedRow["Email"] = emailTextBox.Text;
                selectedRow["Telefon"] = phoneTextBox.Text;
            }
            else
            {
                // If the user should be able to create a new contact with the save button instead auf the new button, using this button for QoL reasons
                // Check if all of the textboxes are empty
                if (!string.IsNullOrEmpty(firstNameTextBox.Text) || 
                    !string.IsNullOrEmpty(lastNameTextBox.Text) || 
                    !string.IsNullOrEmpty(emailTextBox.Text) ||
                    !string.IsNullOrEmpty(phoneTextBox.Text))
                {
                    // Add contact
                    contacts.Rows.Add(firstNameTextBox.Text, lastNameTextBox.Text, emailTextBox.Text, phoneTextBox.Text);

                    // Success msg
                    MessageBox.Show("Neuen Kontakt erfolgreich hinzugefügt!");
                }
                else
                {
                    // Error msg
                    MessageBox.Show("Bitte zuerst einen Kontakt auswählen oder hinzufügen!");
                }
                
            }

            // Clear textboxes
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            emailTextBox.Text = "";
            phoneTextBox.Text = "";
            
            // Done editing
            editing = false;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Try catch for errors
            try
            {
                // Select the data in the selected row and create object that contains the data
                DataRowView selectedRow = (DataRowView)contactsDataGrid.SelectedItem;

                // Delete the selected contact data
                selectedRow.Delete();
            }
            // Specific error msg in case something goes wrong with the object
            catch (InvalidCastException) { MessageBox.Show("Fehler beim konvertieren des objekts"); }
            // Basic error msg
            catch(Exception) { MessageBox.Show("Keinen gültigen Kontakt ausgewählt"); }
        }

        // Grid doubleclick QoL, same function as the edit button
        private void contactsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if a contact is being selected
            if (contactsDataGrid.SelectedItem != null)
            {
                // Select the data in the selected row and create object that contains the data
                DataRowView row = (DataRowView)contactsDataGrid.SelectedItem;

                // Put the contacts informating in the textboxes
                firstNameTextBox.Text = row["Vorname"].ToString();
                lastNameTextBox.Text = row["Nachname"].ToString();
                emailTextBox.Text = row["Email"].ToString();
                phoneTextBox.Text = row["Telefon"].ToString();

                // Set editing to know if a contact is being edited
                editing = true;
            }
        }
        #endregion

        // Trying some stuff with button size
        //private void ResizeControl(Rect r, Control c)
        //{
        //    //float xRatio = (float)(this.Width) / (float)(orginalWindowRect.Width);
        //    //float yRatio = (float)(this.Height) / (float)(orginalWindowRect.Height);

        //    //int newX = (int)(r.Width * xRatio);
        //    //int newY = (int)(r.Height * yRatio);

        //    //uint newWidth = (uint)(r.Width * xRatio);
        //    //uint newHeight = (uint)(r.Height * yRatio);

        //    //c.Width = newWidth; c.Height = newHeight;
        //    //Canvas.SetLeft(c, newX);
        //    //Canvas.SetTop(c, newY);
        //}

        //private void ResponsiveContacts_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    //ResizeControl(buttonOriginalRect, newButton);
        //}
    }
}
