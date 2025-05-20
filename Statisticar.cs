using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace Statisticar
{
    public partial class Statisticar : Form
    {
        private SQLiteConnection connection;
        string folderPath = Path.Combine(Application.StartupPath, "Data");
        private void Statisticar_Load(object sender, EventArgs e)
        {
            connection = new SQLiteConnection();
            connection.ConnectionString = $"Data Source={Path.Combine(folderPath, "default.db")};Version=3;";
            connection.Open();
            textBox2.Enabled = false;
            Golovi.Enabled = false;
            Asistencije.Enabled = false;
            ZutiKartoni.Enabled = false;
            CrveniKartoni.Enabled = false;
            SacuvaneMreze.Enabled = false;
            Nick.Enabled = false;
            radioButton1.Enabled = false;
            radioButton3.Enabled = false;
            radioButton2.Enabled = false;
        }
        public Statisticar()
        {
            InitializeComponent();
            UcitajBaze();
        }

        private void OsveziListu()
        {

            try
            {
                string selectedDbName = label9.Text;
                string dbFileName = selectedDbName + ".db";
                string dbPath = Path.Combine(folderPath, dbFileName);

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.ConnectionString = $"Data Source={dbPath};Version=3;";
                connection.Open();


                string query = "SELECT * FROM '" + selectedDbName + "'";

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri prikazu podataka: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UcitajSaListe()
        {

            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Molimo vas izaberite bazu iz liste.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string selectedDbName = listBox1.SelectedItem.ToString();
                string dbFileName = selectedDbName + ".db";
                string dbPath = Path.Combine(folderPath, dbFileName);

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.ConnectionString = $"Data Source={dbPath};Version=3;";
                connection.Open();


                string query = "SELECT * FROM '"+selectedDbName+"'";

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                    label9.Text = listBox1.SelectedItem.ToString();
                }

                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri prikazu podataka: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteDatabase()
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Molimo vas izaberite bazu koju želite da obrišete.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string listboxitem = listBox1.SelectedItem.ToString();
            string selectedFile = listBox1.SelectedItem.ToString() + ".db";

            DialogResult result = MessageBox.Show($"Da li ste sigurni da želite da obrišete bazu '{selectedFile}'?", "Potvrda brisanja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {

                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    string filePath = Path.Combine(folderPath, selectedFile);

                    File.Delete(filePath);
              
                    listBox1.Items.Remove(listboxitem);

                    MessageBox.Show("Baza je uspešno obrisana.", "Brisanje uspešno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri brisanju: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }              
            }
        }

        private void UcitajBaze()
        { 
            listBox1.Items.Clear();

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Data folder ne postoji.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] dbFiles = Directory.GetFiles(folderPath, "*.db");
            string excludedFileName = "default";

            foreach (string dbFile in dbFiles)
            {
                string dbName = Path.GetFileNameWithoutExtension(dbFile);
                if (!dbName.Equals(excludedFileName, StringComparison.OrdinalIgnoreCase))
                {
                    listBox1.Items.Add(Path.GetFileName(dbName));
                }
            }
        }

        private void NapraviBazu()
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string dbPath = Path.Combine(folderPath, textBox1.Text + ".db");

                if (File.Exists(dbPath))
                {
                    MessageBox.Show($"Baza sa imenom '{textBox1.Text}' već postoji.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
           

                SQLiteConnection.CreateFile(dbPath);

                using (SQLiteConnection con = new SQLiteConnection($"data source={dbPath}"))
                {
                    con.Open();

                    string createTableQuery = $@"
                    CREATE TABLE '{textBox1.Text}' (
                    Nick TEXT NOT NULL UNIQUE,
                    Golovi INTEGER,
                    Asistencije INTEGER,
                    ZutiKartoni INTEGER,
                    CrveniKartoni INTEGER,
                    SacuvaneMreze INTEGER
                    );";

                    using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Baza je uspešno napravljena.", "Napravljeno!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri pravljenju: " + ex.Message, "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UcitajBaze();
            }
        }

        private void UpdateIme()
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show($"Polje za novo ime je prazno.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
             {
                if (Nick.Text != "")
                {
                    string selectedDbName = label9.Text.ToString();
                    string dbFileName = selectedDbName + ".db";
                    string dbPath = Path.Combine(folderPath, dbFileName);

                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    connection.ConnectionString = $"Data Source={dbPath};Version=3;";
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM '" + selectedDbName + "' WHERE Nick = '" + textBox2.Text + "'";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Ime već postoji.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string updateQuery = "UPDATE '" + selectedDbName + "' SET Nick = '" + textBox2.Text + "' WHERE Nick = '" + Nick.Text + "'";
                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Ime je uspešno izmenjeno.", "Uspešno!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            OsveziListu();
                            Nick.Text = "";
                            textBox2.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Promena neuspešna. Igrač verovatno ne postoji u bazi.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    
                }
                else
                {
                    MessageBox.Show("Polje za ime je prazno.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
           
        }

        private void PretraznjaImena()
            {

            if (Nick.Text == "")
            {
                OsveziListu();
            }
            else
            {
                string selectedDbName = label9.Text.ToString();
                string dbFileName = selectedDbName + ".db";
                string dbPath = Path.Combine(folderPath, dbFileName);

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.ConnectionString = $"Data Source={dbPath};Version=3;";
                connection.Open();

                string query = $"SELECT * FROM '" + label9.Text + "' WHERE Nick LIKE @Nick;";

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@Nick", "%" + Nick.Text + "%");
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
            }

        private void ObrisiIgraca()
        {
            if (label9.Text != "Ništa nije selektovano.")
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Izaberi igrača kog želiš da obrišeš.");
                    return;
                }

                string selectedNick = dataGridView1.SelectedRows[0].Cells["Nick"].Value.ToString();

                DialogResult confirmResult = MessageBox.Show($"Da li si siguran da želiš da obrišeš igrača '{selectedNick}'?",
                                                     "Potvrda",
                                                     MessageBoxButtons.YesNo);

                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }


                string selectedDbName = label9.Text.ToString();
                string dbFileName = selectedDbName + ".db";
                string dbPath = Path.Combine(folderPath, dbFileName);

                try
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    string query = "DELETE FROM aa WHERE Nick = '" + selectedNick + "'";

                    connection.ConnectionString = $"Data Source={dbPath};Version=3;";
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                    OsveziListu();
                }
            }
            else
            {
                MessageBox.Show($"Nije učitana baza.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PromenaStatistike()
        {
            if (Nick.Text != "")
            {
                string selectedDbName = label9.Text.ToString();
                string dbFileName = selectedDbName + ".db";
                string dbPath = Path.Combine(folderPath, dbFileName);

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                string query = "UPDATE '" + label9.Text + "' SET Golovi = Golovi + '" + Golovi.Text + "', Asistencije = Asistencije + '" + Asistencije.Text + "', ZutiKartoni = ZutiKartoni + '" + ZutiKartoni.Text + "', CrveniKartoni = CrveniKartoni + '" + CrveniKartoni.Text + "', SacuvaneMreze = SacuvaneMreze + '" + SacuvaneMreze.Text + "' WHERE Nick = '" + Nick.Text + "'";

                connection.ConnectionString = $"Data Source={dbPath};Version=3;";
                connection.Open();

                string checkQuery = "SELECT COUNT(1) FROM '" + selectedDbName + "' WHERE Nick = '"+Nick.Text+"'";
                using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                {
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        MessageBox.Show($"Igrač sa nazivom '"+Nick.Text+"' ne postoji.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);                     
                        return;
                    }
                }

                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
                MessageBox.Show($"Ažurirani podaci.", "Promenjeno!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OsveziListu();
            }
            else
            {
                MessageBox.Show($"Ni jedan igrač nije unet.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NoviIgrac()
        {
            if (Nick.Text != "")
            {


                string selectedDbName = label9.Text.ToString();
                string dbFileName = selectedDbName + ".db";
                string dbPath = Path.Combine(folderPath, dbFileName);

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                string query = "INSERT INTO '" + label9.Text + "' (Nick, Golovi, Asistencije, ZutiKartoni, CrveniKartoni, SacuvaneMreze) VALUES ('" + Nick.Text + "', 0, 0, 0, 0, 0)";

                connection.ConnectionString = $"Data Source={dbPath};Version=3;";
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
                MessageBox.Show($"Novi igrač je unet.", "Uspešno!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OsveziListu();
            }
            else
            {
                MessageBox.Show($"Polje za ime je prazno.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {   
                Nick.Enabled = true;
                Golovi.Enabled = false;
                Asistencije.Enabled = false;
                ZutiKartoni.Enabled = false;
                CrveniKartoni.Enabled = false;
                SacuvaneMreze.Enabled = false;
                textBox2.Enabled = false;

            Nick.Text = "";
            Golovi.Text = "";
            Asistencije.Text = "";
            ZutiKartoni.Text = "";
            CrveniKartoni.Text = "";
            SacuvaneMreze.Text = "";
            textBox2.Text = "";

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
                Nick.Enabled = true;
                Golovi.Enabled = true;
                Asistencije.Enabled = true;
                ZutiKartoni.Enabled = true;
                CrveniKartoni.Enabled = true;
                SacuvaneMreze.Enabled = true;
                textBox2.Enabled = false;
            Nick.Text = "";
            Golovi.Text = "";
            Asistencije.Text = "";
            ZutiKartoni.Text = "";
            CrveniKartoni.Text = "";
            SacuvaneMreze.Text = "";
            textBox2.Text = "";
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            Nick.Text = "";
            Golovi.Text = "";
            Asistencije.Text = "";
            ZutiKartoni.Text = "";
            CrveniKartoni.Text = "";
            SacuvaneMreze.Text = "";
            textBox2.Enabled = false;
            Golovi.Enabled = false;
            Asistencije.Enabled = false;
            ZutiKartoni.Enabled = false;
            CrveniKartoni.Enabled = false;
            SacuvaneMreze.Enabled = false;
            Nick.Enabled = false;
            
        
}

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show($"Unesi naziv.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
            NapraviBazu();
            textBox1.Text = "";
            }
        }

        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            UcitajSaListe();
        }
   
        private void button4_Click(object sender, EventArgs e)
        {
            DeleteDatabase();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
                e.Handled = e.KeyChar == ' ';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                UpdateIme();           
            }
            else if (radioButton2.Checked == true)
            {
                PromenaStatistike();
            }
            else if (radioButton3.Checked == true)
            {
                NoviIgrac();
            }
            else
            {
                MessageBox.Show($"Nisi selektovao koji izbor manipulacije želiš.", "Greška!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Nick.Enabled = true;
            Golovi.Enabled = false;
            Asistencije.Enabled = false;
            ZutiKartoni.Enabled = false;
            CrveniKartoni.Enabled = false;
            SacuvaneMreze.Enabled = false;
            textBox2.Enabled = true;
            Nick.Text = "";
            Golovi.Text = "";
            Asistencije.Text = "";
            ZutiKartoni.Text = "";
            CrveniKartoni.Text = "";
            SacuvaneMreze.Text = "";
            textBox2.Text = "";

        }

        private void Nick_TextChanged(object sender, EventArgs e)
        {
            PretraznjaImena();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ObrisiIgraca();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
          
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
